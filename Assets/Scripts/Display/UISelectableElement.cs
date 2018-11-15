using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UISelectableElement : SelectableElement
{

    public UIElement uiSelectable;

    public UnityEvent hoverBeginEvent;
    public UnityEvent hoverEndEvent;
    public UnityEvent clickBeginEvent;
    public UnityEvent clickEndEvent;
    public UnityEvent clickActionEvent;

    protected bool hoverState;
    protected bool clickState;

    protected override void Awake()
    {
        base.Awake();

        uiSelectable = selectable as UIElement;
        uiSelectable.hoverEvent.AddListener(GetHover);
        uiSelectable.clickEvent.AddListener(GetClick);
        uiSelectable.onClickAction.AddListener(DoClickAction);

        hoverState = false;
        clickState = false;
    }

    /// <summary>
    /// Method called on hover begin or end
    /// </summary>
    /// <param name="hover"></param>
    public void GetHover(bool hover)
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
    public void GetClick(bool click)
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
