using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MenuAnimator))]
public class MenuPageElement : MonoBehaviour {

	public MenuPageElement m_parent;
	private MenuAnimator animator;

	public void Activate()
	{
		if (!gameObject.activeSelf) {
			Activate(false);
		}
	}

	public void GoBack()
	{
		Activate(true);
	}

	public MenuAnimator GetAnimator()
	{
		if (animator == null) {
			animator = GetComponent<MenuAnimator>();
		}
		return animator;
	}

	/// <summary>
	/// Activates a menu screen and disables all others
	/// </summary>
	/// <param name="goingBack"></param>
	private void Activate(bool goingBack)
	{
		Transform child;
		if (animator == null) {
			animator = GetComponent<MenuAnimator>();
		}

		if (goingBack) {
			if (m_parent == null) {
				Debug.LogWarning("Error: Cannot go back because parent is unassigned");
				return;
			}
			animator.BackTransition(false);
			m_parent.GetAnimator().BackTransition(true);
		} else {
			for (int i = 0; i < transform.parent.childCount; i++) {
				child = transform.parent.GetChild(i);
				if (child.GetComponent<MenuPageElement>() && child.gameObject.activeInHierarchy) {
					if (child.GetComponent<MenuAnimator>() == null) {
						child.gameObject.SetActive(false);
					} else {
						child.GetComponent<MenuAnimator>().Transition(false);
					}
					break;
				}
			}
			animator.Transition(true);
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(MenuPageElement))]
	public class MenuPageElementInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Activate")) {
				((MenuPageElement)target).Activate();
			}

			if (GUILayout.Button("Go Back")) {
				((MenuPageElement)target).GoBack();
			}
		}
	}
#endif
}
