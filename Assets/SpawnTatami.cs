using UnityEngine;

public class SpawnTatami : MonoBehaviour
{
    [Header("Tatami")]
    public GameObject tatamiPrefab;

    [Header("Rules")]
    public float bothHandsHoldSeconds = 0.15f; // require brief simultaneous touch
    public float liftAboveFloor = 0.002f;

    bool leftOnFloor, rightOnFloor;
    Vector3 leftPoint, rightPoint;
    float holdTimer = 0f;

    GameObject spawned;

    void Update()
    {
        if (spawned != null) return;
        if (tatamiPrefab == null) return;

        if (leftOnFloor && rightOnFloor)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= bothHandsHoldSeconds)
            {
                Vector3 midpoint = (leftPoint + rightPoint) * 0.5f;

                // Force it onto your defined floor plane y = 0
                midpoint.y = 0f;

                // Flat on floor
                Quaternion rot = Quaternion.identity;

                spawned = Instantiate(tatamiPrefab, midpoint + Vector3.up * liftAboveFloor, rot);
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }

    public void SetHandOnFloor(bool isLeft, bool onFloor, Vector3 contactPoint)
    {
        if (isLeft)
        {
            leftOnFloor = onFloor;
            leftPoint = contactPoint;
        }
        else
        {
            rightOnFloor = onFloor;
            rightPoint = contactPoint;
        }
    }
}
