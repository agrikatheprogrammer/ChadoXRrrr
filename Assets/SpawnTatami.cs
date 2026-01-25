using UnityEngine;

public class SpawnTatami : MonoBehaviour
{
    [Header("Hand references (drag these in)")]
    public Transform leftPalm;
    public Transform rightPalm;

    [Header("Tatami")]
    public GameObject tatamiPrefab;
    public float tatamiSizeMeters = 3f;

    [Header("Floor detection")]
    public float raycastDownDistance = 2.0f;
    public float palmToFloorMaxDistance = 0.08f; // 8cm
    public float maxSlopeDegrees = 15f;          // floor check
    public float holdTimeSeconds = 0.25f;        // require both hands held briefly

    [Header("Debug")]
    public bool drawDebugRays = true;

    private float holdTimer = 0f;
    private GameObject spawnedTatami;

    void Start()
    {
        // If you want the tatami hidden until placed, DON'T place it in the scene.
        // Only use the prefab reference above.
    }

    void Update()
    {
        if (spawnedTatami != null) return;
        if (leftPalm == null || rightPalm == null || tatamiPrefab == null) return;

        bool leftOnFloor  = TryGetFloorHit(leftPalm.position, out RaycastHit leftHit);
        bool rightOnFloor = TryGetFloorHit(rightPalm.position, out RaycastHit rightHit);

        if (leftOnFloor && rightOnFloor)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdTimeSeconds)
            {
                Vector3 spawnPos = (leftHit.point + rightHit.point) * 0.5f;

                // Keep tatami flat on the floor (match normal)
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, leftHit.normal);

                spawnedTatami = Instantiate(tatamiPrefab, spawnPos, rot);

                // Optional: snap scale to 3x3m if your prefab isn't already correct
                // (Assumes tatami mesh is 1x1 units originally; if not, remove this)
                // spawnedTatami.transform.localScale = new Vector3(tatamiSizeMeters, 1f, tatamiSizeMeters);

                // tiny lift to avoid z-fighting
                spawnedTatami.transform.position += leftHit.normal * 0.002f;
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }

    bool TryGetFloorHit(Vector3 palmPos, out RaycastHit hit)
    {
        hit = default;

        // raycast from a little above the palm downwards
        Vector3 origin = palmPos + Vector3.up * 0.02f;
        Vector3 dir = Vector3.down;

        if (drawDebugRays)
            Debug.DrawRay(origin, dir * raycastDownDistance, Color.green);

        if (!Physics.Raycast(origin, dir, out hit, raycastDownDistance, ~0, QueryTriggerInteraction.Ignore))
            return false;

        // Must be "floor-like" (mostly horizontal)
        float slope = Vector3.Angle(hit.normal, Vector3.up);
        if (slope > maxSlopeDegrees) return false;

        // Palm must be close enough to the floor hit
        float verticalDist = Mathf.Abs(origin.y - hit.point.y);
        if (verticalDist > palmToFloorMaxDistance) return false;

        return true;
    }
}
