using UnityEngine;


public class AutoAddDirectInteractor : MonoBehaviour
{
    [Header("Names to search for (edit to match your hierarchy)")]
    public string leftHandNameContains = "Left";
    public string rightHandNameContains = "Right";
    public string handNameContains = "Hand";

    public float sphereRadius = 0.05f;

    void Update()
    {
        TrySetupHand(true);
        TrySetupHand(false);
    }

    void TrySetupHand(bool left)
    {
        // Find candidate hand objects
        var all = GameObject.FindObjectsOfType<Transform>(true);
        foreach (var t in all)
        {
            string n = t.name;
            if (!n.Contains(handNameContains)) continue;
            if (left && !n.Contains(leftHandNameContains)) continue;
            if (!left && !n.Contains(rightHandNameContains)) continue;

            var go = t.gameObject;

            // If already has interactor, done
            if (go.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>() != null) return;

            // Add Direct Interactor
            go.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();

            // Add trigger sphere for interaction volume (if missing)
            var col = go.GetComponent<SphereCollider>();
            if (col == null) col = go.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = sphereRadius;

            // Add kinematic rigidbody (needed for triggers)
            var rb = go.GetComponent<Rigidbody>();
            if (rb == null) rb = go.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            return;
        }
    }
}
