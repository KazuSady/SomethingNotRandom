using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MugGameplay : MonoBehaviour
{
    [SerializeField] private LiquidBehaviour liquid;
    [Header("Aeropress")]
    [SerializeField] private XRSocketInteractor aeropressSocket;
    [Header("Kettle")]
    [SerializeField] private XRSocketInteractor kettleSocket;
    
    private bool _hasAeropress;
    private bool _hasWater;
    private GameObject _aeropress;
    private GameObject _kettle;
    
    public void AeropressPlaced()
    {
        _hasAeropress = true;
        var aeropress = aeropressSocket.GetOldestInteractableSelected();
        _aeropress = aeropress.transform.gameObject;
        _aeropress.GetComponent<TubeGameplay>().PressAttached += SubscribeToPress;
        Debug.Log("Aeropress attached");
        kettleSocket.gameObject.SetActive(true);
    }

    public void AeropressRemoved()
    {
        _hasAeropress = false;
        _aeropress = null;
        kettleSocket.gameObject.SetActive(false);
        Debug.Log("Aeropress removed");
    }
    
    public void KettlePlaced()
    {
        var kettle = kettleSocket.GetOldestInteractableSelected();
        _kettle = kettle.transform.gameObject;
        _kettle.transform.GetComponent<Animator>().Play("Kettle");
        _hasWater = true;
        _aeropress.GetComponent<TubeGameplay>().ActivatePressSocket();
    }

    private void SubscribeToPress(PressGameplay pressGameplay)
    {
        pressGameplay.EnablePressing();
        pressGameplay.Pressing += HandlePressedCoffee;
    }

    private void HandlePressedCoffee()
    {
        _kettle.GetComponentInChildren<LiquidBehaviour>().PressLiquid(3.5f);
        liquid.PourLiquid();
    }
}
