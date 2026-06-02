using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRInteractorTriggerForwarder : MonoBehaviour
{
    public XRBaseInteractor interactor;

    private Collider cachedCollider;

    void Awake()
    {
        if (interactor == null) interactor = GetComponent<XRBaseInteractor>();
        if (interactor != null)
        {
            cachedCollider = interactor.GetComponentInChildren<Collider>();
            interactor.selectEntered.AddListener(OnSelectEntered);
            interactor.selectExited.AddListener(OnSelectExited);
        }
    }

    void OnDestroy()
    {
        if (interactor != null)
        {
            interactor.selectEntered.RemoveListener(OnSelectEntered);
            interactor.selectExited.RemoveListener(OnSelectExited);
        }
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        var go = args.interactableObject?.transform?.gameObject;
        if (go == null) return;
        go.SendMessage("OnTriggerEnter", cachedCollider, SendMessageOptions.DontRequireReceiver);
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        var go = args.interactableObject?.transform?.gameObject;
        if (go == null) return;
        go.SendMessage("OnTriggerExit", cachedCollider, SendMessageOptions.DontRequireReceiver);
    }
}