
using System;
using UnityEngine;

public class ContinuousTrigger : MonoBehaviour
{
    public event Action Stopped;
    [SerializeField] private string validTag = "Player";

    [Range(0f, 1f)]
    public float PressProgress { get; private set; }

    [SerializeField] private float progressSpeed = 0.28f;

    public bool IsTouching;
    private bool _pressedFired;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(validTag))
        {
            IsTouching = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(validTag))
        {
            if (IsTouching)
            {
                Stopped?.Invoke();
            }
            IsTouching = false;
        }
    }

    private void Update()
    {
        if (Mathf.Approximately(PressProgress, 1.0f) && _pressedFired)
        {
            PressProgress = 0.0f;
            return;
        }
        float direction = IsTouching ? 1f : -1f;
        PressProgress = Mathf.Clamp01(PressProgress + direction * progressSpeed * Time.deltaTime);

        if (PressProgress >= 1f && !_pressedFired)
        {
            _pressedFired = true;
        }
        else if (PressProgress < 1f)
        {
            _pressedFired = false;
        }
    }
}
