using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{

	public LineCastSelector selectorCursor;

	//Make sure the meshrenderer is disabled on start
	public MeshRenderer canvasCursor;

	private bool onCanvas = false;
	//private bool activeState = true;
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

				//activeState = selectorCursor.isActive;
				//print("Setting active state to " + activeState);
				selectorCursor.Disable();
				onCanvas = true;

				if (sel != null) {
					selectorCursor.furthestSelectable = sel;
					selectorCursor.selectableTargetEvent.Invoke(sel);
					sel = null;
				}
			}
		} else if (onCanvas) {
			//bool activatingWith = selectorCursor.isActive;
			//bool activatingWith = true;
			//bool activatingWith = activeState;
			bool activatingWith = (selectorCursor.furthestSelectable == null);

			print("Enabling with " + activatingWith);
			selectorCursor.Enable(activatingWith);
			onCanvas = false;
		}
    }

	public void CanceledClickCheck(bool click)
	{
		if (onCanvas && !click) {
			print("Click cancled");
			selectorCursor.Disable();
			FindObjectOfType<NewSelection>().CanceledClick(false);
			return;
			selectorCursor.TurnOffCursor();
			selectorCursor.isActive = false;
			//selectorCursor.Enable(true);
			//onCanvas = false;
			//activeState = true;
		}
	}
}
