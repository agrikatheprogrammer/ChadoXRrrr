using UnityEngine;

public class LadleWater : MonoBehaviour
{
    [Range(0f, 1f)] public float amount = 0f; // 0..1

    public bool HasWater => amount > 0.01f;

    // Infinite kettle fallback: set to full instantly
    public void FillInstant() => amount = 1f;

    // Add water (used when scooping from a finite kettle)
    public void Add(float a) => amount = Mathf.Clamp01(amount + a);

    [Tooltip("Pour once the ladle is tilted more than this many degrees from upright. Works whichever way you rotate it (clockwise OR counter-clockwise).")]
    public float pourTiltAngle = 55f;

    public bool IsPouringDown()
    {
        // How far the ladle is tilted from upright, regardless of rotation direction.
        // Clockwise or counter-clockwise both pour once past pourTiltAngle.
        return Vector3.Angle(transform.up, Vector3.up) > pourTiltAngle;
    }

    public float Pour(float dt, float rate)
    {
        float poured = Mathf.Min(amount, rate * dt);
        amount -= poured;
        return poured;
    }
}
