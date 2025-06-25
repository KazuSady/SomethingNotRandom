using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FrotherGameplay : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor mugSocket;
    [SerializeField] private float frothInterval = 0.5f;
    [SerializeField] private float frothAmount = 1f;

    private Coroutine _frothingRoutine;

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
        }
    }

    private void OnMugRemoved(SelectExitEventArgs args)
    {
        if (_frothingRoutine != null)
        {
            StopCoroutine(_frothingRoutine);
            _frothingRoutine = null;
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
