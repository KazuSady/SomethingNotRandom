using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MugGameplay : MonoBehaviour
{
    public event Action<MugGameplay> CoffeeStateUpdated;
    public event Action<bool> TubeAttached;

    [SerializeField] private BoxCollider mainCollider;
    [SerializeField] private CupType cupType;
    [SerializeField] private LiquidBehaviour liquid;

    [Header("Aeropress")]
    [SerializeField] private XRSocketInteractor aeropressSocket;
    

    private GameObject _aeropress;
    private PressGameplay _pressGameplay;
    
    public CupType CupType => cupType;

    private void Awake()
    {
        liquid.NoLiquidLeft += ResetProgress;
    }

    public void AeropressPlaced()
    {
        var aeropress = aeropressSocket.GetOldestInteractableSelected();
        _aeropress = aeropress.transform.gameObject;
        _aeropress.GetComponent<TubeGameplay>().PressAttached += SubscribeToPress;
        Physics.IgnoreCollision(mainCollider, _aeropress.GetComponentInChildren<BoxCollider>(), true);
        TubeAttached?.Invoke(true);
    }

    public void AeropressRemoved()
    {
        _aeropress.GetComponent<TubeGameplay>().PressAttached -= SubscribeToPress;
        Physics.IgnoreCollision(mainCollider, _aeropress.GetComponentInChildren<BoxCollider>(), false);
        _aeropress = null;
        if (liquid.LiquidAmount < 0.5f)
        {
            TubeAttached?.Invoke(false);
        }
    }
    
    public float GetCoffeeAmount()
    {
        return liquid.CanAddLiquid ? 9.0f : 0.0f;
    }

    public float GetMilkAmount()
    {
        return liquid.MilkAmount;
    }

    public float GetWaterAmount()
    {
        return liquid.LiquidAmount - liquid.MilkAmount;
    }

    public float GetCremeAmount()
    {
        return 0f;
    }
    
    private void ResetProgress()
    {
        _pressGameplay.Pressing -= HandlePressedCoffee;
        liquid.NoLiquidLeft -= ResetProgress;
        _aeropress = null;
        _pressGameplay = null;
        aeropressSocket.gameObject.SetActive(true);
        liquid.Reset();
    }
    private void SubscribeToPress(PressGameplay pressGameplay)
    {
        _pressGameplay = pressGameplay;
        _pressGameplay.EnablePressing();
        _pressGameplay.Pressing += HandlePressedCoffee;
    }

    private void HandlePressedCoffee()
    {
        var amount = _aeropress.GetComponentInChildren<LiquidBehaviour>().LiquidAmount;
        _aeropress.GetComponentInChildren<LiquidBehaviour>().PressLiquid(3.5f);
        liquid.PourLiquid(amount);
        CoffeeStateUpdated?.Invoke(this);
    }

    private void OnDestroy()
    {
        ResetProgress();
    }
}
