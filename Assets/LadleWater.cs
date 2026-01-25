using UnityEngine;

public class LadleWater : MonoBehaviour
{
    [Range(0f, 1f)] public float amount = 0f; // 0..1

    public bool HasWater => amount > 0.01f;

    // Infinite kettle: just set to full instantly
    public void FillInstant() => amount = 1f;

    public bool IsPouringDown()
    {
        // Change axis if your ladle model is oriented differently
        // This says: if ladle "up" points downward enough, it's pouring
        return Vector3.Dot(transform.up, Vector3.down) > 0.4f;
    }

    public float Pour(float dt, float rate)
    {
        float poured = Mathf.Min(amount, rate * dt);
        amount -= poured;
        return poured;
    }
}
