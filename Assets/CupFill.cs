using UnityEngine;

public class CupFill : MonoBehaviour
{
    [Range(0f, 1f)] public float fill = 0f;  // Current fill level between 0 (empty) and 1 (full)
    public Renderer waterRenderer;          // Assign the Renderer of the water material in Unity

    public void AddWater(float amount01)
    {
        fill = Mathf.Clamp01(fill + amount01); // Adjust the fill level between 0 and 1

        UpdateWaterVisual(); // Update the shader visuals
    }

    void UpdateWaterVisual()
    {
        if (waterRenderer != null)
        {
            // Update the water material shader's "_Fill" property with the current fill level
            waterRenderer.material.SetFloat("_Fill", fill);
        }
    }
}