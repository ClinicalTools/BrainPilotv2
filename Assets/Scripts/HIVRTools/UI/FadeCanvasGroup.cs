using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class FadeCanvasGroup : MonoBehaviour {

    public bool visible;

    public float fadeTime;

    public AnimationCurve animationCurve;

    CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        visible = (canvasGroup.alpha != 0);
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTo(1));
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTo(0));
    }

    IEnumerator FadeTo(float alpha)
    {
        //Debug.Log("Canvas group is being faded to " + alpha.ToString());

        visible = (alpha != 0);

        float elapsedTime = 0;
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        while (elapsedTime <= fadeTime)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, alpha, animationCurve.Evaluate(elapsedTime / fadeTime));
        }
       
    }


}
