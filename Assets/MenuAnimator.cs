using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
[RequireComponent(typeof(Animator)), RequireComponent(typeof(CanvasGroup))]
public class MenuAnimator : MonoBehaviour {

	private enum AnimationActions
	{
		FadeOutRight,
		FadeOutLeft,
		FadeInRight,
		FadeInLeft
	}

	Animator ani;
	Animation ani2;
	public void Start()
	{
		ani = GetComponent<Animator>();
		ani2 = GetComponent<Animation>();
	}

	public void SetActive()
	{
		gameObject.SetActive(true);
	}

	public void SetInactive()
	{
		gameObject.SetActive(false);
	}

	public void Transition(bool enabling) 
	{
		if (ani == null) {
			Start();
		}

		if (enabling) {
			gameObject.SetActive(true);
			//ani2.Play("Entering");
			//ani.SetTrigger(AnimationActions.FadeInRight.ToString());
		} else {
			//ani2.Play("Selected");
			//ani.SetTrigger(AnimationActions.FadeOutLeft.ToString());
			gameObject.SetActive(false);
		}
	}

	public void BackTransition(bool enabling)
	{
		if (ani == null) {
			Start();
		}
		if (enabling) {
			gameObject.SetActive(true);
			//ani.SetTrigger(AnimationActions.FadeInLeft.ToString());
			//ani2.Play("Entering");
		} else {
			gameObject.SetActive(false);
			//ani.SetTrigger(AnimationActions.FadeOutRight.ToString());
			//ani2.Play();
		}
	}
}
