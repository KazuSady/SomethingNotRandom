using System;
using UnityEngine;


public class ButtonTrigger : MonoBehaviour
{
    [SerializeField] private string validTag = "Player";

    public event Action OnButtonPressed;
    public event Action OnButtonReleased;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(validTag))
        {
            OnButtonPressed?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(validTag))
        {
            OnButtonReleased?.Invoke();
        }
    }
}
