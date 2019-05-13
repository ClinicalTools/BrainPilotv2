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
	public bool animate;
	Animator ani;
	Animation ani2;
	public void Start()
	{
		ani = GetComponent<Animator>();
		ani2 = GetComponent<Animation>();
	}

	public void SetActive()
	{
		Debug.Log("Active");
		gameObject.SetActive(true);
	}

	public void SetInactive()
	{
		Debug.Log("Inactive");
		gameObject.SetActive(false);
	}

	public void Transition(bool enabling) 
	{
		if (ani == null) {
			Start();
		}

		if (enabling) {
			gameObject.SetActive(true);
			if (animate) {
				//ani.SetTrigger(AnimationActions.FadeInRight.ToString());
				PlayAnimation("Entering");
			}
			//ani2.Play("Entering");
		} else {
			//ani2.Play("Selected");
			if (animate) {
				//ani.SetTrigger(AnimationActions.FadeOutLeft.ToString());
				//StartCoroutine(DisableWhenAniDone());
				PlayAnimation("Selected");
			} else {
				gameObject.SetActive(false);
			}
		}
	}

	public void BackTransition(bool enabling)
	{
		if (ani == null) {
			Start();
		}
		if (enabling) {
			gameObject.SetActive(true);
			if (animate) {
				//ani.SetTrigger(AnimationActions.FadeInLeft.ToString());
				PlayAnimation("Selected", true);
				//ani2.Play("Entering");
			}
		} else {
			if (animate) {
				//ani.SetTrigger(AnimationActions.FadeOutRight.ToString());
				PlayAnimation("Entering", true);
				StartCoroutine(DisableWhenAniDone());

				/*ani2["Entering"].speed = -1;
				ani2["Entering"].time = ani2["Entering"].length;
				ani2.Play("Entering");*/
			} else {
				gameObject.SetActive(false);
			}
		}
	}

	public void PlayAnimation(string name, bool reverse = false)
	{
		PlayAnimation(name, -1f, true);
	}

	public void PlayAnimation(string name, float speed, bool playFromEnd)
	{
		if (ani2 == null) {
			Start();
		}
		ani2[name].speed = speed;
		if (playFromEnd) {
			ani2[name].time = ani2[name].length;
		} else {
			ani2[name].time = 0;
		}
		ani2.Play(name);
	}

	private IEnumerator DisableWhenAniDone()
	{
		while(ani2.isPlaying) {
			yield return null;
		}
		gameObject.SetActive(false);
	}
#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(MenuAnimator))]
	public class MenuAnimatorInspector : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Play Entering Forward")) {
				((MenuAnimator)target).PlayAnimation("Entering");
			}
			if (GUILayout.Button("Play Entering Reversed")) {
				((MenuAnimator)target).PlayAnimation("Entering", true);
			}
			if (GUILayout.Button("Play Selected Forward")) {
				((MenuAnimator)target).PlayAnimation("Selected");
			}
			if (GUILayout.Button("Play Selected Reversed")) {
				((MenuAnimator)target).PlayAnimation("Selected", true);
			}

		}
	}
#endif
}
