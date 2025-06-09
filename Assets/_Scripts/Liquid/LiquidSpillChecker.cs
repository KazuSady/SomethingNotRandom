using System;
using System.Collections;
using UnityEngine;

public class LiquidSpillChecker : MonoBehaviour
{
    public event Action<Transform> OnDropSpill;

    [SerializeField] private BoxCollider liquidCollider;
    private Coroutine _liquidSpillingCoroutine;
    private float _timeBetweenDroplets = 1.0f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other == liquidCollider  && _liquidSpillingCoroutine == null)
        {
            _liquidSpillingCoroutine = StartCoroutine(EmitDrop());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_liquidSpillingCoroutine != null)
        {
            StopCoroutine(_liquidSpillingCoroutine);
            _liquidSpillingCoroutine = null;
        }
    }

    private IEnumerator EmitDrop()
    {
        while (true)
        {
            OnDropSpill?.Invoke(transform);
            yield return new WaitForSeconds(_timeBetweenDroplets);
        }
    }
}
