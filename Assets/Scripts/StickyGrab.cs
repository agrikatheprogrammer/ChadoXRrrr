using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Keeps a grabbed object "stuck" to the hand through brief hand-tracking gesture flickers
/// (e.g. when you rotate your wrist to pour, the pinch/grasp can momentarily drop).
///
/// When the grab is released, the object keeps following the releasing hand for a short grace
/// window. If you re-grab within that window the normal grab resumes seamlessly; if the window
/// expires, it's treated as a real release and the object is left where it is.
///
/// Attach to the ladle (it has the XRGrabInteractable). Pairs well with a Kinematic Rigidbody
/// + gravity off, so a true release just leaves the ladle in place.
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
public class StickyGrab : MonoBehaviour
{
    [Tooltip("How long (seconds) to keep following the hand after a release before truly letting go. " +
             "Bigger = more forgiving of flickers, but slower to deliberately drop. ~0.3–0.5 is good.")]
    public float graceSeconds = 0.4f;

    private XRGrabInteractable grab;
    private Transform hand;
    private Transform homeParent;
    private Coroutine routine;
    private bool reGrabbed;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        homeParent = transform.parent;
    }

    void OnEnable()
    {
        grab.selectEntered.AddListener(OnSelectEntered);
        grab.selectExited.AddListener(OnSelectExited);
    }

    void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnSelectEntered);
        grab.selectExited.RemoveListener(OnSelectExited);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        reGrabbed = true;
        if (args.interactorObject != null) hand = args.interactorObject.transform;

        // Cancel any sticky follow and hand control back to the grab interactable.
        if (routine != null) { StopCoroutine(routine); routine = null; }
        if (transform.parent != homeParent) transform.SetParent(homeParent, true);
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactorObject != null) hand = args.interactorObject.transform;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(StickWindow());
    }

    IEnumerator StickWindow()
    {
        reGrabbed = false;

        // Parent to the hand, preserving the current (grabbed) world pose, so the ladle keeps
        // following the hand during the flicker instead of being dropped.
        if (hand != null) transform.SetParent(hand, true);

        float t = 0f;
        while (t < graceSeconds && !reGrabbed)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // If we weren't re-grabbed, this was a real release: detach and leave it in place.
        if (!reGrabbed && transform.parent != homeParent)
            transform.SetParent(homeParent, true);

        routine = null;
    }
}
