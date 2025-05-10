using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class StrainerGameplay : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor filterSocket;

    private bool _hasFilterAttached;
    
    public void FilterPlaced()
    {
        _hasFilterAttached = true;
        var filter = filterSocket.GetOldestInteractableSelected();
        filter.transform.SetParent(filterSocket.transform, false);
        filter.transform.localPosition = Vector3.zero;
        Debug.Log("Filter attached");
    }

    public void FilterRemoved()
    {
        _hasFilterAttached = false;
        Debug.Log("Filter removed");
    }
}
