using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickAction
{
    /// <summary>
    /// Accepts a new click status, and returns True if it should block all other actions. 
    /// </summary>
    /// <param name="clickstatus"></param>
    /// <returns></returns>
    bool AnswerNewClick(bool clickstatus, ISelectable selectable);

    /// <summary>
    /// Performs click actions and returns True if the click can be released, false if it should be held onto
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    bool ReleaseClickAction(bool status);

}
