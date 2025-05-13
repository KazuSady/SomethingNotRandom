using System.Collections;
using DG.Tweening;
using UnityEngine;

public class LiquidBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject liquidObject;
    [SerializeField] private float topScale;
    [SerializeField] private GameObject liquidHolder;

    public void PourLiquid()
    {
        liquidObject.SetActive(true);
        StartCoroutine(SimulateLiquid());
    }

    private IEnumerator SimulateLiquid()
    {
        var tween = liquidObject.transform.DOScaleY(topScale, 1.0f);
        yield return tween.WaitForCompletion();
        liquidObject.transform.parent = liquidHolder.transform;
    }
}
