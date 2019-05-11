using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MenuAnimator))]
public class MenuPageElement : MonoBehaviour {

	public MenuPage m_parent;
	private MenuAnimator animator;

	// Use this for initialization
	void Start()
	{

	}

	public void Activate()
	{
		Activate(false);
	}

	public void GoBack()
	{
		if (m_parent == null) {
			Debug.Log("Cannot go back. No parent menu");
			return;
		}

		Activate(true);
	}

	private void Activate(bool goingBack)
	{
		Transform child;
		if (animator == null) {
			animator = GetComponent<MenuAnimator>();
		}
		if (m_parent == null) {
			m_parent = GetComponentInParent<MenuPage>();
		}
		if (m_parent != null) {
			if (goingBack) {
				animator.BackTransition(false);
				
			} else {
				animator.Transition(true);
			}
			for (int i = 0; i < transform.parent.transform.childCount; i++) {
				child = transform.parent.transform.GetChild(i);
				if (child != transform) {
					if (goingBack) {
						child.GetComponent<MenuAnimator>().BackTransition(true);
					} else {
						if (child.gameObject.activeInHierarchy && child.GetComponent<MenuAnimator>()) {

						} else {
							child.GetComponent<MenuAnimator>().Transition(false);
						}
					}
				} else {
					transform.parent.GetChild(i).gameObject.SetActive(false);
				}
			}
		}
	}
}
