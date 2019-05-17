using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenScaleByFactor : MonoBehaviour
{

	//public bool runOnUpdateValue = true;
	public float scale;

	private float lastResourceValue;
	public Transform targetTransform;

	public float transitionTime = 0.5f;
	public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);

	bool scaling = false;

	public float maxScale;
	public float minScale;

	private void Start()
	{
		if (targetTransform == null)
			targetTransform = transform;

		lastResourceValue = targetTransform.localScale.x;

		//if (runOnUpdateValue)
			//resource.OnValueChanged.AddListener(UpdateFromResource);
	}

	public void UpdateByFactor(float factor)
	{
		SetScaleFactor(factor);
		UpdateByFactor();
	}

	public void SetScaleFactor(float factor)
	{
		scale = factor;
	}

	public void UpdateByFactor()
	{
		if (scaling) {
			return;
		}
		Debug.Log("Updating to a new value of " + scale);
		//if (scale == lastResourceValue)
			//return;

		StopAllCoroutines();
		StartCoroutine(TweenScale());
		lastResourceValue = scale;
	}

	IEnumerator TweenScale()
	{
		scaling = true;
		float elapsedTime = 0f;
		Vector3 originScale = targetTransform.localScale;
		Vector3 destinationScale = targetTransform.localScale * scale;
		//scale = Mathf.Clamp(targetTransform.localScale.x * scale, minScale, maxScale);

		if (destinationScale.x < minScale) {
			destinationScale = Vector3.one * minScale;
		} else if (destinationScale.x > maxScale) {
			destinationScale = Vector3.one * maxScale;
		}
		if (originScale == destinationScale) {
			scaling = false;
			yield break;
		}
		Debug.Log(destinationScale);
		while (elapsedTime <= transitionTime) {
			float ratio = elapsedTime / transitionTime;
			targetTransform.localScale = Vector3.LerpUnclamped(originScale, destinationScale, curve.Evaluate(ratio));

			elapsedTime += Time.deltaTime;
			yield return null;
		}
		scaling = false;
	}
}
