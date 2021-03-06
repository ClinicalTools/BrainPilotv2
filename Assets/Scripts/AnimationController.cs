﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

	new public Animation animation;

	public bool active;
	private Selectable lastSelectable;

	/**
	 * The idea is to play the animation only on selection switch
	 */

	public void PlayAnimation()
	{
		active = true;
		animation.Rewind();
		animation.Play();

		
		if (GetComponentInChildren<TMPro.TextMeshProUGUI>()) {
			string desc = ((BrainElement)lastSelectable)?.elementName;
			GetComponentInChildren<TMPro.TextMeshProUGUI>().text = desc;
		}
	}

	public void StopAnimation()
	{
		active = false;
		animation.Rewind();
		animation.Stop();
		if (GetComponentInChildren<TMPro.TextMeshProUGUI>()) {
			string desc = "";
			GetComponentInChildren<TMPro.TextMeshProUGUI>().text = desc;
		}
		if (GetComponentInChildren<CanvasGroup>()) {
			GetComponentInChildren<CanvasGroup>().alpha = 0;
		}
	}

	public void MoveText(bool expand)
	{
		AnimationState slide = animation["SlideOver"];
		if (expand) {
			slide.speed = 1;
			slide.time = 0;
		} else {
			slide.speed = -1;
			slide.time = slide.length;
		}
		slide.layer = 1;
		animation.Play("SlideOver", PlayMode.StopSameLayer);
	}

	public void HandleNewSelection(Selectable s)
	{

		if (s == null) {
			StopAnimation();
		} else {
			lastSelectable = s;
			PlayAnimation();
		}
	}

	public void Start()
	{
		if (animation == null) {
			animation = GetComponentInChildren<Animation>();
		}
	}
}
