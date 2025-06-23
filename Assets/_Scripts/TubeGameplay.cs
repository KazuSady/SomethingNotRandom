using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TubeGameplay : MonoBehaviour
{
    public event Action<PressGameplay> PressAttached;
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
        if (_strainer.transform.gameObject.GetComponent<StrainerGameplay>().HasFilter)
        {
            liquid.CanAddCoffee = true;
        }
        _strainer.GetComponent<StrainerGameplay>().DisableFilterInteract();
        StrainerAttached?.Invoke(true);
    }

    public void StrainerRemoved()
    {
        liquid.CanAddCoffee = false;
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
        PressAttached?.Invoke(press.transform.GetComponent<PressGameplay>());
        liquid.CanAddLiquid = false;
        AeropressAttached?.Invoke(true);
    }

    public void PressRemoved()
    {
        Physics.IgnoreCollision(mainCollider, _press.GetComponentInChildren<BoxCollider>(), false);
        _press = null;
        liquid.CanAddLiquid = true;
        AeropressAttached?.Invoke(false);
    }
    
    public void DisableStrainerInteract()
    {
        if (_strainer == null)
        {
            return;
        }

        var grab = _strainer.GetComponent<XRGrabInteractable>();
        if (grab)
        {
            grab.enabled = false;
        }

        Collider strainerCollider = _strainer.GetComponentInChildren<Collider>();
        if (strainerCollider)
        {
            Physics.IgnoreCollision(mainCollider, strainerCollider, true);
        }
    }

    public void EnableStrainerInteract()
    {
        if (_strainer == null)
        {
            return;
        }
        
        var grab = _strainer.GetComponent<XRGrabInteractable>();
        if (grab)
        {
            grab.enabled = true;
        }

        Collider strainerCollider = _strainer.GetComponentInChildren<Collider>();
        if (strainerCollider)
        {
            Physics.IgnoreCollision(mainCollider, strainerCollider, false);
        }
    }
}
