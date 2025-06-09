using System;
using System.Collections;
using UnityEngine;

public class LiquidDroplet : MonoBehaviour
{
    [SerializeField] private DropletType type;
    [SerializeField] private float amountOfLiquid;
    
    public DropletType LiquidType => type;
    public float AmountOfLiquid => amountOfLiquid;

    private void Start()
    {
        StartCoroutine(HandleDestroyAfterDelay());
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("NoCollisionForDrop") == false)
        {
            Destroy(gameObject); 
        }
    }

    private IEnumerator HandleDestroyAfterDelay()
    {
        yield return new WaitForSeconds(10.0f);
        Destroy(gameObject);
    }
}

[Serializable]
public enum DropletType
{
    Water,
    Coffee,
    Milk
}