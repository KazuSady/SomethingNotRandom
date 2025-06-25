using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class LiquidBehaviour : MonoBehaviour
{
    private static readonly int WobbleX = Shader.PropertyToID("_WobbleX");
    private static readonly int WobbleZ = Shader.PropertyToID("_WobbleZ");
    private static readonly int Cutoff = Shader.PropertyToID("_Cutoff");
    public event Action NoLiquidLeft;
    public event Action NewLiquid;
    public event Action<LiquidBehaviour, float, float> PressedAll;
    public event Action CoffeeIn;
    
    public bool CanAddLiquid;
    public bool CanAddCoffee;

    [Header("Foam")]
    [SerializeField] private GameObject foamObject;
    
    [Header("UI Elements")]
    [SerializeField] private TMP_Text liquidText;
    [SerializeField] private TMP_Text coffeeText;
    [SerializeField] private TMP_Text milkText;
    [SerializeField] private TMP_Text cremeText;
    
    [Header("Materials")]
    [SerializeField] private Material coffeeMaterial;
    [SerializeField] private Material milkMaterial;
    
    [Header("Cutoff")]
    [SerializeField] private GameObject liquidObject;
    [SerializeField] private Renderer liquidRenderer;
    [SerializeField] private float minCutoff;
    [SerializeField] private float duration = 3.5f;

    [Header("For liquid spilling")] 
    [SerializeField] private BoxCollider liquidCollider;
    [SerializeField] private LiquidDroplet dropletPrefab;
    [SerializeField] private List<LiquidSpillChecker> liquidSpillCheckers = new();

    [Header("For liquid receiving")] 
    [SerializeField] private bool noMilk;
    [SerializeField] private LiquidReceiver liquidReceiver;
    
    [Header("For liquid movement")]
    [SerializeField] private float maxWobble = 0.03f;
    [SerializeField] private float wobbleSpeed = 1.0f;
    [SerializeField] private float recovery = 1.0f;

    private float _liquidAmount;
    private float _milkAmount;
    private float _coffeeAmount;
    private float _foamAmount;
    private float _time = 0.5f;
    private Vector3 _lastPos;
    private Vector3 _lastRot;
    private Vector3 _velocity;
    private Vector3 _angularVelocity;
    private float _wobbleAmountX;
    private float _wobbleAmountZ;
    private float _wobbleAmountToAddX;
    private float _wobbleAmountToAddZ;
    private float _pulse;
    private Coroutine _disappearRoutine;
    
    public float LiquidAmount => _liquidAmount;
    public float MilkAmount => _milkAmount;
    public float CoffeeAmount => _coffeeAmount;
    public float FoamAmount => _foamAmount;

    private void Awake()
    {
        foreach (var spillChecker in liquidSpillCheckers)
        {
            spillChecker.OnDropSpill += HandleLiquidSpill;
        }
        liquidReceiver.OnDropletReceived += HandleNewDroplet;
    }

    private void Update()
    {
        if (liquidCollider)
        {
            liquidCollider.transform.rotation = Quaternion.identity;
        }
        if (liquidObject.activeSelf && _liquidAmount > 0.0f)
        {
            _time += Time.deltaTime;
            
            _wobbleAmountToAddX = Mathf.Lerp(_wobbleAmountToAddX, 0, Time.deltaTime * (recovery));
            _wobbleAmountToAddZ = Mathf.Lerp(_wobbleAmountToAddZ, 0, Time.deltaTime * (recovery));
            
            _pulse = 2 * Mathf.PI * wobbleSpeed;
            _wobbleAmountX = _wobbleAmountToAddX * Mathf.Sin(_pulse * _time);
            _wobbleAmountZ = _wobbleAmountToAddZ * Mathf.Sin(_pulse * _time);
            
            liquidRenderer.material.SetFloat(WobbleX, _wobbleAmountX);
            liquidRenderer.material.SetFloat(WobbleZ, _wobbleAmountZ);
            
            _velocity = (_lastPos - transform.position) / Time.deltaTime;
            _angularVelocity = transform.rotation.eulerAngles - _lastRot;
            
            _wobbleAmountToAddX += Mathf.Clamp((_velocity.x + (_angularVelocity.z * 0.2f)) * maxWobble, -maxWobble, maxWobble);
            _wobbleAmountToAddZ += Mathf.Clamp((_velocity.z + (_angularVelocity.x * 0.2f)) * maxWobble, -maxWobble, maxWobble);
            
            _lastPos = transform.position;
            _lastRot = transform.rotation.eulerAngles;
        }
    }

    private void OnDestroy()
    {
        foreach (var spillChecker in liquidSpillCheckers)
        {
            spillChecker.OnDropSpill -= HandleLiquidSpill;
        }
        liquidReceiver.OnDropletReceived -= HandleNewDroplet;
    }

    public void Reset()
    {
        _liquidAmount = 0;
        _milkAmount = 0;
        _coffeeAmount = 0;
        foamObject.SetActive(false);
        HandleChangeInAmount();
        liquidObject.SetActive(false);
        CanAddLiquid = false;
    }
    
    public void PourLiquid(float amountToSimulate, float coffeeAmount)
    {
        if (liquidRenderer.material == milkMaterial)
        {
            liquidRenderer.material = coffeeMaterial;
        }
        liquidObject.SetActive(true);
        StartCoroutine(SimulateLiquid(amountToSimulate, coffeeAmount));
        CanAddLiquid = true;
    }

    public void FrothMilk(float amount)
    {
        if (_milkAmount < amount)
        {
            amount = _milkAmount;
        }

        if (amount <= 0.0f)
        {
            return;
        }

        if (foamObject.activeSelf == false)
        {
            foamObject.SetActive(true);
            foamObject.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);;
        }

        _foamAmount += amount;
        _milkAmount -= amount;

        var scale = Mathf.Clamp(_foamAmount / 250.0f, 0.0f, 1.0f);
        foamObject.transform.localScale = new Vector3(1.0f, scale * 0.57f, 1.0f);

        milkText.text = $"Milk: {_milkAmount} ml";
        cremeText.text = $"Foam: {FoamAmount} ml";
    }

    private void HandleNewDroplet(LiquidDroplet droplet)
    {
        if (noMilk && droplet.LiquidType == DropletType.Milk)
        {
            return;
        }
        if (droplet.LiquidType == DropletType.CoffeeBlend)
        {
            if (_coffeeAmount == 0.0f && _coffeeAmount + droplet.AmountOfLiquid > 0.0f)
            {
                CoffeeIn?.Invoke();
            }
            _coffeeAmount += droplet.AmountOfLiquid;
            coffeeText.text = $"Coffee: {_coffeeAmount.ToString(CultureInfo.InvariantCulture)} g";
        }
        else
        {
            if (_liquidAmount == 0.0f && _liquidAmount + droplet.AmountOfLiquid > 0.0f)
            {
                NewLiquid?.Invoke();
                liquidObject.SetActive(true);
                CanAddLiquid = true;
            }

            if (_liquidAmount >= 210.0f || _liquidAmount + _milkAmount >= 210.0f)
            {
                return;
            }
            switch (droplet.LiquidType)
            {
                case DropletType.Water:
                    _liquidAmount += droplet.AmountOfLiquid;
                    break;
                case DropletType.Coffee:
                    break;
                case DropletType.Milk:
                    _liquidAmount += droplet.AmountOfLiquid;
                    _milkAmount += droplet.AmountOfLiquid;
                    milkText.text = $"Milk: {_milkAmount.ToString(CultureInfo.InvariantCulture)} ml";
                    liquidRenderer.material = milkMaterial;
                    break;
            }
            liquidText.text = $"Liquid: {_liquidAmount.ToString(CultureInfo.InvariantCulture)} ml";
            HandleChangeInAmount();
        }
        Destroy(droplet.gameObject);
    }

    private void HandleLiquidSpill(Transform liquidDropStart)
    {
        if (_liquidAmount > 0.0f)
        {
            var drop = Instantiate(dropletPrefab, liquidDropStart.position, Quaternion.identity);
            drop.GetComponent<Rigidbody>().AddForce(Physics.gravity, ForceMode.Acceleration);
            _liquidAmount -= drop.AmountOfLiquid;
            liquidText.text = $"Liquid: {_liquidAmount.ToString(CultureInfo.InvariantCulture)} ml";
            HandleChangeInAmount();
            if (_liquidAmount < 0.01f)
            {
                liquidRenderer.material.SetFloat(Cutoff, minCutoff);
                _liquidAmount = 0.0f;
                _coffeeAmount = 0.0f;
                NoLiquidLeft?.Invoke();
                liquidText.text = "";
                coffeeText.text = "";
                if (noMilk == false)
                {
                    _milkAmount = 0.0f;
                    milkText.text = "";
                }
                liquidObject.SetActive(false);
            }
        }
    }

    private void HandleChangeInAmount()
    {
        var proportion = _liquidAmount / 200.0f;
        liquidRenderer.material.SetFloat(Cutoff, proportion);
        var colliderSize = liquidCollider.size;
        var colliderCenter = liquidCollider.center;
        colliderCenter.y = -1.0f + proportion;
        liquidCollider.center = colliderCenter;
        colliderSize.y = Mathf.Max(colliderSize.y * proportion, 0.02f);
        liquidCollider.size = colliderSize;
    }
    private IEnumerator SimulateLiquid(float amountToSimulate, float coffeeAmount)
    {
        var top = amountToSimulate / 210.0f;
        var time = 0.0f;
        while (time < duration)
        {
            var t = time / duration;
            var cutoff = Mathf.Lerp(minCutoff, top, t);
            liquidRenderer.material.SetFloat(Cutoff, cutoff);
            time += Time.deltaTime;
            yield return null;
        }
        var colliderSize = liquidCollider.size;
        var colliderCenter = liquidCollider.center;
        colliderCenter.y = -1.0f + top;
        liquidCollider.center = colliderCenter;
        colliderSize.y = Mathf.Max(colliderSize.y * top, 0.02f);
        liquidCollider.size = colliderSize;

        _liquidAmount = amountToSimulate;
        _coffeeAmount = coffeeAmount;
        liquidText.text = $"Liquid: {_liquidAmount.ToString(CultureInfo.InvariantCulture)} ml";
        coffeeText.text = $"Coffee: {_coffeeAmount.ToString(CultureInfo.InvariantCulture)} g";
    }
    
    public void StartDisappearingLiquid(float disappearingDuration)
    {
        if (_disappearRoutine != null)
            StopCoroutine(_disappearRoutine);
    
        _disappearRoutine = StartCoroutine(SimulateDisappearingLiquid(disappearingDuration));
    }
    
    public void StopDisappearingLiquid()
    {
        if (_disappearRoutine != null)
        {
            StopCoroutine(_disappearRoutine);
            _disappearRoutine = null;
        }
    }

    private IEnumerator SimulateDisappearingLiquid(float disappearingDuration)
    {
        var totalSteps = Mathf.CeilToInt(disappearingDuration / 0.28f);
        var currentStep = 0;

        var start = _liquidAmount / 210.0f;
        var cutoff = start;

        while (cutoff > minCutoff)
        {
            var t = (float)currentStep / totalSteps;
            cutoff = Mathf.Lerp(start, minCutoff, t);
            liquidRenderer.material.SetFloat(Cutoff, cutoff);

            currentStep++;
            yield return new WaitForSeconds(0.28f);
        }

        liquidRenderer.material.SetFloat(Cutoff, minCutoff);
        liquidObject.SetActive(false);
        
        PressedAll?.Invoke(this, _liquidAmount, _coffeeAmount);

        _liquidAmount = 0.0f;
        liquidText.text = "";
        _coffeeAmount = 0.0f;
        coffeeText.text = "";

        _disappearRoutine = null;
    }
    
}
