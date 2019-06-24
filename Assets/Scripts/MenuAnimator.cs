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
				if (useAnimation) {
					PlayAnimation("Entering");
				} else {
					ani.SetTrigger(AnimationActions.FadeInRight.ToString());
				}
			}
		} else {
			if (animate) {
				if (useAnimation) {
					PlayAnimation("Selected");
				} else {
					ani.SetTrigger(AnimationActions.FadeOutLeft.ToString());
					StartCoroutine(DisableWhenAniDone());
				}
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
				if (useAnimation) {
					PlayAnimation("Selected", true);
				} else {
					ani.SetTrigger(AnimationActions.FadeInLeft.ToString());
				}
			}
		} else {
			if (animate) {
				if (useAnimation) {
					PlayAnimation("Entering", true);
					StartCoroutine(DisableWhenAniDone());
				} else {
					ani.SetTrigger(AnimationActions.FadeOutRight.ToString());
					StartCoroutine(DisableWhenAniDone());
				}
			} else {
				gameObject.SetActive(false);
			}
		}
	}

	public void PlayAnimation(string name, bool reverse = false)
	{
		if (reverse)
			PlayAnimation(name, -1f, true);
		else
			PlayAnimation(name, 1, false);
	}

	public void PlayAnimation(string name, float speed, bool playFromEnd)
	{
		if (ani2 == null) {
			Start();
		}
		foreach(AnimationState state in ani2) {
			state.clip.legacy = true;
		}
		ani2[name].speed = speed;
		if (playFromEnd) {
			ani2[name].time = ani2[name].length;
		} else {
			ani2[name].time = 0;
		}
		ani2.Stop();
		ani2.clip = ani2[name].clip;
		ani2.Play(name);
	}

	private IEnumerator DisableWhenAniDone()
	{
		if (useAnimation) {
			while (ani2.isPlaying) {
				yield return null;
			}
		} else {
			while (!ani.GetCurrentAnimatorStateInfo(0).IsName("Done")) {
				yield return null;
			}
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
