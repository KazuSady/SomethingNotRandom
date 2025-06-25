using System;
using DG.Tweening;
using UnityEngine;


public class ButtonTrigger : MonoBehaviour
{
    [SerializeField] private string validTag = "Player";
    [SerializeField] private GameObject button;
    [SerializeField] private float pressedPosition = -0.16f;
    [SerializeField] private float unpressedPosition = -0.176f;
    
    public event Action OnButtonPressed;
    public event Action OnButtonReleased;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(validTag))
        {
            button.transform.DOLocalMoveX(pressedPosition, 0.5f);
            OnButtonPressed?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(validTag))
        {
            button.transform.DOLocalMoveX(unpressedPosition, 0.5f);
            OnButtonReleased?.Invoke();
        }
    }
}
