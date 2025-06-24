using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class StrainerGameplay : MonoBehaviour
{
    public event Action<bool> FilterAttached;
    [SerializeField] private XRSocketInteractor filterSocket;
    [SerializeField] private BoxCollider mainCollider;

    private bool _hasFilterAttached;
    private GameObject _filter;

    public GameObject Filter => _filter;
    public bool HasFilter => _hasFilterAttached;

    public void FilterPlaced()
    {
        _hasFilterAttached = true;
        _filter = filterSocket.GetOldestInteractableSelected().transform.gameObject;
        Physics.IgnoreCollision(mainCollider, _filter.GetComponentInChildren<BoxCollider>(), true);
        FilterAttached?.Invoke(true);
    }

    public void FilterRemoved()
    {
        _hasFilterAttached = false;
        Physics.IgnoreCollision(mainCollider, _filter.GetComponentInChildren<BoxCollider>(), false);
        _filter = null;
        FilterAttached?.Invoke(false);
    }

    public void DisableFilterInteract()
    {
        if (_filter == null)
        {
            return;
        }

        if (_filter.TryGetComponent<XRGrabInteractable>(out var grab))
        {
            var interactor = grab.firstInteractorSelecting;
            if (interactor != null)
            {
                var manager = grab.interactionManager;
                if (manager)
                {
                    manager.CancelInteractableSelection(grab as IXRSelectInteractable);
                }
            }
            grab.interactionLayers &= ~InteractionLayerMask.GetMask("Default");
        }

        if (_filter.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void EnableFilterInteract()
    {
        if (_filter == null)
        {
            return;
        }
        
        if (_filter.TryGetComponent<XRGrabInteractable>(out var grab))
        {
            grab.interactionLayers |= InteractionLayerMask.GetMask("Default");
        }
    }
}
