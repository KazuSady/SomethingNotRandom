using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private static readonly int ShouldLight = Shader.PropertyToID("_ShouldLight");
    
    
    [Header("Aeropress parts")]
    [SerializeField] private StrainerGameplay strainerGameplay;
    [SerializeField] private PressGameplay pressGameplay;
    [SerializeField] private TubeGameplay tubeGameplay;
    [SerializeField] private Renderer filterObject;
    [SerializeField] private Renderer strainerRenderer;
    [SerializeField] private Renderer pressRenderer;
    [SerializeField] private Renderer tubeRenderer;
    
    [Header("Other objects")]
    [SerializeField] private Renderer milkObject;
    [SerializeField] private Renderer coffeeObject;
    [SerializeField] private Renderer kettleObject;

    private MugGameplay _mugGameplay;
    private Renderer _mugRenderer;

    private void Start()
    {
        strainerGameplay.FilterAttached += FilterStrainer;
        tubeGameplay.StrainerAttached += StrainerTube;
        tubeGameplay.AeropressAttached += TubePress;
        tubeGameplay.CoffeePresent += CoffeeBag;
        tubeGameplay.WaterPresent += Kettle;
        FilterStrainer(false);
    }

    private void OnDestroy()
    {
        strainerGameplay.FilterAttached -= FilterStrainer;
        tubeGameplay.StrainerAttached -= StrainerTube;
        tubeGameplay.AeropressAttached -= TubePress;
        _mugGameplay.TubeAttached -= MugTube;
    }

    public void SetMug(MugGameplay mug, Renderer mugRenderer)
    {
        _mugGameplay = mug;
        _mugRenderer = mugRenderer;
        _mugGameplay.TubeAttached += MugTube;
    }

    public void RemoveOldMug()
    {
        _mugGameplay.TubeAttached -= MugTube;
        _mugRenderer = null;
        _mugGameplay = null;
    }
    
    private void FilterStrainer(bool isFilterAttached)
    {
        if (isFilterAttached)
        {
            filterObject.material.SetFloat(ShouldLight, 0.0f);
            strainerRenderer.material.SetFloat(ShouldLight, 0.0f);
            StrainerTube(false);
        }
        else
        {
            filterObject.material.SetFloat(ShouldLight, 1.0f);
            strainerRenderer.material.SetFloat(ShouldLight, 1.0f);
        }
    }
    
    private void StrainerTube(bool isStrainerAttached)
    {
        if (isStrainerAttached)
        {
            tubeRenderer.material.SetFloat(ShouldLight, 0.0f);
            strainerRenderer.material.SetFloat(ShouldLight, 0.0f);
            MugTube(false);
        }
        else
        {
            tubeRenderer.material.SetFloat(ShouldLight, 1.0f);
            strainerRenderer.material.SetFloat(ShouldLight, 1.0f);
        }
    }

    private void MugTube(bool isTubeAttached)
    {
        if (isTubeAttached)
        {
            tubeRenderer.material.SetFloat(ShouldLight, 0.0f);
            _mugRenderer.material.SetFloat(ShouldLight, 0.0f);
            CoffeeBag(false);
        }
        else
        {
            tubeRenderer.material.SetFloat(ShouldLight, 1.0f);
            _mugRenderer.material.SetFloat(ShouldLight, 1.0f);
        }
    }

    private void CoffeeBag(bool isCoffeeIn)
    {
        if (isCoffeeIn)
        {
            coffeeObject.material.SetFloat(ShouldLight, 0.0f);
            Kettle(false);
        }
        else
        {
            coffeeObject.material.SetFloat(ShouldLight, 1.0f);
        }
    }

    private void Kettle(bool isWaterIn)
    {
        if (isWaterIn)
        {
            kettleObject.material.SetFloat(ShouldLight, 0.0f);
            TubePress(false);
        }
        else
        {
            kettleObject.material.SetFloat(ShouldLight, 1.0f);
        }
    }

    private void TubePress(bool isPressAttached)
    {
        if (isPressAttached)
        {
            pressRenderer.material.SetFloat(ShouldLight, 0.0f);
            tubeRenderer.material.SetFloat(ShouldLight, 0.0f);
        }
        else
        {
            pressRenderer.material.SetFloat(ShouldLight, 1.0f);
            tubeRenderer.material.SetFloat(ShouldLight, 1.0f);
        }
    }
}
