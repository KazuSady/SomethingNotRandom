using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MugGameplay : MonoBehaviour
{
    [Header("Aeropress")]
    [SerializeField] private XRSocketInteractor aeropressSocket;
    [Header("Kettle")]
    [SerializeField] private XRSocketInteractor kettleSocket;
    
    private bool _hasAeropress;
    private bool _hasWater;
    private GameObject _aeropress;
    
    public void AeropressPlaced()
    {
        _hasAeropress = true;
        var aeropress = aeropressSocket.GetOldestInteractableSelected();
        _aeropress = aeropress.transform.gameObject;
        Debug.Log("Aeropress attached");
        kettleSocket.gameObject.SetActive(true);
    }

    public void AeropressRemoved()
    {
        _hasAeropress = false;
        
        kettleSocket.gameObject.SetActive(false);
        Debug.Log("Aeropress removed");
    }
    
    public void KettlePlaced()
    {
        var kettle = kettleSocket.GetOldestInteractableSelected();
        kettle.transform.GetComponent<Animator>().Play("Kettle");
        _hasWater = true;
        _aeropress.GetComponent<TubeGameplay>().ActivatePressSocket();
    }
}
