using UnityEngine;

/// <summary>
/// Shows a small water disc inside the ladle bowl only when the ladle holds water,
/// and scales it with the amount. Cheap: a single opaque mesh toggled on/off.
/// Attach to the ladle; assign the LadleWater and the water-disc Transform.
/// </summary>
[RequireComponent(typeof(LadleWater))]
public class LadleWaterVisual : MonoBehaviour
{
    [Tooltip("The ladle's water state. Auto-found on this object if left empty.")]
    public LadleWater ladle;

    [Tooltip("A small flat disc (with the water material) positioned in the ladle bowl. Hidden when empty. Size it directly in its own Transform.")]
    public Transform waterVisual;

    private bool lastVisible;

    void Awake()
    {
        if (ladle == null) ladle = GetComponent<LadleWater>();
    }

    void OnEnable()
    {
        // Force the disc to match the current water state immediately, so it doesn't matter
        // whether the disc was left active or inactive in the editor.
        if (waterVisual != null && ladle != null)
        {
            lastVisible = ladle.HasWater;
            waterVisual.gameObject.SetActive(lastVisible);
        }
    }

    void Update()
    {
        if (waterVisual == null || ladle == null) return;

        // Just show the disc when there's water, hide it when empty.
        // The disc keeps whatever size you set in its own Transform.
        bool visible = ladle.HasWater;
        if (visible != lastVisible)
        {
            waterVisual.gameObject.SetActive(visible);
            lastVisible = visible;
        }
    }
}
