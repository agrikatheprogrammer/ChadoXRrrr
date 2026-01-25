using UnityEngine;

public class CupFill : MonoBehaviour
{
    [Range(0f, 1f)] public float fill = 0f;

    public void AddWater(float amount01)
    {
        fill = Mathf.Clamp01(fill + amount01);
        // TODO: update visuals (water mesh height / shader) using fill
    }
}
