using System;
using System.Collections;
using UnityEngine;

public class ContinuousPressGameplay : MonoBehaviour
{
    public event Action Pressing;
    public event Action StoppedPressing;
    
    [SerializeField] private ContinuousTrigger continuousTrigger;
    [SerializeField] private Animator pressAnimator;


    private void Awake()
    {
        if(continuousTrigger)
        {
            continuousTrigger.Stopped += () => StoppedPressing?.Invoke();
        }
    }

    private void Update()
    {
        if (!continuousTrigger)
        {
            return;
        }
        if (continuousTrigger.IsTouching)
        {
            if (continuousTrigger.PressProgress == 0.0f)
            {
                Pressing?.Invoke();
            }
            pressAnimator.Play("Press", 0, continuousTrigger.PressProgress);
            pressAnimator.speed = 0f;
        }
        else if (continuousTrigger.PressProgress == 0.0f)
        {
            pressAnimator.Play("New State 0");
        }
    }
    
    public void EnablePressing()
    {
        continuousTrigger.gameObject.SetActive(true);
    }

    public void DisablePressing()
    {
        continuousTrigger.IsTouching = false;
        continuousTrigger.gameObject.SetActive(false);
    }
}
