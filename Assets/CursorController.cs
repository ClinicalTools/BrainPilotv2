using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{

	public LineCastSelector selectorCursor;

	//Make sure the meshrenderer is disabled on start
	public MeshRenderer canvasCursor;

	private bool onCanvas = false;
	private bool activeState = true;

    // Update is called once per frame
    void Update()
    {
		if (canvasCursor.enabled) {
			if (!onCanvas) {
				activeState = selectorCursor.isActive;
				selectorCursor.Disable();
				onCanvas = true;
			}
		} else if (onCanvas) {
			selectorCursor.Enable(selectorCursor.isActive);
			onCanvas = false;
		}
    }
}
