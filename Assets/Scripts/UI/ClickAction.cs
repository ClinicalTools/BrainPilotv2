using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Class for Taking Click Events, testing if this action should be taken, and then holding on to the click event.
/// </summary>
public class ClickAction : MonoBehaviour, IClickAction
{
	public bool toggleClick;
	protected bool toggleVal;

	/// <summary>
	/// Answers a Click on a Selectable (could be null). Should return true if this ClickAction wants take that click. Default class returns false;
	/// </summary>
	/// <param name="clickstatus">Click == Down</param>
	/// <param name="selectable">Selectable Element We are passed</param>
	/// <returns></returns>
	public virtual bool AnswerNewClick(bool clickstatus, ISelectable selectable)
    {
        return false;
    }

    /// <summary>
    /// Determines if we need to retain focus from our click switch. Returns True if the Actions can be finished in this method, and false if we need to wait. 
    /// Example: Retturn false to retain click focus for a UI button that should only respond to click up. 
    /// Default class returns true.
    /// </summary>
    /// <param name="clickstatus">Click == Down</param>
    /// <returns></returns>
    public virtual bool ReleaseClickAction(bool clickstatus)
    {
        return true;
    }


    
}
