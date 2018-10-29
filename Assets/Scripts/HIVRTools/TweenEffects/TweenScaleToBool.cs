using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenScaleToBool : MonoBehaviour {

    public float scaleFactor = 2f;

    public float transitionTime = 0.3f;
    public AnimationCurve curve;

    public bool isActive = false;

    public Transform target;
    Vector3 startingScale;

    private void Start()
    {
        isActive = false;
        target = target ?? transform;

        startingScale = target.localScale;
    }

    public void TweenToBool(bool status)
    {
        if (status == isActive)
            return;

        StopAllCoroutines();

        if (status)
        {
            StartCoroutine(TweenScaleTo(scaleFactor * startingScale));
            
        }
        else
        {
            StartCoroutine(TweenScaleTo(startingScale));
        }

        isActive = status;
    }

    IEnumerator TweenScaleTo(Vector3 targetScale)
    {
        float elapsedTime = 0;
        Vector3 scaleFrom = target.localScale;
        while (elapsedTime <= transitionTime)
        {
            float t = elapsedTime / transitionTime;
            target.localScale = Vector3.LerpUnclamped(scaleFrom, targetScale, curve.Evaluate(t));
            elapsedTime += Time.deltaTime;
            yield return null;

        }

    }



}
