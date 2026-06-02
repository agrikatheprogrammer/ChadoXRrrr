using UnityEngine;
using UnityEngine.Events;

public class CupFill : MonoBehaviour
{
    [Range(0f, 1f)] public float fill = 0f;   // Current water level (0 = empty, 1 = full)
    public Transform waterSurface;            // Disc representing the water surface
    [Tooltip("How far (in WORLD metres) the surface rises from empty to full. e.g. 0.05 = 5cm.")]
    public float maxHeight = 0.05f;

    [Header("Ceremony target")]
    [Tooltip("Cup capacity in millilitres. A full cup (fill = 1) = this many ml.")]
    public float capacityMl = 60f;
    [Tooltip("Fill fraction that counts as 'done' (1 = full = capacityMl).")]
    [Range(0f, 1f)] public float targetFill = 1f;
    [Tooltip("Fired ONCE when the cup first reaches the target amount (e.g. 60ml). Wire this to advance the narration step.")]
    public UnityEvent onTargetReached;

    /// <summary>Current amount of water in millilitres.</summary>
    public float CurrentMl => fill * capacityMl;

    private bool targetFired;
    private Vector3 emptyLocalPos;   // where you placed the surface = the "empty" spot
    private bool captured;

    void Awake() => CaptureEmpty();

    void CaptureEmpty()
    {
        if (waterSurface != null) { emptyLocalPos = waterSurface.localPosition; captured = true; }
    }

    public void AddWater(float amount)
    {
        fill = Mathf.Clamp01(fill + amount);

        if (!targetFired && fill >= targetFill - 0.001f)
        {
            targetFired = true;
            onTargetReached?.Invoke();
        }
    }

    void Update() => ApplyLevel();

    void ApplyLevel()
    {
        if (waterSurface == null) return;
        if (!captured) CaptureEmpty();

        // Base = where you placed the surface (empty), following the cup if it moves.
        Vector3 baseWorld = waterSurface.parent != null
            ? waterSurface.parent.TransformPoint(emptyLocalPos)
            : emptyLocalPos;

        // Rise straight UP in world space (water is always level, regardless of cup rotation).
        waterSurface.position = baseWorld + Vector3.up * (fill * maxHeight);
    }

    /// <summary>Reset so the target can fire again (e.g. if the step is retried).</summary>
    public void ResetCup()
    {
        fill = 0f;
        targetFired = false;
    }
}
