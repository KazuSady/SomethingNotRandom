using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MugGameplay : MonoBehaviour
{
    public event Action<MugGameplay> CoffeeStateUpdated;
    [SerializeField] private CupType cupType;
    [SerializeField] private LiquidBehaviour liquid;
    [Header("Aeropress")]
    [SerializeField] private XRSocketInteractor aeropressSocket;
    [Header("Kettle")]
    [SerializeField] private XRSocketInteractor kettleSocket;
    [Header("Milk")]
    [SerializeField] private XRSocketInteractor milkSocket;
    
    private bool _hasAeropress;
    private bool _hasWater;
    private bool _hasMilk;
    private GameObject _aeropress;
    private GameObject _kettle;
    private GameObject _milk;
    private PressGameplay _pressGameplay;
    
    public CupType CupType => cupType;
    public bool HasMilk => _hasMilk;
    
    public void AeropressPlaced()
    {
        milkSocket.gameObject.SetActive(false);
        _hasAeropress = true;
        var aeropress = aeropressSocket.GetOldestInteractableSelected();
        _aeropress = aeropress.transform.gameObject;
        _aeropress.GetComponent<TubeGameplay>().PressAttached += SubscribeToPress;
        Debug.Log("Aeropress attached");
        kettleSocket.gameObject.SetActive(true);
    }

    public void AeropressRemoved()
    {
        _aeropress.GetComponent<TubeGameplay>().PressAttached -= SubscribeToPress;
        _hasAeropress = false;
        _aeropress = null;
        if (_hasWater)
        {
            milkSocket.gameObject.SetActive(true);
        }
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
    
    public void KettleRemoved()
    {
        if (_hasWater)
        {
            kettleSocket.gameObject.SetActive(false);
        }
        Debug.Log("Kettle removed");
    }
    
    public void MilkPlaced()
    {
        var milk = milkSocket.GetOldestInteractableSelected();
        _milk = milk.transform.gameObject;
        _milk.transform.GetComponent<Animator>().Play("MilkCarton");
        _hasMilk = true;
        liquid.HandleMilk();
        CoffeeStateUpdated?.Invoke(this);
    }
    
    public void MilkRemoved()
    {
        _milk = null;
        if (_hasMilk)
        {
            milkSocket.gameObject.SetActive(false);
        }
        Debug.Log("Milk removed");
    }

    private void SubscribeToPress(PressGameplay pressGameplay)
    {
        _pressGameplay = pressGameplay;
        _pressGameplay.EnablePressing();
        _pressGameplay.Pressing += HandlePressedCoffee;
    }

    private void HandlePressedCoffee()
    {
        _kettle.GetComponentInChildren<LiquidBehaviour>().PressLiquid(3.5f);
        liquid.PourLiquid();
        CoffeeStateUpdated?.Invoke(this);
    }

    private void OnDestroy()
    {
        _pressGameplay.Pressing += HandlePressedCoffee;
    }
}
