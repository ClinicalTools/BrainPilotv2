using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TweenItemScale : MonoBehaviour {

    //public Transform target;

    public float scaleTime;

    public float scaleFactor;

    public AnimationCurve animationCurve;

    public UnityEvent effectComplete;

    public void TweenToScaleFactor(Transform target)
    {
        StartCoroutine(RunTweenToScaleFactor(target));


    }

    IEnumerator RunTweenToScaleFactor(Transform target)
    {
        float elapsedTime = 0;
        Vector3 startingScale = target.localScale;
        Vector3 targetScale = startingScale * scaleFactor;
        while (elapsedTime <= scaleTime && target != null)
        {
            Vector3 nextScale = Vector3.LerpUnclamped(startingScale, targetScale, animationCurve.Evaluate(elapsedTime / scaleTime));
            target.localScale = nextScale;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        effectComplete.Invoke();
    }

    public void TweenInFromZero(Transform target)
    {
        StartCoroutine(RunTweenInFromZero(target));
    }

    IEnumerator RunTweenInFromZero(Transform target)
    {
        float elapsedTime = 0;
        Vector3 startingScale = Vector3.zero;
        Vector3 targetScale = target.localScale;
        while (elapsedTime <= scaleTime && target != null)
        {
            Vector3 nextScale = Vector3.LerpUnclamped(startingScale, targetScale, animationCurve.Evaluate(elapsedTime / scaleTime));
            target.localScale = nextScale;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        effectComplete.Invoke();
    }
      

}
