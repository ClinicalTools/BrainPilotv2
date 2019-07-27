using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSelectAnimation : MonoBehaviour {

	private float aniDirection;
	private bool active;
	private bool isEnabled;

	public bool connectButtons = true;

	private float ROT_AMOUNT = 36f;
	public float aniSpeed = 10;
	public bool testRotate;
	public AnimationCurve curve;

	public int buttonCount = 4;
	private const int MAX_BUTTONS = 8;
	private const float MIN_ROT_AMOUNT = 36f;
	private const float MAX_ROT_AMOUNT = 16f;
	private const float TOTAL_DEGREES = 324f;

	int i;
	Vector3 eulers;
	float progress = 0;
	float lerpVal;

	private void Awake()
	{
		for(int i = 0; i < transform.childCount; i++) {
			eulers = transform.GetChild(i).localEulerAngles;
			eulers.z = 0;
			transform.GetChild(i).localEulerAngles = eulers;
		}
	}

	private void Update()
	{
		if (active) {
			progress += aniDirection * Time.deltaTime / aniSpeed;
			lerpVal = curve.Evaluate(progress);
			for (i = 0; i < buttonCount; i++) {
				eulers = transform.GetChild(i).localEulerAngles;
				eulers.z = lerpVal * ROT_AMOUNT * (i + 1);
				//eulers.z += Time.deltaTime / aniSpeed * aniDirection * ROT_AMOUNT * (i + 1);
				transform.GetChild(i).localEulerAngles = eulers;
			}

			//If the animation is finished
			if (progress >= 1) {
				progress = 1;
				active = false;
			} else if (progress <= 0) {
				progress = 0;
				active = false;
				gameObject.SetActive(false);
			}
		} else if (testRotate) {
			aniDirection = 1;
			//progress += aniDirection * Time.deltaTime / aniSpeed;
			progress += aniDirection * Time.deltaTime / aniSpeed;
			float lerpVal = curve.Evaluate(progress);
			for (i = 0; i < transform.childCount; i++) {
				eulers = transform.GetChild(i).localEulerAngles;
				eulers.z = lerpVal * aniDirection * ROT_AMOUNT * (i + 1);
				//eulers.z += Time.deltaTime / aniSpeed * aniDirection * ROT_AMOUNT * (i + 1);
				transform.GetChild(i).localEulerAngles = eulers;
			}

			if (progress >= 1 || progress <= 0) {
				progress = 0;
			}
		}
	}

	public void ToggleActive()
	{
		gameObject.SetActive(true);
		if (isEnabled) {
			//Deactivate
			aniDirection = -1;
			progress = 1;
			isEnabled = false;
		} else {
			//Activate
			aniDirection = 1;
			progress = 0;
			isEnabled = true;
			if (connectButtons) {
				ROT_AMOUNT = MIN_ROT_AMOUNT;
			} else {
				ROT_AMOUNT = TOTAL_DEGREES / buttonCount;
			}
		}
		active = true;
		lerpVal = 0;
		ActivateChildren();
	}

	public void Activate(int n)
	{
		if (n < 0) {
			n = 0;
		}
		if (n > transform.childCount) {
			n = transform.childCount - 1;
		}
		buttonCount = n;
		aniDirection = 1;
		active = true;
		isEnabled = true;
		ActivateChildren();
	}

	public void Deactivate()
	{
		aniDirection = -1;
		active = true;
		isEnabled = false;
	}

	public void ActivateChildren()
	{
		if (buttonCount < 0) {
			buttonCount = 0;
		}
		if (buttonCount > transform.childCount) {
			buttonCount = transform.childCount;
		}
		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).gameObject.SetActive(i < buttonCount ? true : false);
		}
	}
}
