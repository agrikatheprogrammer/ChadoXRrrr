using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Adds a direct (touch) grab interactor to each hand ONCE, then stops.
///
/// The old version did this every frame via FindObjectsOfType<Transform>(true) and
/// AddComponent on every object named "*Hand*" — hundreds of calls per second that crushed
/// the framerate to ~1 FPS. This version searches only until both hands are set up
/// (throttled, a few times per second) and then disables itself, so there is no per-frame cost.
/// </summary>
public class AutoAddDirectInteractor : MonoBehaviour
{
    [Header("Names to search for (edit to match your hierarchy)")]
    public string leftHandNameContains = "Left";
    public string rightHandNameContains = "Right";
    public string handNameContains = "Hand";

    public float sphereRadius = 0.05f;

    [Tooltip("How often to look for the hands while waiting for them to appear (seconds).")]
    public float searchInterval = 0.5f;
    [Tooltip("Give up searching after this many seconds.")]
    public float searchTimeout = 30f;

    private bool leftDone, rightDone;
    private float nextSearch;
    private float elapsed;

    void Update()
    {
        if (leftDone && rightDone) { enabled = false; return; }

        elapsed += Time.deltaTime;
        if (elapsed > searchTimeout) { enabled = false; return; }

        if (Time.time < nextSearch) return;
        nextSearch = Time.time + searchInterval;

        if (!leftDone) leftDone = TrySetupHand(true);
        if (!rightDone) rightDone = TrySetupHand(false);
    }

    // Returns true once a hand has been set up.
    bool TrySetupHand(bool left)
    {
        var all = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        // Pick the best match: the hand ROOT, which has the shortest name
        // (e.g. "Left Hand" rather than "Left Hand Index Tip").
        Transform best = null;
        foreach (var t in all)
        {
            string n = t.name;
            if (!n.Contains(handNameContains)) continue;
            if (left && !n.Contains(leftHandNameContains)) continue;
            if (!left && !n.Contains(rightHandNameContains)) continue;

            if (best == null || n.Length < best.name.Length) best = t;
        }

        if (best == null) return false; // hand not found yet; will retry

        var go = best.gameObject;
        if (go.GetComponent<XRDirectInteractor>() != null) return true; // already set up

        go.AddComponent<XRDirectInteractor>();

        var col = go.GetComponent<SphereCollider>();
        if (col == null) col = go.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = sphereRadius;

        var rb = go.GetComponent<Rigidbody>();
        if (rb == null) rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        return true;
    }
}
