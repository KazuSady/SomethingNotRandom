using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class StrainerGameplay : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor filterSocket;

    private bool _hasFilterAttached;
    
    public void FilterPlaced()
    {
        _hasFilterAttached = true;
        Debug.Log("Filter attached");
    }

    public void FilterRemoved()
    {
        _hasFilterAttached = false;
        Debug.Log("Filter removed");
    }
}
