using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MugGameplay : MonoBehaviour
{
    public event Action<bool> TubeAttached;
    public event Action<bool> HotAchieved;

    [SerializeField] private BoxCollider mainCollider;
    [SerializeField] private CupType cupType;
    [SerializeField] private LiquidBehaviour liquid;
    [SerializeField] private float maxTemperature = 100.0f;
    
    [Header("Sockets")]
    [SerializeField] private XRSocketInteractor aeropressSocket;
    [SerializeField] private XRSocketInteractor decorSocket;
    
    [Header("Temperature effects")] 
    [SerializeField] private ParticleSystem midEffect;
    [SerializeField] private ParticleSystem hotEffect;

    private float _midTemp = 50.0f;
    private float _maxTemp = 90.0f;
    private GameObject _aeropress;
    private DecorationGameplay _decorationGameplay;
    private ContinuousPressGameplay _pressGameplay;
    private bool _midEffectOn = false;
    private bool _hotEffectOn = false;

    private float _temperature = 20.0f;

    public CupType CupType => cupType;

    private XRGrabInteractable _grabInteractable;

    private void Awake()
    {
        liquid.NoLiquidLeft += ResetProgress;

        _grabInteractable = GetComponent<XRGrabInteractable>();
        _grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDestroy()
    {
        _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        MugCupboard.Instance.OnMugGrabbed(this);
    }

    public void DecorPlaced()
    {
        var decor = decorSocket.GetOldestInteractableSelected();
        _decorationGameplay = decor.transform.GetComponent<DecorationGameplay>();
        Physics.IgnoreCollision(mainCollider, _decorationGameplay.GetComponentInChildren<BoxCollider>(), true);
    }
    
    public void DecorRemoved()
    {
        Physics.IgnoreCollision(mainCollider, _decorationGameplay.GetComponentInChildren<BoxCollider>(), false);
        _decorationGameplay = null;
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
        if (!(liquid.LiquidAmount > 0.0f))
        {
            return;
        }
        _temperature += temperatureIncreasePerSecond * Time.deltaTime;
        _temperature = Mathf.Min(_temperature, maxTemperature);
        if (_temperature >= _midTemp && _temperature < _maxTemp && _midEffectOn == false)
        {
            _midEffectOn = true;
            midEffect.Play();
            HotAchieved?.Invoke(false);
        }
        else if (_hotEffectOn && _temperature >= _maxTemp)
        {
            _hotEffectOn = true;
            midEffect.Stop();
            hotEffect.Play();
            HotAchieved?.Invoke(true);
        }
    }
    
    public void Froth(float amount)
    {
        liquid.FrothMilk(amount);
    }

    public float GetCoffeeAmount()
    {
        return liquid.CoffeeAmount;
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
        return liquid.FoamAmount;
    }

    public float GetTemperature()
    {
        return _temperature;
    }

    public DecorationType DecorType()
    {
        return _decorationGameplay == null ? DecorationType.None : _decorationGameplay.DecorationType;
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

    private void SubscribeToPress(ContinuousPressGameplay pressGameplay)
    {
        _pressGameplay = pressGameplay;
        _pressGameplay.EnablePressing();
        _pressGameplay.Pressing += HandlePressedCoffee;
        _pressGameplay.StoppedPressing += StoppedPressed;
    }

    private void HandlePressedCoffee()
    {
        var liquidPress = _aeropress.GetComponentInChildren<LiquidBehaviour>();
        liquidPress.PressedAll += FinishPressing;
        liquidPress.StartDisappearingLiquid(3.5f);
    }

    private void FinishPressing(LiquidBehaviour liquidbeh, float liquidAmount, float coffee)
    {
        liquid.PourLiquid(liquidAmount, coffee);
        liquidbeh.PressedAll -= FinishPressing;
    }
    
    private void StoppedPressed()
    {
        var liquidPress = _aeropress.GetComponentInChildren<LiquidBehaviour>();
        liquidPress.StopDisappearingLiquid();
    }
    
    public void DisableTubeInteract()
    {
        var tube = _aeropress.GetComponent<TubeGameplay>();
        if (tube == null)
        {
            return;
        }

        if (tube.TryGetComponent<XRGrabInteractable>(out var grab))
        {
            grab.interactionLayers &= ~InteractionLayerMask.GetMask("Default");
        }
    }

    public void EnableTubeInteract()
    {
        var tube = _aeropress.GetComponent<TubeGameplay>();
        if (tube == null)
        {
            return;
        }
        
        if (tube.TryGetComponent<XRGrabInteractable>(out var grab))
        {
            grab.interactionLayers |= InteractionLayerMask.GetMask("Default");
        }
    }
}
