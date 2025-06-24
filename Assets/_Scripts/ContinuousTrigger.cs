using System;
using UnityEngine;

public class ContinuousTrigger : MonoBehaviour
{
    [SerializeField] private string validTag = "Player";

    [Range(0f, 1f)]
    public float PressProgress { get; private set; }

    [SerializeField] private float progressSpeed = 1.0f;

    public event Action OnFullyPressed;

    private bool _isTouching;
    private bool _pressedFired;

    public bool IsTouching => _isTouching;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(validTag))
        {
            _isTouching = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(validTag))
        {
            _isTouching = false;
        }
    }

    private void Update()
    {
        float direction = _isTouching ? 1f : -1f;
        PressProgress = Mathf.Clamp01(PressProgress + direction * progressSpeed * Time.deltaTime);

        if (PressProgress >= 1f && !_pressedFired)
        {
            OnFullyPressed?.Invoke();
            _pressedFired = true;
        }
        else if (PressProgress < 1f)
        {
            _pressedFired = false;
        }
    }
}
