using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FrotherGameplay : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor mugSocket;
    [SerializeField] private GameObject tongue;
    [SerializeField] private float frothInterval = 0.5f;
    [SerializeField] private float frothAmount = 1f;

    private Coroutine _frothingRoutine;
    private Coroutine _rotatingRoutine;

    private void OnEnable()
    {
        mugSocket.selectEntered.AddListener(OnMugPlaced);
        mugSocket.selectExited.AddListener(OnMugRemoved);
    }

    private void OnDisable()
    {
        mugSocket.selectEntered.RemoveListener(OnMugPlaced);
        mugSocket.selectExited.RemoveListener(OnMugRemoved);
    }

    private void OnMugPlaced(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.TryGetComponent(out MugGameplay mug))
        {
            _frothingRoutine = StartCoroutine(FrothLoop(mug));
            _rotatingRoutine = StartCoroutine(RotateTongue());
        }
    }

    private void OnMugRemoved(SelectExitEventArgs args)
    {
        if (_frothingRoutine != null)
        {
            StopCoroutine(_frothingRoutine);
            StopCoroutine(_rotatingRoutine);
            _frothingRoutine = null;
            _rotatingRoutine = null;
        }
    }

    private IEnumerator RotateTongue()
    {
        Tween tongueTween;
        while (true)
        {
            tongueTween = tongue.transform.DORotate(new Vector3(0.0f, 360.0f, 0.0f), 1.0f, RotateMode.FastBeyond360);
            yield return tongueTween.WaitForCompletion();
        }
    }

    private IEnumerator FrothLoop(MugGameplay mug)
    {
        while (true)
        {
            mug.Froth(frothAmount);
            yield return new WaitForSeconds(frothInterval);
        }
    }
}
