using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TubeGameplay : MonoBehaviour
{
    public event Action<ContinuousPressGameplay> PressAttached;
    public event Action<bool> StrainerAttached;
    public event Action<bool> AeropressAttached;
    public event Action<bool> CoffeePresent;
    public event Action<bool> WaterPresent;

    [SerializeField] private BoxCollider mainCollider;
    [Header("Strainer")]
    [SerializeField] private XRSocketInteractor strainerSocket;
    [Header("Press")]
    [SerializeField] private XRSocketInteractor pressSocket;
    [SerializeField] private LiquidBehaviour liquid;

    private GameObject _press;
    private GameObject _strainer;

    public GameObject Strainer => _strainer;

    private void Start()
    {
        liquid.NewLiquid += () => { WaterPresent?.Invoke(true); };
        liquid.CoffeeIn += () => { CoffeePresent?.Invoke(true); };
    }

    public void StrainerPlaced()
    {
        _strainer = strainerSocket.GetOldestInteractableSelected().transform.gameObject;
        Physics.IgnoreCollision(mainCollider, _strainer.GetComponentInChildren<BoxCollider>(), true);
        var sg = _strainer.transform.gameObject.GetComponent<StrainerGameplay>();
        sg.DisableFilterInteract();
        StrainerAttached?.Invoke(true);
    }

    public void StrainerRemoved()
    {
        Physics.IgnoreCollision(mainCollider, _strainer.GetComponentInChildren<BoxCollider>(), false);
        _strainer.GetComponent<StrainerGameplay>().EnableFilterInteract();
        _strainer = null;
        liquid.Reset();
        StrainerAttached?.Invoke(false);
    }

    public void PressPlaced()
    {
        var press = pressSocket.GetOldestInteractableSelected();
        _press = press.transform.gameObject;
        Physics.IgnoreCollision(mainCollider, _press.GetComponentInChildren<BoxCollider>(), true);
        PressAttached?.Invoke(press.transform.GetComponent<ContinuousPressGameplay>());
        AeropressAttached?.Invoke(true);
    }

    public void PressRemoved()
    {
        Physics.IgnoreCollision(mainCollider, _press.GetComponentInChildren<BoxCollider>(), false);
        _press = null;
        AeropressAttached?.Invoke(false);
    }
    
    public void DisableStrainerInteract()
    {
        if (_strainer == null)
        {
            return;
        }

        if (_strainer.TryGetComponent<XRGrabInteractable>(out var grab))
        {
            grab.interactionLayers &= ~InteractionLayerMask.GetMask("Default");
        }
    }

    public void EnableStrainerInteract()
    {
        if (_strainer == null)
        {
            return;
        }
        
        if (_strainer.TryGetComponent<XRGrabInteractable>(out var grab))
        {
            grab.interactionLayers |= InteractionLayerMask.GetMask("Default");
        }
    }
}
