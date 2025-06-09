using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TubeGameplay : MonoBehaviour
{
    public event Action<PressGameplay> PressAttached;
    
    [Header("Strainer")]
    [SerializeField] private XRSocketInteractor strainerSocket;
    [Header("Press")]
    [SerializeField] private XRSocketInteractor pressSocket;
    [Header("Coffee")]
    [SerializeField] private XRSocketInteractor coffeSocket;
    [SerializeField] private LiquidBehaviour liquid;
    

    public void StrainerPlaced()
    {
        Debug.Log("Strainer attached");
        coffeSocket.gameObject.SetActive(true);
    }

    public void StrainerRemoved()
    {
        coffeSocket.gameObject.SetActive(false);
        liquid.Reset();
        Debug.Log("Strainer removed");
    }
    
    public void PressPlaced()
    {
        var press = pressSocket.GetOldestInteractableSelected();
        PressAttached?.Invoke(press.transform.GetComponent<PressGameplay>());
        liquid.CanAddLiquid = false;
        Debug.Log("Press attached");
    }

    public void PressRemoved()
    {
        Debug.Log("Press removed");
        liquid.CanAddLiquid = true;
    }

    public void CoffePlaced()
    {
        var coffee = coffeSocket.GetOldestInteractableSelected();
        coffee.transform.GetComponent<Animator>().Play("Coffee");
        liquid.CanAddLiquid = true;
    }
}
