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
    

    public void StrainerPlaced()
    {
        Debug.Log("Strainer attached");
        coffeSocket.gameObject.SetActive(true);
    }

    public void StrainerRemoved()
    {
        coffeSocket.gameObject.SetActive(false);
        Debug.Log("Strainer removed");
    }
    
    public void PressPlaced()
    {
        var press = pressSocket.GetOldestInteractableSelected();
        PressAttached?.Invoke(press.transform.GetComponent<PressGameplay>());
        Debug.Log("Press attached");
    }

    public void PressRemoved()
    {
        Debug.Log("Press removed");
    }

    public void CoffePlaced()
    {
        var coffee = coffeSocket.GetOldestInteractableSelected();
        coffee.transform.GetComponent<Animator>().Play("Coffee");
    }

    public void ActivatePressSocket()
    {
        pressSocket.gameObject.SetActive(true);
    }
}
