using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UIClickEvent : UnityEvent<UIElement, bool> { }

public class ClickActionUIElement : ClickAction {

    public UIClickEvent uiClickEvent;
    public UnityEvent clickDown;
    public UnityEvent clickUp;

    public UIElement uiElement;

    public override bool AnswerNewClick(bool clickstatus, ISelectable selectable)
    {
        if (selectable is UIElement)
        {
            uiElement = selectable as UIElement;
            
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool ReleaseClickAction(bool status)
    {
        // invoke contextual event
        uiClickEvent.Invoke(uiElement, status);

        if (status)
            clickDown.Invoke();
        else
            clickUp.Invoke();

        // release if click up
        return !status;
    }

}
