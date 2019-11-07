using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For resizing the user and platform
/// </summary>
public class TweenScaleByFactor : MonoBehaviour
{

	//public bool runOnUpdateValue = true;
	public float scale;

	private float lastResourceValue;
	public Transform targetTransform;

	public const float defaultTransitionTime = 0.5f;
	public float transitionTime = defaultTransitionTime;
	public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);

	bool scaling = false;

	public float maxScale;
	public float minScale;

	LineCastSelector selector;
	Transform ovrCursor;

	private void Start()
	{
		if (targetTransform == null)
			targetTransform = transform;

		lastResourceValue = targetTransform.localScale.x;

		selector = GetComponentInChildren<LineCastSelector>();
		ovrCursor = GetComponentInChildren<OVRGazePointer>()?.transform.parent;

		//if (runOnUpdateValue)
		//resource.OnValueChanged.AddListener(UpdateFromResource);
	}

	public void TweenToScale(float scaleVal, float time = defaultTransitionTime)
	{
		UpdateByFactor(scaleVal / targetTransform.localScale.x, time);
	}

	public void UpdateByFactor(float factor, float time)
	{
		SetScaleFactor(factor);
		SetTransitionTime(time);
		UpdateByFactor();
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

	public void SetTransitionTime(float time)
	{
		transitionTime = time;
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
			ScaleOtherObjects(targetTransform.localScale.x / maxScale);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		scaling = false;
	}

	/// <summary>
	/// Scales other objects that the platform needs adjusted
	/// </summary>
	/// <param name="ratio">The x scale over max scale</param>
	private void ScaleOtherObjects(float ratio)
	{
		//Ratio values:
		//50 = 1
		//5 = .1
		//.5 = .01
		
		//Adjust the selector's line width
		//.2 @ 50
		//.02 @ 5
		//.002 @ .5
		selector.line.startWidth = .2f * ratio;

		//Adjust the canvas cursor's size. 3D cursor is independent
		//.1 @ 50
		//1 @ 5
		//10 @ .5
		ovrCursor.localScale = Vector3.one * (.1f / ratio);


		//Adjust the line selector max distance and line speed
		//20 @ 50
		//15 @ 5
		//10 @ .5
		//float r2 = 2 * Mathf.Sqrt(ratio * maxScale - .5f) + 10;
		float distanceRatio = selector.distance / selector.maxDistance;
		selector.maxDistance = 2 * Mathf.Sqrt(ratio * maxScale - .5f) + 10;
		selector.distance = distanceRatio * selector.maxDistance;
		selector.inputEffectFactor = selector.maxDistance - 5;


		//Adjust movement speed
		//10 @ 50 : 1
		//5 @ 5 : .1
		//2.5 @ .5 : .01
		AnchorUXController controller = GetComponentInChildren<AnchorUXController>();
		if (ratio * maxScale < 5) {
			//Scaling when below 5
			//250/9*ratio
			//2.5/4.5*ratio*maxscale
			//controller.forwardSpeed = (3.5f / 4.5f) * (ratio * maxScale - .5f) + 1.5f;
			controller.forwardSpeed = (5f * (ratio * maxScale) + 20f) / 9f; //For 2.5
			controller.forwardSpeed = (2f/3f) * (ratio * maxScale) + (5f/3f); //For 2

		} else {
			//Scaling when above 5
			//controller.forwardSpeed = (ratio * maxScale) / 5f;
			controller.forwardSpeed = ((ratio * maxScale) + 40) / 9f;
		}

		//Adjust comfort plane's sensitivity
		//1.5 @ 50
		//1 @ 5
		//.5 @ .5
		TweenAlphaByVelocity comfortAlpha = GetComponentInChildren<TweenAlphaByVelocity>();
		comfortAlpha.scale = controller.forwardSpeed / 5;

		//Platform Cone
		//10 @ 50
		//1 @ 5
		//.1 @ 5
		Light cone = GetComponentInChildren<AdvancedDissolve_Example.Controller_Mask_Cone>().spotLight1;
		cone.range = ratio * 10;

		//Adjust the drone's target position
		//pos = -1.5, .45, 2 @ 5
		//pos *= 10
		//pos *= 1
		//pos *= .1
		//Scale by 50*ratio
		DroneController drone = FindObjectOfType<DroneController>();
		Vector3 pos = new Vector3(-1.5f, .45f, 2);
		pos *= ratio * 10;
		drone.UpdateGoals(new Vector3[] { pos } );

		//Adjust the drone's scale
		// 2f  @ 50
		// .2f @ 5
		// .02f @ .5
		drone.transform.localScale = Vector3.one * (2f * ratio);

		//Adjust the holo glow on the minimap
		//.1
		//.01 @ 5
		//.001
		transform.Find("Brain/Holo Brain/Holo Brainn/GameObject/Cube").GetComponent<Light>().range = ratio * .1f;
	}

	void Update(){
		if(OVRInput.GetDown(OVRInput.Button.Two)) 
		{
            switch (scale)
			{
				case .01f:
					break;
				case .1f:
					UpdateByFactor(.01f);
					break;
				case 1f:
					UpdateByFactor(.1f);
					break;
				case 10f:
					UpdateByFactor(1f);
					break;
				case 100f:
					UpdateByFactor(10f);
					break;
				default:
					Debug.Log("Shrink Failed: Scale Unknown");
					break;
			}
        }
		if(OVRInput.GetDown(OVRInput.Button.Four))
		{
            switch (scale)
			{
				case .01f:
					UpdateByFactor(.1f);
					break;
				case .1f:
					UpdateByFactor(1f);
					break;
				case 1f:
					UpdateByFactor(10f);
					break;
				case 10f:
					UpdateByFactor(100f);
					break;
				case 100f:
					break;
				default:
					Debug.Log("Growth Failed: Scale Unknown");
					break;
			}
		}
	}
}
