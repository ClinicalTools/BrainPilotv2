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
	Selectable sel;
    // Update is called once per frame
    void Update()
    {
		if (canvasCursor.enabled) {
			if (!onCanvas) {
				if (!selectorCursor.isActive) {
					//Have something chosen
					sel = selectorCursor.furthestSelectable;
				}

				activeState = selectorCursor.isActive;
				selectorCursor.Disable();
				onCanvas = true;

				if (sel != null) {
					selectorCursor.furthestSelectable = sel;
					selectorCursor.selectableTargetEvent.Invoke(sel);
					sel = null;
				}
			}
		} else if (onCanvas) {
			selectorCursor.Enable(selectorCursor.isActive);
			onCanvas = false;
		}
    }

	public void CanceledClick(bool click)
	{
		if (onCanvas & !click) {
			selectorCursor.GetClickDown(false);
		}
	}
}
