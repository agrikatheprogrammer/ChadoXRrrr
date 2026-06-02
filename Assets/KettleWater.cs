using UnityEngine;

/// <summary>
/// The kettle's water. Starts full and drains as the ladle scoops from it, lowering a visible
/// water surface (a disc) — the mirror of CupFill, which rises.
/// Put this on the kettle and assign the visible water surface.
/// </summary>
public class KettleWater : MonoBehaviour
{
    [Range(0f, 1f)] public float fill = 1f;          // starts full (level 0..1)
    [Tooltip("How many full ladle scoops the kettle holds before it's empty.")]
    public float ladleCapacity = 5f;
    [Tooltip("The visible water surface inside the kettle. Lowers as water is scooped out.")]
    public Transform waterSurface;
    [Tooltip("How far (in WORLD metres) the surface sits above empty when the kettle is full. e.g. 0.05 = 5cm.")]
    public float maxHeight = 0.05f;

    private Vector3 emptyLocalPos;   // where you placed the surface = the "empty" spot
    private bool captured;

    void Awake() => CaptureEmpty();

    void CaptureEmpty()
    {
        if (waterSurface != null) { emptyLocalPos = waterSurface.localPosition; captured = true; }
    }

    /// <summary>Remove up to 'amount' (in ladle units) of water; returns how much was taken.</summary>
    public float Take(float amount)
    {
        amount = Mathf.Max(0f, amount);
        float availableLadleUnits = fill * ladleCapacity;
        float taken = Mathf.Min(availableLadleUnits, amount);
        if (ladleCapacity > 0f) fill = Mathf.Clamp01(fill - taken / ladleCapacity);
        return taken;
    }

    void Update() => ApplyLevel();

    void ApplyLevel()
    {
        if (waterSurface == null) return;
        if (!captured) CaptureEmpty();

        Vector3 baseWorld = waterSurface.parent != null
            ? waterSurface.parent.TransformPoint(emptyLocalPos)
            : emptyLocalPos;

        // Rise straight UP in world space, regardless of how the kettle model is rotated.
        waterSurface.position = baseWorld + Vector3.up * (fill * maxHeight);
    }
}
