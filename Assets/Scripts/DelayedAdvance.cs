using System.Collections;
using UnityEngine;

/// <summary>
/// Advances the narration after a delay, so a celebration (e.g. Seto's joyful jump) can play
/// before the next instruction appears. Wire CupFill.onTargetReached -> AdvanceAfterDelay().
/// </summary>
public class DelayedAdvance : MonoBehaviour
{
    [Tooltip("The narration UIScript to advance.")]
    public UIScript narration;
    [Tooltip("Seconds to wait (let the celebration play) before moving to the next step.")]
    public float delaySeconds = 2f;

    public void AdvanceAfterDelay()
    {
        if (isActiveAndEnabled) StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        yield return new WaitForSeconds(delaySeconds);
        if (narration != null) narration.OnNextTriggerEntered();
    }
}
