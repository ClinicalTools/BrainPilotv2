using System.Collections;
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
			string desc = ((BrainElement)lastSelectable)?.description;
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
	}

	public void HandleNewSelection(Selectable s)
	{
		if (s == null && active) {
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
