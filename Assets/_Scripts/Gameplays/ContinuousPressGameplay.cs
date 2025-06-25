using System;
using System.Collections;
using UnityEngine;

public class ContinuousPressGameplay : MonoBehaviour
{
    public event Action Pressing;
    [SerializeField] private ContinuousTrigger continuousTrigger;
    [SerializeField] private Animator pressAnimator;

    private bool _notPressedYet = true;

    private void Update()
    {
        if (!continuousTrigger)
        {
            return;
        }
        if (continuousTrigger.IsTouching)
        {
            pressAnimator.Play("Press", 0, continuousTrigger.PressProgress);
            pressAnimator.speed = 0f;
            if (continuousTrigger.PressProgress == 1.0f && _notPressedYet)
            {
                _notPressedYet = false;
                Pressing?.Invoke();
            }

        }
        else if (continuousTrigger.PressProgress == 0.0f)
        {
            _notPressedYet = true;
        }
    }
    
    public void EnablePressing()
    {
        continuousTrigger.gameObject.SetActive(true);
    }

    public void DisablePressing()
    {
        continuousTrigger.gameObject.SetActive(false);
    }
}
