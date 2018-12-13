using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Extends SelectableElement and adds convenience layer to respond to changes in UI State (hover, click)
/// </summary>
public class UISelectableElement : SelectableElement
{

    public UIElement uiSelectable;

    public UnityEvent visibleOnEvent;
    public UnityEvent visibleOffEvent;
    public UnityEvent hoverBeginEvent;
    public UnityEvent hoverEndEvent;
    public UnityEvent clickBeginEvent;
    public UnityEvent clickEndEvent;
    public UnityEvent clickActionEvent;

    protected bool hoverState;
    protected bool clickState;
    protected bool visibleState;

    protected override void Awake()
    {
        base.Awake();

        uiSelectable = selectable as UIElement;
        uiSelectable.hoverEvent.AddListener(GetHoverState);
        uiSelectable.clickEvent.AddListener(GetClickState);
        uiSelectable.visibleEvent.AddListener(GetVisibleState);
        uiSelectable.onClickAction.AddListener(DoClickAction);

        hoverState = false;
        clickState = false;
    }

    private void GetVisibleState(bool visible)
    {
        if (visible == visibleState)
            return;

        if (visible)
            visibleOnEvent.Invoke();
        else
            visibleOffEvent.Invoke();

        visibleState = visible;
    }

    /// <summary>
    /// Method called on hover begin or end
    /// </summary>
    /// <param name="hover"></param>
    public void GetHoverState(bool hover)
    {
        if (hover == hoverState)
            return;

        if (hover)
            hoverBeginEvent.Invoke();
        else
            hoverEndEvent.Invoke();

        hoverState = hover;
    }

    /// <summary>
    /// Method called when click is started or released (whether or not the click is cancelled by moving to another target)
    /// </summary>
    /// <param name="click"></param>
    public void GetClickState(bool click)
    {
        if (click == clickState)
            return;

        if (click)
            clickBeginEvent.Invoke();
        else
            clickEndEvent.Invoke();

        clickState = click;
    }

    /// <summary>
    /// Extra Method Called when click is released and *not* cancelled
    /// </summary>
    public void DoClickAction()
    {
        clickActionEvent.Invoke();
    }
	
}
