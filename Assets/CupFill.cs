using UnityEngine;

public class CupFill : MonoBehaviour
{
    [Range(0f, 1f)] public float fill = 0f;  // Current water level (0 = empty, 1 = full)
    public Transform waterSurface;          // Circular plane representing water surface
    public float maxHeight = 2f;            // Maximum height of water in the cup

    public void AddWater(float amount)
    {
        fill = Mathf.Clamp01(fill + amount);

        // Position the circular water surface plane dynamically
        if (waterSurface != null)
        {
            waterSurface.localPosition = new Vector3(
                waterSurface.localPosition.x,
                fill * maxHeight, // Adjust water surface height proportionally
                waterSurface.localPosition.z
            );
        }
    }
}