using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ClickActionFallback : ClickAction {

    /// <summary>
    /// passes a bool event with the status (down / up) of the click
    /// </summary>
    public ClickUpdateEvent clickEvent;

    public override bool AnswerNewClick(bool clickstatus, ISelectable selectable)
    {
        return true;
    }

    public override bool ReleaseClickAction(bool status)
    {
		if (toggleClick) {
			if (status) {
				if (!toggleVal) {
					toggleVal = true;
					clickEvent.Invoke(status);
				} else {
					toggleVal = false;
				}
			} else if (!toggleVal) {
				clickEvent.Invoke(status);
			}
			return true;
		} else {
			clickEvent.Invoke(status);
			return true;
		}
    }

	public void SetToggle(bool b)
	{
		toggleClick = b;
	}
}
