using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CoffeeMachineGameplay : MonoBehaviour
{
    public enum MachineState
    {
        Idle,
        PouringMilk,
        PouringCoffee
    }

    [SerializeField] private float pourInterval = 0.1f;
    
    [Header("Portafilter")]
    [SerializeField] private PortafilterGameplay portafilter;
    [SerializeField] private XRSocketInteractor portafilterSocket;

    [Header("Milk Nozzle")]
    [SerializeField] private LiquidProvider milkNozzle;

    [Header("Buttons")]
    [SerializeField] private ButtonTrigger milkButton;
    [SerializeField] private ButtonTrigger coffeeButton;

    private MachineState _currentState = MachineState.Idle;
    private Coroutine _pourRoutine;

    void Start()
    {
        if (milkButton)
        {
            milkButton.OnButtonPressed += OnMilkButtonPressed;
        }
        if (coffeeButton)
        {
            coffeeButton.OnButtonPressed += OnCoffeeButtonPressed;
        }
    }

    void OnDestroy()
    {
        if (milkButton)
        {
            milkButton.OnButtonPressed -= OnMilkButtonPressed;
        }
        if (coffeeButton)
        {
            coffeeButton.OnButtonPressed -= OnCoffeeButtonPressed;
        }
    }

    public void OnMilkButtonPressed()
    {
        if (_currentState == MachineState.Idle)
        {
            _currentState = MachineState.PouringMilk;
            _pourRoutine = StartCoroutine(PourMilkRoutine());
        }
        else if (_currentState == MachineState.PouringMilk)
        {
            StopPouring();
        }
    }

    public void OnCoffeeButtonPressed()
    {
        if (_currentState == MachineState.Idle && portafilterSocket.hasSelection && portafilter && portafilter.HasCoffee)
        {
            _currentState = MachineState.PouringCoffee;
            _pourRoutine = StartCoroutine(PourCoffeeRoutine());
        }
        else if (_currentState == MachineState.PouringCoffee)
        {
            StopPouring();
        }
    }

    public void StopPouring()
    {
        if (_pourRoutine != null)
        {
            StopCoroutine(_pourRoutine);
            _pourRoutine = null;
        }
        _currentState = MachineState.Idle;
    }

    private IEnumerator PourMilkRoutine()
    {
        while (true)
        {
            milkNozzle.ForceSpawnDroplet();
            yield return new WaitForSeconds(pourInterval);
        }
    }

    private IEnumerator PourCoffeeRoutine()
    {
        while (portafilter && portafilter.HasCoffee && portafilterSocket.hasSelection)
        {
            portafilter.TryPourCoffee();
            yield return new WaitForSeconds(pourInterval);
        }

        StopPouring();
    }
}
