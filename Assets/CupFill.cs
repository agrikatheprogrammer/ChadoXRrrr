using UnityEngine;
public class CupFill : MonoBehaviour
{
    [Range(0f, 1f)] public float fill = 0f;  // Current fill level
    public Renderer waterRenderer;          // Material using shader
    public Transform waterTransform;        // Cube water object

    public void AddWater(float amount)
    {
        fill = Mathf.Clamp01(fill + amount);

        // Shader effect: Update `_Fill`
        if (waterRenderer != null)
        {
            waterRenderer.material.SetFloat("_Fill", fill);
        }

        // Physical scaling: Update water cube scale
        if (waterTransform != null)
        {
            waterTransform.localScale = new Vector3(
                waterTransform.localScale.x,
                fill, // Scale along vertical axis
                waterTransform.localScale.z
            );
            // Adjust position if necessary
            waterTransform.localPosition = new Vector3(
                waterTransform.localPosition.x,
                fill / 2f, // Keep it flush with the cup
                waterTransform.localPosition.z
            );
        }
    }
}