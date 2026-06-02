using System.Collections;
using UnityEngine;

/// <summary>
/// Owns the tea ceremony flow. Nothing is shown until the room is placed; on placement it
/// reveals the host (Seto) and gates the narration so it begins post-placement.
///
/// Every state transition is its own public method so a future STYLY netsync layer can wrap
/// each one in NetSyncManager.Rpc(...) without touching the logic.
/// </summary>
public class CeremonyDirector : MonoBehaviour
{
    public enum CeremonyState { WaitingToPlace, Intro, RunningSteps, Done }

    [Header("Placement")]
    public TatamiPlacer placer;
    public GameObject roomRoot;

    [Header("Host (Seto)")]
    public SetoAnimationTrigger seto;
    [Tooltip("Seto's GameObject. Disabled at start, enabled on placement.")]
    public GameObject setoRoot;
    public float setoIntroDelay = 0.5f;

    [Header("Narration")]
    public UIScript narration;
    [Tooltip("The UIScript GameObject. If it lives under roomRoot you can leave this empty (the room toggle gates it).")]
    public GameObject narrationRoot;

    public CeremonyState State { get; private set; }

    void Start()
    {
        State = CeremonyState.WaitingToPlace;

        if (setoRoot != null) setoRoot.SetActive(false);
        if (narrationRoot != null) narrationRoot.SetActive(false);

        if (placer != null) placer.OnPlaced += HandlePlaced;
    }

    void OnDestroy()
    {
        if (placer != null) placer.OnPlaced -= HandlePlaced;
    }

    void HandlePlaced(Vector3 position, Quaternion rotation)
    {
        EnterIntro();
    }

    /// <summary>Reveal host + narration after the room has been placed.</summary>
    public void EnterIntro()
    {
        if (State != CeremonyState.WaitingToPlace) return;
        State = CeremonyState.Intro;

        if (setoRoot != null) setoRoot.SetActive(true);
        if (narrationRoot != null) narrationRoot.SetActive(true);

        if (isActiveAndEnabled) StartCoroutine(PlayHostGreeting());
        else if (seto != null) seto.PlayOpenScene();
    }

    IEnumerator PlayHostGreeting()
    {
        if (setoIntroDelay > 0f) yield return new WaitForSeconds(setoIntroDelay);
        if (seto != null) seto.PlayOpenScene();
        EnterRunningSteps();
    }

    /// <summary>User is now working through the narrated steps.</summary>
    public void EnterRunningSteps()
    {
        if (State != CeremonyState.Intro) return;
        State = CeremonyState.RunningSteps;
    }

    /// <summary>Narration complete. Wire UIScript.onNarrationFinished here.</summary>
    public void Finish()
    {
        State = CeremonyState.Done;
        Debug.Log("[CeremonyDirector] Ceremony complete.");
    }
}
