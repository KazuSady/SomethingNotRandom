using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LiquidBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject liquidObject;
    [SerializeField] private Renderer liquidRenderer;
    [SerializeField] private Transform newParent;
    [SerializeField] private Transform oldParent;
    [SerializeField] private float minCutoff = -0.5f;
    [SerializeField] private float topCutoff = 0.25f;
    [SerializeField] private float duration = 3.5f;

    [Header("For liquid spilling")] 
    [SerializeField] private BoxCollider liquidCollider;
    [SerializeField] private GameObject dropletPrefab;
    [SerializeField] private List<LiquidSpillChecker> liquidSpillCheckers = new();
    
    [Header("For liquid movement")]
    [SerializeField] private float maxWobble = 0.03f;
    [SerializeField] private float wobbleSpeed = 1.0f;
    [SerializeField] private float recovery = 1.0f;

    private float _liquidAmount = 0.0f;
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

    private void Awake()
    {
        foreach (var spillChecker in liquidSpillCheckers)
        {
            spillChecker.OnDropSpill += HandleLiquidSpill;
        }
    }

    private void Update()
    {
        if (liquidCollider)
        {
            liquidCollider.transform.rotation = Quaternion.identity;
        }
        if (liquidObject.activeSelf && _liquidAmount > 1.0f)
        {
            _time += Time.deltaTime;
            
            _wobbleAmountToAddX = Mathf.Lerp(_wobbleAmountToAddX, 0, Time.deltaTime * (recovery));
            _wobbleAmountToAddZ = Mathf.Lerp(_wobbleAmountToAddZ, 0, Time.deltaTime * (recovery));
            
            _pulse = 2 * Mathf.PI * wobbleSpeed;
            _wobbleAmountX = _wobbleAmountToAddX * Mathf.Sin(_pulse * _time);
            _wobbleAmountZ = _wobbleAmountToAddZ * Mathf.Sin(_pulse * _time);
            
            liquidRenderer.material.SetFloat("_WobbleX", _wobbleAmountX);
            liquidRenderer.material.SetFloat("_WobbleZ", _wobbleAmountZ);
            
            _velocity = (_lastPos - transform.position) / Time.deltaTime;
            _angularVelocity = transform.rotation.eulerAngles - _lastRot;
            
            _wobbleAmountToAddX += Mathf.Clamp((_velocity.x + (_angularVelocity.z * 0.2f)) * maxWobble, -maxWobble, maxWobble);
            _wobbleAmountToAddZ += Mathf.Clamp((_velocity.z + (_angularVelocity.x * 0.2f)) * maxWobble, -maxWobble, maxWobble);
            
            _lastPos = transform.position;
            _lastRot = transform.rotation.eulerAngles;
        }
    }

    public void PourLiquid()
    {
        if (liquidRenderer.material == GameManager.Instance.MilkCoffeeMaterial)
        {
            liquidRenderer.material = GameManager.Instance.BasicCoffeeMaterial;
        }
        liquidObject.SetActive(true);
        StartCoroutine(SimulateLiquid());
    }

    public void PressLiquid(float disappearingDuration)
    {
        StartCoroutine(SimulateDisappearingLiquid(disappearingDuration));
    }

    public void HandleMilk()
    {
        liquidObject.transform.DOScaleY(0.02f, 3.0f);
        liquidRenderer.material = GameManager.Instance.MilkCoffeeMaterial;
        liquidCollider.size = liquidObject.GetComponent<BoxCollider>().size;
    }

    private void HandleLiquidSpill(Transform liquidDropStart)
    {
        if (_liquidAmount > 0.0f)
        {
            var drop = Instantiate(dropletPrefab, liquidDropStart.position, Quaternion.identity);
            drop.GetComponentInChildren<Rigidbody>().AddForce(Physics.gravity, ForceMode.Acceleration);
            _liquidAmount -= 0.1f;
            var cutoffAmount = topCutoff * _liquidAmount ;
            liquidRenderer.material.SetFloat("_Cutoff", cutoffAmount);
            var colliderSize = liquidCollider.size;
            var colliderCenter = liquidCollider.center;
            colliderCenter.y = -1.0f + _liquidAmount;
            liquidCollider.center = colliderCenter;
            colliderSize.y *= _liquidAmount;
            liquidCollider.size = colliderSize;
            Debug.Log($"Liquid amount: {_liquidAmount}");
            Debug.Log($"Cut off: {cutoffAmount}");
        }
    }
    
    private IEnumerator SimulateLiquid()
    {
        var time = 0.0f;
        while (time < duration)
        {
            var t = time / duration;
            var cutoff = Mathf.Lerp(minCutoff, topCutoff, t);
            liquidRenderer.material.SetFloat("_Cutoff", cutoff);
            time += Time.deltaTime;
            yield return null;
        }

        if (newParent)
        {
            liquidObject.transform.SetParent(newParent);
        }

        _liquidAmount = 1.0f;
    }
    
    private IEnumerator SimulateDisappearingLiquid(float disappearingDuration)
    {
        var time = 0.0f;
        while (time < disappearingDuration)
        {
            var t = time / disappearingDuration;
            var cutoff = Mathf.Lerp(topCutoff, minCutoff, t);
            liquidRenderer.material.SetFloat("_Cutoff", cutoff);
            time += Time.deltaTime;
            yield return null;
        }
        liquidObject.SetActive(false);
        if (newParent)
        {
            liquidObject.transform.SetParent(oldParent);
        }

        _liquidAmount = 0.0f;
    }
}
