using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Class for Taking Click Events, testing if this action should be taken, and then holding on to the click event.
/// </summary>
public class ClickAction : MonoBehaviour, IClickAction
{
    public virtual bool AnswerNewClick(bool clickstatus, ISelectable selectable)
    {
        return false;
    }

    public virtual bool ReleaseClickAction(bool status)
    {
        return true;
    }

}
