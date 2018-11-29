using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Extends SelectableElement and adds convenience layer to respond to changes in UI State (hover, click)
/// </summary>
public class UISelectableElement : SelectableElement
{

    public UnityEvent hoverEvent;
    public UnityEvent hoverEndEvent;
    public UnityEvent clickEvent;
    public UnityEvent endClickEvent;

    public void GetHover(bool hover)
    {

    }

    public void GetClick(bool click)
    {

    }
	
}
