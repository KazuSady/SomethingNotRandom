using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TubeGameplay : MonoBehaviour
{
    [Header("Strainer")]
    [SerializeField] private XRSocketInteractor strainerSocket;
    [Header("Press")]
    [SerializeField] private XRSocketInteractor pressSocket;
    
    private bool _hasStrainerAttached;
    private bool _hasPressAttached;
    
    public void StrainerPlaced()
    {
        _hasStrainerAttached = true;
        var strainer = strainerSocket.GetOldestInteractableSelected();
        strainer.transform.SetParent(strainerSocket.transform, false);
        strainer.transform.localPosition = Vector3.zero;
        Debug.Log("Strainer attached");
    }

    public void StrainerRemoved()
    {
        _hasStrainerAttached = false;
        Debug.Log("Strainer removed");
    }
    
    public void PressPlaced()
    {
        _hasPressAttached = true;
        var press = pressSocket.GetOldestInteractableSelected();
        press.transform.SetParent(pressSocket.transform, false);
        press.transform.localPosition = Vector3.zero;
        Debug.Log("Press attached");
    }

    public void PressRemoved()
    {
        _hasPressAttached = false;
        Debug.Log("Press removed");
    }
}
