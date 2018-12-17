using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenScaleToFloatResource : MonoBehaviour {

    public bool runOnUpdateValue = true;
    public FloatResource resource;

    public Vector3 startingScale;
    private float lastResourceValue;
    public Transform targetTransform;

    public float transitionTime = 0.5f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);

    private void Start()
    {
        targetTransform = targetTransform ?? transform;
        startingScale = targetTransform.localScale;
        lastResourceValue = resource.Value;

        if (runOnUpdateValue)
            resource.OnValueChanged.AddListener(UpdateFromResource);
    }

    public void UpdateFromResource()
    {
        Debug.Log("Updating to a new value of " + resource.Value);
        if (resource.Value == lastResourceValue)
            return;

        StopAllCoroutines();
        StartCoroutine(TweenScale(targetTransform.localScale, startingScale * resource.Value));
        lastResourceValue = resource.Value;
    }

    IEnumerator TweenScale(Vector3 originScale, Vector3 destinationScale)
    {
        float elapsedTime = 0f;
        while (elapsedTime <= transitionTime)
        {
            float ratio = elapsedTime / transitionTime;
            targetTransform.localScale = Vector3.LerpUnclamped(originScale, destinationScale, curve.Evaluate(ratio));

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
