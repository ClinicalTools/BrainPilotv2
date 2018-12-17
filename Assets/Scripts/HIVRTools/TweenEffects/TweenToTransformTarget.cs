using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TweenToTransformTarget : MonoBehaviour
{

    public Transform targetTransform;
    public Transform transformToTween;

    public Transform originTransform;
    public Vector3 originOffset;

    public Vector3 OriginPosition
    {
        get
        {
            return originTransform.position + originOffset;
        }
    }
    

    public bool tweenState;

    public float transitionTime = 0.3f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);
    public float delay = 0f;

    public UnityEvent transitionComplete;

    private void OnEnable()
    {
        transformToTween = transformToTween ?? transform;
        originTransform = originTransform ?? transform;
        tweenState = false;
    }

    public void GetNewTarget(Transform newTarget)
    {
        targetTransform = newTarget;
    }



    public void Toggle()
    {
        if (tweenState)
            TweenToOrigin();
        else
            TweenToTarget();
    }

    public void TweenToTarget()
    {
        StopAllCoroutines();
        StartCoroutine(RunTweenAction(OriginPosition, targetTransform.position));
        tweenState = true;
    }

    public void TweenToOrigin()
    {
        StopAllCoroutines();
        StartCoroutine(RunTweenAction(targetTransform.position, OriginPosition));
        tweenState = false;
    }

    IEnumerator RunTweenAction(Vector3 origin, Vector3 destination)
    {
        float timeElapsed = 0f;
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        while (timeElapsed <= transitionTime)
        {
            float ratio = timeElapsed / transitionTime;
            Vector3 nextPosition = Vector3.Lerp(origin, destination, curve.Evaluate(ratio));
            transformToTween.position = nextPosition;

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transitionComplete.Invoke();
    }
}
