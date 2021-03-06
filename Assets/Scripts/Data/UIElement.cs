﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UIBoolEvent : UnityEvent<bool> { }

[CreateAssetMenu]
public class UIElement : Selectable
{

    public string label;

    public bool hover;
    public bool click;
    public bool gaze;
    public bool visible;

    public UIBoolEvent hoverEvent;
    public UIBoolEvent clickEvent;
    public UIBoolEvent gazeEvent;
    public UIBoolEvent visibleEvent;
    public UnityEvent onClickAction;

    public SelectableState hoverState;
    public SelectableState clickState;
    public SelectableState gazeState;

}
