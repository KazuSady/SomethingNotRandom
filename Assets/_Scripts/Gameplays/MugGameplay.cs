using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MugGameplay : MonoBehaviour
{
    public event Action<MugGameplay> CoffeeStateUpdated;
    public event Action<bool> TubeAttached;

    [SerializeField] private BoxCollider mainCollider;
    [SerializeField] private CupType cupType;
    [SerializeField] private LiquidBehaviour liquid;
    [SerializeField] private float maxTemperature = 100.0f;

    [Header("Aeropress")]
    [SerializeField] private XRSocketInteractor aeropressSocket;


    private GameObject _aeropress;
    private PressGameplay _pressGameplay;

    private float _temperature = 20.0f;

    public CupType CupType => cupType;

    private XRGrabInteractable _grabInteractable;

    void Awake()
    {
        liquid.NoLiquidLeft += ResetProgress;

        _grabInteractable = GetComponent<XRGrabInteractable>();
        _grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    void OnDestroy()
    {
        _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        MugCupboard.Instance.OnMugGrabbed(this);
    }

    public void AeropressPlaced()
    {
        var aeropress = aeropressSocket.GetOldestInteractableSelected();
        _aeropress = aeropress.transform.gameObject;
        var tubeGameplay = _aeropress.GetComponent<TubeGameplay>();
        tubeGameplay.PressAttached += SubscribeToPress;
        tubeGameplay.DisableStrainerInteract();
        Physics.IgnoreCollision(mainCollider, _aeropress.GetComponentInChildren<BoxCollider>(), true);
        Physics.IgnoreCollision(mainCollider, tubeGameplay.Strainer.GetComponentInChildren<BoxCollider>(), true);
        var strainerGameplay = tubeGameplay.Strainer.GetComponentInChildren<StrainerGameplay>();
        Physics.IgnoreCollision(mainCollider, strainerGameplay.Filter.GetComponentInChildren<BoxCollider>(), true);
        TubeAttached?.Invoke(true);
    }

    public void AeropressRemoved()
    {
        var tubeGameplay = _aeropress.GetComponent<TubeGameplay>();
        tubeGameplay.PressAttached -= SubscribeToPress;
        tubeGameplay.EnableStrainerInteract();
        Physics.IgnoreCollision(mainCollider, _aeropress.GetComponentInChildren<BoxCollider>(), false);
        Physics.IgnoreCollision(mainCollider, tubeGameplay.Strainer.GetComponentInChildren<BoxCollider>(), false);
        var strainerGameplay = tubeGameplay.Strainer.GetComponentInChildren<StrainerGameplay>();
        Physics.IgnoreCollision(mainCollider, strainerGameplay.Filter.GetComponentInChildren<BoxCollider>(), false);
        _aeropress = null;
        if (liquid.LiquidAmount < 0.5f)
        {
            TubeAttached?.Invoke(false);
        }
    }

    public void HeatUp(float temperatureIncreasePerSecond)
    {
        _temperature += temperatureIncreasePerSecond * Time.deltaTime;
        _temperature = Mathf.Min(_temperature, maxTemperature);
        CoffeeStateUpdated?.Invoke(this);
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

    public float GetTemperature()
    {
        return _temperature;
    }

    private void ResetProgress()
    {
        if (_pressGameplay)
        {
            _pressGameplay.Pressing -= HandlePressedCoffee;
        }

        if (liquid)
        {
            liquid.NoLiquidLeft -= ResetProgress;
        }
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
        var liquidPress = _aeropress.GetComponentInChildren<LiquidBehaviour>();
        liquid.PourLiquid(liquidPress.LiquidAmount, liquidPress.CoffeeAmount);
        liquidPress.PressLiquid(3.5f);
        CoffeeStateUpdated?.Invoke(this);
    }
    
    public void DisableTubeInteract()
    {
        var _tube = _aeropress.GetComponent<TubeGameplay>();
        if (_tube == null)
        {
            return;
        }

        if (_tube.TryGetComponent<XRGrabInteractable>(out var grab))
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

        if (_tube.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void EnableTubeInteract()
    {
        var _tube = _aeropress.GetComponent<TubeGameplay>();
        if (_tube == null)
        {
            return;
        }
        
        if (_tube.TryGetComponent<XRGrabInteractable>(out var grab))
        {
            grab.interactionLayers |= InteractionLayerMask.GetMask("Default");
        }
    }
}
