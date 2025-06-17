using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class LiquidSpillChecker : MonoBehaviour
{
    public event Action<Transform> OnDropSpill;

    [SerializeField] private BoxCollider liquidCollider;
    [SerializeField] private float timeBetweenDroplets = 0.1f;
    private Coroutine _liquidSpillingCoroutine;
    
    
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
            yield return new WaitForSeconds(timeBetweenDroplets);
        }
    }
}
