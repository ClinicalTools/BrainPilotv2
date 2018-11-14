using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UIClickEvent : UnityEvent<UIElement, bool> { }

public class ClickActionUIElement : ClickAction {

    public UIClickEvent uiClickEvent;           // passes a UI Element and a bool
    public UIClickEvent cancelClickEvent;
    public ClickUpdateEvent regularClickEvent;  // just passes a bool
    public UnityEvent clickDown;
    public UnityEvent clickUp;

    public UIElement uiElement;

    public LineCastSelector parentSelector;

    /// <summary>
    /// Get access to parent selector so we can compare current selectable on click up
    /// </summary>
    private void Start()
    {
        parentSelector = parentSelector ?? GetComponentInParent<LineCastSelector>();
    }

    /// <summary>
    /// For UI Elements, we just need to save the new click and return true.
    /// </summary>
    /// <param name="click"></param>
    /// <param name="selectable"></param>
    /// <returns></returns>
    public override bool AnswerNewClick(bool click, ISelectable selectable)
    {
        
        if (selectable is UIElement)
        {
            uiElement = selectable as UIElement;    // don't do anything except assign the value, we want to respond in the release method
            
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Releases focus of click if the pointer is no longer on the target, or if we are clicking up. Keep focus if it is a click down. 
    /// </summary>
    /// <param name="click">Click == Down</param>
    /// <returns></returns>
    public override bool ReleaseClickAction(bool click)
    {
        // bail out if the selected element isn't this one (user has moved selector elsewhere after click down)
        if (parentSelector.uiTarget != uiElement)
        {
            cancelClickEvent.Invoke(uiElement, click);
            return true;
        }

        // invoke contextual event
        uiClickEvent.Invoke(uiElement, click);
        regularClickEvent.Invoke(click);

        if (click)
            clickDown.Invoke();
        else
            clickUp.Invoke();

        // release if click up
        return !click;
    }


}
