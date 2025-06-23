using System;
using System.Collections;
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
        StartCoroutine(PressAnimation());
        Pressing?.Invoke();
    }

    private IEnumerator PressAnimation()
    {
        pressAnimator.Play("Press");
        pressAnimator.Update(0f);
        yield return new WaitForSeconds(pressAnimator.GetCurrentAnimatorStateInfo(0).length);
        pressAnimator.Play("New State 0");
    }
}
