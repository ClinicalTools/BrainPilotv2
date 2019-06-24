using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
[/*RequireComponent(typeof(Animator)),*/ RequireComponent(typeof(CanvasGroup))]
public class MenuAnimator : MonoBehaviour {

	private enum AnimationActions
	{
		FadeOutRight,
		FadeOutLeft,
		FadeInRight,
		FadeInLeft
	}
	public bool animate;
	public bool useAnimation;
	Animator ani;
	Animation ani2;
	public void Start()
	{
		ani = GetComponent<Animator>();
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
				ani.SetTrigger(AnimationActions.FadeInRight.ToString());
			}
		} else {
			if (animate) {
				ani.SetTrigger(AnimationActions.FadeOutLeft.ToString());
				StartCoroutine(DisableWhenAniDone());
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
				ani.SetTrigger(AnimationActions.FadeInLeft.ToString());
			}
		} else {
			if (animate) {
				ani.SetTrigger(AnimationActions.FadeOutRight.ToString());
				StartCoroutine(DisableWhenAniDone());
			} else {
				gameObject.SetActive(false);
			}
		}
	}

	private IEnumerator DisableWhenAniDone()
	{
		int loopProtection = 0;
		while (!ani.GetCurrentAnimatorStateInfo(0).IsName("Done")) {
			loopProtection++;
			if (loopProtection >= 30) {
				loopProtection = 0;
				break;
			}
			yield return null;
		}
		gameObject.SetActive(false);
	}
#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(MenuAnimator)), UnityEditor.CanEditMultipleObjects()]
	public class MenuAnimatorInspector : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Play Entering Forward")) {
				//((MenuAnimator)target).PlayAnimation("Entering");
				((MenuAnimator)target).Transition(true);
			}
			if (GUILayout.Button("Play Entering Reversed")) {
				((MenuAnimator)target).BackTransition(false);
				//((MenuAnimator)target).PlayAnimation("Entering", true);
			}
			if (GUILayout.Button("Play Selected Forward")) {
				((MenuAnimator)target).Transition(false);
				//((MenuAnimator)target).PlayAnimation("Selected");
			}
			if (GUILayout.Button("Play Selected Reversed")) {
				//((MenuAnimator)target).PlayAnimation("Selected", true);
				((MenuAnimator)target).BackTransition(true);
			}

		}
	}
#endif
}
