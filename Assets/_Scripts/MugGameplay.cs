using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MugGameplay : MonoBehaviour
{
    [SerializeField] private GameObject liquidHolder;
    [Header("Aeropress")]
    [SerializeField] private XRSocketInteractor aeropressSocket;
    [Header("Kettle")]
    [SerializeField] private XRSocketInteractor kettleSocket;
    
    private bool _hasAeropress;
    private bool _hasWater;
    private GameObject _aeropress;
    
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
        
        kettleSocket.gameObject.SetActive(false);
        Debug.Log("Aeropress removed");
    }
    
    public void KettlePlaced()
    {
        var kettle = kettleSocket.GetOldestInteractableSelected();
        kettle.transform.GetComponent<Animator>().Play("Kettle");
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
        StartCoroutine(PressCoffee());
    }

    private IEnumerator PressCoffee()
    {
        var coffeWater = liquidHolder.transform.GetChild(0);
        var tween = coffeWater.transform.DOScaleY(0.0f, 3.5f);
        yield return tween.WaitForCompletion();
    }
}
