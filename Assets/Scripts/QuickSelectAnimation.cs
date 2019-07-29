using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSelectAnimation : MonoBehaviour {

	private float aniDirection;
	private bool active;
	private bool isEnabled;

	[SerializeField]
	private bool connectButtons = true;

	[SerializeField]
	private bool setIdx = false;
	public int buttonCount = 4;

	private float ROT_AMOUNT = 36f;
	public float aniSpeed = 10;
	public AnimationCurve curve;

	private const int MAX_BUTTONS = 9;
	private const float MIN_ROT_AMOUNT = 36f;
	private const float MAX_ROT_AMOUNT = 162f;
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
			} else if (progress <= .02 && aniDirection < 0) {
				progress = 0;
				active = false;
				gameObject.SetActive(false);
			}
		}
	}

	public void ToggleActive()
	{
		gameObject.SetActive(true);
		if (isEnabled) {
			Deactivate();
		} else {
			Activate(buttonCount);
		}
		active = true;
		ActivateChildren();
		return;


		gameObject.SetActive(true);
		if (isEnabled) {
			//Deactivate
			aniDirection = -1;
			progress = 1;
			isEnabled = false;
		} else {
			//Activate
			aniDirection = 1;
			progress = 0 + .02f;
			isEnabled = true;
			if (connectButtons) {
				ROT_AMOUNT = MIN_ROT_AMOUNT;
			} else {
				ROT_AMOUNT = TOTAL_DEGREES / buttonCount;
			}
		}
		active = true;
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
		buttonCount = Mathf.Min(n, MAX_BUTTONS);

		aniDirection = 1;
		//progress = 0 + .02f;
		active = true;
		isEnabled = true;

		if (connectButtons) {
			ROT_AMOUNT = MIN_ROT_AMOUNT;
		} else {
			ROT_AMOUNT = TOTAL_DEGREES / buttonCount;
		}
		ActivateChildren();

		gameObject.SetActive(true);
	}

	public void Deactivate()
	{
		aniDirection = -1;
		//progress = 1;
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
			if (i < buttonCount) {
				transform.GetChild(i).gameObject.SetActive(true);
				if (setIdx) {
					transform.GetChild(i).GetComponentInChildren<TMPro.TextMeshProUGUI>().text = i.ToString();
				}
			} else {
				transform.GetChild(i).gameObject.SetActive(false);
			}

		}
	}
}
