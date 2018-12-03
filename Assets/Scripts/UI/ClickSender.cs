using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSender : MonoBehaviour
{
    /// <summary>
    /// Sends a Click Update to the UIElement Scriptable Object, invoking the event and loading the state on that element
    /// </summary>
    /// <param name="element"></param>
    /// <param name="click"></param>
    public void SendUIClick(UIElement element, bool click)
    {
        Debug.Log("Send Click Called on " + element?.name);

        if (element == null)
            return;

        element.clickEvent.Invoke(click);
        element.ActivateState(element.clickState);
        element.click = click;

        if (!click)
            element.onClickAction.Invoke();
    }


    public void CancelClick(UIElement element, bool click)
    {
        Debug.Log("Cancel Click Called on " + element.name);
        if (element == null)
            return;
        
        element.click = click;
        element.clickEvent.Invoke(click);
        element.DeactivateState(element.clickState);
    }

}
