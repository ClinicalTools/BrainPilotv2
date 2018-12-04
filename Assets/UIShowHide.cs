using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Very simple Utility class for exposing UnityEvents to OnShow and OnHide to be called by a Menu or other UI element
/// </summary>
public class UIShowHide : MonoBehaviour {

    public UnityEvent showEvent;
    public UnityEvent hideEvent;

    public bool visible = false;

    [ContextMenu("Show")]
    public void OnShow()
    {
        showEvent.Invoke();
        visible = true;
    }

    [ContextMenu("Hide")]
    public void OnHide()
    {
        hideEvent.Invoke();
        visible = false;
    }

    [ContextMenu("Toggle")]
    public void Toggle()
    {
        if (visible)
            OnHide();
        else
            OnShow();
    }

}
