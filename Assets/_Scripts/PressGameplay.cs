using System;
using UnityEngine;

public class PressGameplay : MonoBehaviour
{
    public event Action Pressing;
    [SerializeField] private HoldInPlaceTrigger holdInPlaceTrigger;
    [SerializeField] private Animator pressAnimator;

    private void Awake()
    {
        holdInPlaceTrigger.StartAction += Press;
    }

    public void EnablePressing()
    {
        holdInPlaceTrigger.gameObject.SetActive(true);
    }

    public void DisablePressing()
    {
        holdInPlaceTrigger.gameObject.SetActive(false);
    }

    private void Press()
    {
        pressAnimator.Play("Press");
        Pressing?.Invoke();
    }
}
