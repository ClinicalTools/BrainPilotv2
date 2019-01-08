using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickActionNoTarget : ClickAction {

    public Selectable targetSelectable;

    public GameplayEvent noTargetClick;

    public void UpdateSelection(Selectable newTarget)
    {
        targetSelectable = newTarget;
    }

    public override bool AnswerNewClick(bool clickstatus, ISelectable selectable)
    {
        if (targetSelectable == null)
        {
            noTargetClick.RaiseEvent();
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool ReleaseClickAction(bool clickstatus)
    {
        return true;
    }
}
