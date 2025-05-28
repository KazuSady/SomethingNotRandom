using System.Collections;
using UnityEngine;

public class LiquidBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject liquidObject;
    [SerializeField] private Transform newParent;
    [SerializeField] private Transform oldParent;
    [SerializeField] private Material liquidMaterial;
    [SerializeField] private float minCutoff = -0.5f;
    [SerializeField] private float topCutoff = 1.0f;
    [SerializeField] private float duration = 3.5f;

    public void PourLiquid()
    {
        liquidObject.SetActive(true);
        StartCoroutine(SimulateLiquid());
    }

    public void PressLiquid(float disappearingDuration)
    {
        StartCoroutine(SimulateDisappearingLiquid(disappearingDuration));
    }
    
    private IEnumerator SimulateLiquid()
    {
        var time = 0.0f;
        while (time < duration)
        {
            var t = time / duration;
            var cutoff = Mathf.Lerp(minCutoff, topCutoff, t);
            liquidMaterial.SetFloat("_Cutoff", cutoff);
            time += Time.deltaTime;
            yield return null;
        }

        if (newParent)
        {
            liquidObject.transform.SetParent(newParent);
        }
    }
    
    private IEnumerator SimulateDisappearingLiquid(float disappearingDuration)
    {
        var time = 0.0f;
        while (time < disappearingDuration)
        {
            var t = time / disappearingDuration;
            var cutoff = Mathf.Lerp(topCutoff, minCutoff, t);
            liquidMaterial.SetFloat("_Cutoff", cutoff);
            time += Time.deltaTime;
            yield return null;
        }
        liquidObject.SetActive(false);
        if (newParent)
        {
            liquidObject.transform.SetParent(oldParent);
        }
    }
}
