using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HeaterPlate : MonoBehaviour
{
    private static readonly int ShouldEmissive = Shader.PropertyToID("_ShouldEmissive");
    
    [SerializeField] private float heatRate = 5.0f;
    [SerializeField] private XRSocketInteractor socket;
    [SerializeField] private Material material;
    [Header("Temperature effects")] 
    [SerializeField] private ParticleSystem midEffect;
    [SerializeField] private ParticleSystem hotEffect;

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

    private void ChangeVisuals(bool shouldHot)
    {
        switch (shouldHot)
        {
            case true:
                midEffect.Stop();
                hotEffect.Play();
                break;
            case false:
                midEffect.Play();
                hotEffect.Stop();
                break;
        }
    }

    private void OnMugPlaced(SelectEnterEventArgs args)
    {
        _currentMug = args.interactableObject.transform.GetComponent<MugGameplay>();
        _currentMug.HotAchieved += ChangeVisuals;
        material.SetFloat(ShouldEmissive, 1.0f);
    }

    private void OnMugRemoved(SelectExitEventArgs args)
    {
        _currentMug.HotAchieved -= ChangeVisuals;
        _currentMug = null;
        material.SetFloat(ShouldEmissive, 0.0f);
    }
}
