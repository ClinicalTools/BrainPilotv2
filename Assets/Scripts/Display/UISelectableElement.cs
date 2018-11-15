using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Extends SelectableElement and adds convenience layer to respond to changes in UI State (hover, click)
/// </summary>
public class UISelectableElement : SelectableElement
{

    public UnityEvent hoverBeginEvent;
    public UnityEvent hoverEndEvent;
    public UnityEvent clickBeginEvent;
    public UnityEvent clickEndEvent;

    [SerializeField]
    protected bool hoverState = false;

    [SerializeField]
    protected bool clickState = false;

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
	
}
