using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class StrainerGameplay : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor filterSocket;
    [SerializeField] private BoxCollider mainCollider;
    
    private bool _hasFilterAttached;
    private GameObject _filter;
    
    public bool HasFilter => _hasFilterAttached;
    
    public void FilterPlaced()
    {
        _hasFilterAttached = true;
        _filter = filterSocket.GetOldestInteractableSelected().transform.gameObject;
        Physics.IgnoreCollision(mainCollider, _filter.GetComponentInChildren<BoxCollider>(), true);
    }

    public void FilterRemoved()
    {
        _hasFilterAttached = false;
        Physics.IgnoreCollision(mainCollider, _filter.GetComponentInChildren<BoxCollider>(), false);
        _filter = null;
    }
}
