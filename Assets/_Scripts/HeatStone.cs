using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HeaterPlate : MonoBehaviour
{
    [SerializeField] private float _heatRate = 10.0f;
    [SerializeField] private XRSocketInteractor _socket;

    private MugGameplay _currentMug;

    private void OnEnable()
    {
        _socket.selectEntered.AddListener(OnMugPlaced);
        _socket.selectExited.AddListener(OnMugRemoved);
    }

    private void OnDisable()
    {
        _socket.selectEntered.RemoveListener(OnMugPlaced);
        _socket.selectExited.RemoveListener(OnMugRemoved);
    }

    private void Update()
    {
        if (_currentMug)
        {
            _currentMug.HeatUp(_heatRate);
        }
    }

    private void OnMugPlaced(SelectEnterEventArgs args)
    {
        _currentMug = args.interactableObject.transform.GetComponent<MugGameplay>();
    }

    private void OnMugRemoved(SelectExitEventArgs args)
    {
        _currentMug = null;
    }
}
