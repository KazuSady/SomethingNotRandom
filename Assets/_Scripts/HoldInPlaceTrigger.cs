using System;
using System.Collections;
using UnityEngine;

public class HoldInPlaceTrigger : MonoBehaviour
{
    public event Action StartAction;
    [SerializeField] private float timeRequired;
    private Coroutine _countingCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _countingCoroutine = StartCoroutine(StartCounting());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopCoroutine(_countingCoroutine);
        }
    }

    private IEnumerator StartCounting()
    {
        yield return new WaitForSeconds(timeRequired);
        StartAction?.Invoke();
    }
}
