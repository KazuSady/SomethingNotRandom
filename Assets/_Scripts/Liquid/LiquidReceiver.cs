using System;
using UnityEngine;

public class LiquidReceiver : MonoBehaviour
{
    public event Action<LiquidDroplet> OnDropletReceived;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<LiquidDroplet>() is var droplet && droplet != null)
        {
            OnDropletReceived?.Invoke(droplet);
        }
    }
}
