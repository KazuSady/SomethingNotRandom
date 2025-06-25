using System.Globalization;
using TMPro;
using UnityEngine;

public class PortafilterGameplay : MonoBehaviour
{
    [SerializeField] private float maxCoffee = 100f;
    [SerializeField] private float coffeePerDroplet = 1f;
    [SerializeField] private LiquidReceiver liquidReceiver;
    [SerializeField] private TMP_Text coffeeText;

    [Header("Droplets")]
    [SerializeField] private LiquidProvider leftNozzle;
    [SerializeField] private LiquidProvider rightNozzle;

    private float _currentCoffee;

    public bool HasCoffee => _currentCoffee > 0f;
    public float CoffeeLevel => _currentCoffee / maxCoffee;

    void Awake()
    {
        liquidReceiver.OnDropletReceived += RefillCoffee;
    }

    void OnDestroy()
    {
        liquidReceiver.OnDropletReceived -= RefillCoffee;
    }

    public void RefillCoffee(LiquidDroplet droplet)
    {
        if (droplet.LiquidType == DropletType.CoffeeBlend)
        {
            _currentCoffee = Mathf.Clamp(_currentCoffee + coffeePerDroplet, 0f, maxCoffee);
            coffeeText.text = $"Coffee: {_currentCoffee.ToString(CultureInfo.InvariantCulture)} g";
        }
    }
   
    public void TryPourCoffee()
    {
        if (HasCoffee)
        {
            leftNozzle.ForceSpawnDroplet();
            rightNozzle.ForceSpawnDroplet();
            _currentCoffee = Mathf.Max(0f, _currentCoffee - 2 * coffeePerDroplet);
            coffeeText.text = $"Coffee: {_currentCoffee.ToString(CultureInfo.InvariantCulture)} g";
        }
    }
}
