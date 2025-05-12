using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TubeGameplay : MonoBehaviour
{
    [Header("Strainer")]
    [SerializeField] private XRSocketInteractor strainerSocket;
    [Header("Press")]
    [SerializeField] private XRSocketInteractor pressSocket;
    [Header("Coffee")]
    [SerializeField] private XRSocketInteractor coffeSocket;
    
    private bool _hasStrainerAttached;
    private bool _hasPressAttached;
    private bool _hasCoffe;

    public void StrainerPlaced()
    {
        _hasStrainerAttached = true;
        Debug.Log("Strainer attached");
        coffeSocket.gameObject.SetActive(true);
        Debug.Log(coffeSocket.gameObject.activeSelf);
    }

    public void StrainerRemoved()
    {
        _hasStrainerAttached = false;
        coffeSocket.gameObject.SetActive(false);
        Debug.Log("Strainer removed");
    }
    
    public void PressPlaced()
    {
        _hasPressAttached = true;
       
        Debug.Log("Press attached");
    }

    public void PressRemoved()
    {
        _hasPressAttached = false;
        Debug.Log("Press removed");
    }

    public void CoffePlaced()
    {
        var coffee = coffeSocket.GetOldestInteractableSelected();
        coffee.transform.GetComponent<Animator>().Play("Coffee");
        _hasCoffe = true;
    }

    public void ActivatePressSocket()
    {
        pressSocket.gameObject.SetActive(true);
    }

    
}
