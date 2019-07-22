using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{

	public LineCastSelector selectorCursor;

	public MeshRenderer canvasCursor;

	private bool onCanvas;

    // Update is called once per frame
    void Update()
    {
		if (canvasCursor.enabled) {
			selectorCursor.Disable();
			onCanvas = true;
		} else if (onCanvas) {
			selectorCursor.Enable();
			onCanvas = false;
		}
    }
}
