using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Takes 'click' events from ClickInput and Iterates through ClickAction(s) until one of the ClickActions wants to do something with the click. 
/// </summary>
public class ClickSwitch : MonoBehaviour {

    public List<ClickAction> clickActions;

    public ClickAction currentAction;

    public LineCastSelector selector;

    bool clickStatus;

    private void Start()
    {
        clickStatus = false;
    }

    public void GetNewClick(bool status)
    {
        if (status == clickStatus)
            return;

        if (clickActions == null)
            return;

        if (currentAction == null)
        {
            var target = selector.furthestSelectable;

            currentAction = clickActions.Find(click => click.AnswerNewClick(status, target));
        }
        if (currentAction.ReleaseClickAction(status))
        {
            currentAction = null;
        }
    }

}
