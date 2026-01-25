using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRInteractorTriggerForwarder : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor;
    void Awake()
    {
        if (interactor == null) interactor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
        if (interactor != null)
        {
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
        // Try to call the same method your trigger-based buttons expect
        go.SendMessage("OnTriggerEnter", GetInteractorCollider(), SendMessageOptions.DontRequireReceiver);
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        var go = args.interactableObject?.transform?.gameObject;
        if (go == null) return;
        go.SendMessage("OnTriggerExit", GetInteractorCollider(), SendMessageOptions.DontRequireReceiver);
    }

    Collider GetInteractorCollider()
    {
        // return any Collider on the interactor to mimic physics callback param (may be null)
        return interactor.transform.GetComponentInChildren<Collider>();
    }
}