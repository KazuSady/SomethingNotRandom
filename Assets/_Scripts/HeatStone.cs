using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HeaterPlate : MonoBehaviour
{
    private static readonly int ShouldEmissive = Shader.PropertyToID("_ShouldEmissive");
    
    [FormerlySerializedAs("_heatRate")] [SerializeField] private float heatRate = 10.0f;
    [FormerlySerializedAs("_socket")] [SerializeField] private XRSocketInteractor socket;
    [SerializeField] private Material material;

    private MugGameplay _currentMug;

    private void Awake()
    {
        socket.selectEntered.AddListener(OnMugPlaced);
        socket.selectExited.AddListener(OnMugRemoved);
    }

    private void OnDestroy()
    {
        socket.selectEntered.RemoveListener(OnMugPlaced);
        socket.selectExited.RemoveListener(OnMugRemoved);
    }

    private void Update()
    {
        if (_currentMug)
        {
            _currentMug.HeatUp(heatRate);
        }
    }

    private void OnMugPlaced(SelectEnterEventArgs args)
    {
        _currentMug = args.interactableObject.transform.GetComponent<MugGameplay>();
        material.SetFloat(ShouldEmissive, 1.0f);
    }

    private void OnMugRemoved(SelectExitEventArgs args)
    {
        _currentMug = null;
        material.SetFloat(ShouldEmissive, 0.0f);
    }
}
