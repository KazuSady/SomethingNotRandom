using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TubeGameplay : MonoBehaviour
{
    public event Action<PressGameplay> PressAttached;
    
    [SerializeField] private BoxCollider mainCollider;
    [Header("Strainer")]
    [SerializeField] private XRSocketInteractor strainerSocket;
    [Header("Press")]
    [SerializeField] private XRSocketInteractor pressSocket;
    [Header("Coffee")]
    [SerializeField] private XRSocketInteractor coffeSocket;
    [SerializeField] private LiquidBehaviour liquid;

    private GameObject _press;
    private GameObject _strainer;
    

    public void StrainerPlaced()
    {
        _strainer = strainerSocket.GetOldestInteractableSelected().transform.gameObject;
        Physics.IgnoreCollision(mainCollider, _strainer.GetComponentInChildren<BoxCollider>(), true);
        if (_strainer.transform.gameObject.GetComponent<StrainerGameplay>().HasFilter)
        {
            coffeSocket.gameObject.SetActive(true);
        }
    }

    public void StrainerRemoved()
    {
        coffeSocket.gameObject.SetActive(false);
        Physics.IgnoreCollision(mainCollider, _strainer.GetComponentInChildren<BoxCollider>(), false);
        _strainer = null;
        liquid.Reset();
    }
    
    public void PressPlaced()
    {
        var press = pressSocket.GetOldestInteractableSelected();
        _press = press.transform.gameObject;
        Physics.IgnoreCollision(mainCollider, _press.GetComponentInChildren<BoxCollider>(), true);
        PressAttached?.Invoke(press.transform.GetComponent<PressGameplay>());
        liquid.CanAddLiquid = false;
    }

    public void PressRemoved()
    {
        Physics.IgnoreCollision(mainCollider, _press.GetComponentInChildren<BoxCollider>(), false);
        _press = null;
        liquid.CanAddLiquid = true;
    }

    public void CoffePlaced()
    {
        var coffee = coffeSocket.GetOldestInteractableSelected();
        coffee.transform.GetComponent<Animator>().Play("Coffee");
        liquid.CanAddLiquid = true;
    }
}
