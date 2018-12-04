using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickActionNoTarget : ClickAction {

    public Selectable targetSelectable;

    public void UpdateSelection(Selectable newTarget)
    {
        targetSelectable = newTarget;
    }

    public override bool AnswerNewClick(bool clickstatus, ISelectable selectable)
    {
        if (targetSelectable == null)
            return true;
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
