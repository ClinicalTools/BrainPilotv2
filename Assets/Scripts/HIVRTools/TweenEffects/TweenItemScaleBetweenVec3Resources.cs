using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenItemScaleBetweenVec3Resources : MonoBehaviour {

    public Vec3Resource activeScale;
    public Vec3Resource inActiveScale;

    public float transitionTime = 0.3f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);

    public Transform target;

    private void Start()
    {
        if (target == null)
            target = transform;

        if (ActiveState)
            target.localScale = activeScale.Value;
        else
            target.localScale = inActiveScale.Value;
    }
    
    [SerializeField]
    public bool ActiveState
    {
        get
        {
            return activeState;
        }
        set
        {
            if (value != activeScale)
                SetActiveState(value);
        }
    }
    [SerializeField]
    private bool activeState;

    public void SetActiveState(bool newActiveState)
    {
        StopAllCoroutines();

        if (newActiveState)
            StartCoroutine((RunTransition(inActiveScale, activeScale)));
        else
            StartCoroutine((RunTransition(activeScale, inActiveScale)));
    }

    private IEnumerator RunTransition(Vec3Resource startingVec3, Vec3Resource endingVec3)
    {
        float elapsedTime = 0f;

        while (elapsedTime <= transitionTime)
        {
            float ratio = elapsedTime / transitionTime;
            target.localScale = Vector3.LerpUnclamped(startingVec3.Value, endingVec3.Value, curve.Evaluate(ratio));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }
}
