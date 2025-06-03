using System;
using System.Collections;
using UnityEngine;

public class LiquidSpillChecker : MonoBehaviour
{
    public event Action<Transform> OnDropSpill;

    private Coroutine _liquidSpillingCoroutine;
    private float _timeBetweenDroplets = 1.0f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LiquidCollider") && _liquidSpillingCoroutine == null)
        {
            _liquidSpillingCoroutine = StartCoroutine(EmitDrop());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_liquidSpillingCoroutine != null)
        {
            StopCoroutine(_liquidSpillingCoroutine);
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
