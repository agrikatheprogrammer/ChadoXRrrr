using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Logs the full grab setup a couple seconds after start, so a development build + logcat
/// shows exactly why grab is/ isn't working: how many interaction managers exist, which
/// interactors exist and what manager they use, and the ladle's grab interactable colliders
/// / manager / layers. Search logcat for "[GRABDIAG]".
/// Attach to any always-active object (e.g. PlacementManager) and make a Development Build.
/// </summary>
public class GrabDiagnostics : MonoBehaviour
{
    public float delay = 2f;

    void Start() => Invoke(nameof(Report), delay);

    void Report()
    {
        var mgrs = FindObjectsByType<XRInteractionManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"[GRABDIAG] XRInteractionManagers in scene: {mgrs.Length}");
        foreach (var m in mgrs)
            Debug.Log($"[GRABDIAG]   manager '{m.name}' active={m.isActiveAndEnabled}");

        var interactors = FindObjectsByType<XRBaseInteractor>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"[GRABDIAG] Interactors found: {interactors.Length}");
        foreach (var i in interactors)
            Debug.Log($"[GRABDIAG]   {i.GetType().Name} on '{i.name}' mgr={(i.interactionManager ? i.interactionManager.name : "NULL")} enabled={i.isActiveAndEnabled}");

        var grabs = FindObjectsByType<XRGrabInteractable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"[GRABDIAG] GrabInteractables found: {grabs.Length}");
        foreach (var g in grabs)
        {
            int colCount = g.colliders != null ? g.colliders.Count : 0;
            int nonNull = 0;
            if (g.colliders != null)
                foreach (var c in g.colliders) if (c != null) nonNull++;
            Debug.Log($"[GRABDIAG]   '{g.name}' mgr={(g.interactionManager ? g.interactionManager.name : "NULL")} " +
                      $"colliders(list)={colCount} nonNull={nonNull} layers={g.interactionLayers.value} " +
                      $"active={g.isActiveAndEnabled}");
        }
    }
}
