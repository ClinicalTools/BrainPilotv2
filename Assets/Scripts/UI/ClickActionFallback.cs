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
        clickEvent.Invoke(status);
        return true;
    }

}
