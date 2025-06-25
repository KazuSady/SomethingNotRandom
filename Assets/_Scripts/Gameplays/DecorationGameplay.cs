using System;
using UnityEngine;

public class DecorationGameplay : MonoBehaviour
{
    public event Action Grabbed;

    [SerializeField] private DecorationType decorationType;
    
    public DecorationType DecorationType => decorationType;
    
    public void OnGrab()
    {
        Grabbed?.Invoke();
    }
}

[Serializable]
public enum DecorationType
{
    None,
    Ear,
    Spider,
    Flower
}
