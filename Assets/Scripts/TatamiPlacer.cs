using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Reticle-based placement for the tea room. From start the room is hidden and a reticle
/// follows where the user points, hitting a pure-math floor plane (no physics collider, no
/// AR plane). On confirm, the existing in-scene room is repositioned, rotated to face the
/// player, revealed, and OnPlaced fires. Works identically in the editor and on Quest.
/// </summary>
public class TatamiPlacer : MonoBehaviour
{
    [Header("What to place")]
    [Tooltip("The existing in-scene tea room root (e.g. 'OKOKKOKFINALLL 1 1'). Hidden at start, revealed on placement.")]
    public GameObject roomRoot;

    [Header("Pointer")]
    [Tooltip("Right-hand controller/interactor transform. If empty, falls back to the main camera.")]
    public Transform pointerTransform;
    [Tooltip("Used to rotate the room to face the player. Auto-fills from Camera.main if empty.")]
    public Transform playerCamera;
    [Tooltip("Math floor height. Start at 0; adjust on Quest if the room sits off the real floor.")]
    public float floorY = 0f;
    public float maxRayDistance = 10f;
    [Tooltip("Rotate the room to face the player on the horizontal plane only (no tilt).")]
    public bool facePlayerYawOnly = true;

    [Header("Dwell placement (no pinch needed)")]
    [Tooltip("Point at the floor and hold steady to place automatically. Avoids the pinch/Meta-menu conflict.")]
    public bool useDwellPlacement = true;
    [Tooltip("Seconds to hold the reticle steady before it places.")]
    public float dwellTime = 1.5f;
    [Tooltip("If the reticle moves more than this (metres), the dwell timer resets.")]
    public float dwellMoveTolerance = 0.06f;

    [Header("Confirm input (optional, in addition to dwell)")]
    [Tooltip("Assign 'XRI STYLY Input Actions/Select' (pinch on hands, grip/trigger on controllers).")]
    public InputActionReference confirmAction;
    [Tooltip("Optional second confirm binding (e.g. left-hand Select).")]
    public InputActionReference confirmActionAlt;
    [Tooltip("Editor keyboard fallback.")]
    public Key editorConfirmKey = Key.Space;
    [Tooltip("Also accept left mouse click in the editor.")]
    public bool acceptEditorMouseClick = true;

    [Header("Reticle")]
    [Tooltip("Optional. If empty, a flat disc is built at runtime (no art needed).")]
    public GameObject reticlePrefab;

    [Header("Events")]
    public UnityEvent OnPlacedUnityEvent;
    /// <summary>Fired once when the room is placed. (worldPosition, worldRotation)</summary>
    public event Action<Vector3, Quaternion> OnPlaced;

    private GameObject reticle;
    private bool placed;
    private float dwellTimer;
    private Vector3 lastHit;
    private bool hasLastHit;

    void OnEnable()
    {
        confirmAction?.action?.Enable();
        confirmActionAlt?.action?.Enable();

        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        EnsureReticle();
        if (reticle != null) reticle.SetActive(false);

        if (!placed && roomRoot != null)
            roomRoot.SetActive(false);
    }

    void OnDisable()
    {
        confirmAction?.action?.Disable();
        confirmActionAlt?.action?.Disable();
        if (reticle != null) reticle.SetActive(false);
    }

    void Update()
    {
        if (placed) return;

        Ray ray = BuildPointerRay();

        if (TryRaycastFloor(ray, out Vector3 hit))
        {
            ShowReticle(hit);

            // Explicit confirm (pinch/trigger/keyboard) always works.
            if (ConfirmPressed())
            {
                Place(hit);
                return;
            }

            // Dwell: hold the reticle steady to place without any button.
            if (useDwellPlacement)
            {
                if (hasLastHit && (hit - lastHit).sqrMagnitude <= dwellMoveTolerance * dwellMoveTolerance)
                    dwellTimer += Time.deltaTime;
                else
                    dwellTimer = 0f;

                lastHit = hit;
                hasLastHit = true;

                // Reticle fills/brightens as the dwell completes (visual feedback, no art).
                UpdateDwellFeedback(dwellTimer / dwellTime);

                if (dwellTimer >= dwellTime)
                    Place(hit);
            }
        }
        else
        {
            if (reticle != null) reticle.SetActive(false);
            dwellTimer = 0f;
            hasLastHit = false;
        }
    }

    void UpdateDwellFeedback(float t)
    {
        if (reticle == null) return;
        t = Mathf.Clamp01(t);
        // Grow the disc slightly as it locks in.
        float s = Mathf.Lerp(0.3f, 0.42f, t);
        reticle.transform.localScale = new Vector3(s, 0.005f, s);
    }

    Ray BuildPointerRay()
    {
        if (pointerTransform != null)
            return new Ray(pointerTransform.position, pointerTransform.forward);
        if (playerCamera != null)
            return new Ray(playerCamera.position, playerCamera.forward);
        return new Ray(Vector3.up, Vector3.forward); // degenerate fallback; raycast will miss
    }

    bool TryRaycastFloor(Ray ray, out Vector3 point)
    {
        var plane = new Plane(Vector3.up, new Vector3(0f, floorY, 0f));
        if (plane.Raycast(ray, out float enter) && enter <= maxRayDistance)
        {
            point = ray.GetPoint(enter);
            return true;
        }
        point = default;
        return false;
    }

    bool ConfirmPressed()
    {
        if (confirmAction != null && confirmAction.action != null && confirmAction.action.WasPressedThisFrame())
            return true;
        if (confirmActionAlt != null && confirmActionAlt.action != null && confirmActionAlt.action.WasPressedThisFrame())
            return true;
        if (Keyboard.current != null && Keyboard.current[editorConfirmKey].wasPressedThisFrame)
            return true;
        if (acceptEditorMouseClick && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;
        return false;
    }

    void Place(Vector3 point)
    {
        Quaternion rotation = Quaternion.identity;
        if (playerCamera != null)
        {
            Vector3 dir = playerCamera.position - point;
            if (facePlayerYawOnly) dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
                rotation = Quaternion.LookRotation(dir);
        }

        if (roomRoot != null)
        {
            roomRoot.transform.SetPositionAndRotation(point, rotation);
            roomRoot.SetActive(true);
        }

        if (reticle != null) reticle.SetActive(false);
        placed = true;
        enabled = false;

        OnPlaced?.Invoke(point, rotation);
        OnPlacedUnityEvent?.Invoke();
    }

    /// <summary>Allow placing again (e.g. recalibration). Re-hides the room and re-enables the placer.</summary>
    public void ResetPlacer()
    {
        placed = false;
        if (roomRoot != null) roomRoot.SetActive(false);
        enabled = true;
    }

    void EnsureReticle()
    {
        if (reticle != null) return;

        if (reticlePrefab != null)
        {
            reticle = Instantiate(reticlePrefab);
        }
        else
        {
            reticle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            reticle.name = "PlacementReticle";
            var col = reticle.GetComponent<Collider>();
            if (col != null) Destroy(col);
            reticle.transform.localScale = new Vector3(0.3f, 0.005f, 0.3f); // 30cm flat disc

            var mr = reticle.GetComponent<MeshRenderer>();
            var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
            if (shader != null)
            {
                var mat = new Material(shader);
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", new Color(0.2f, 0.8f, 1f, 1f));
                else mat.color = new Color(0.2f, 0.8f, 1f, 1f);
                mr.material = mat;
            }
        }
        reticle.SetActive(false);
    }

    void ShowReticle(Vector3 point)
    {
        if (reticle == null) return;
        reticle.transform.position = point;
        reticle.SetActive(true);
    }
}
