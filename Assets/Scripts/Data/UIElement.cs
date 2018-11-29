using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UIBoolEvent : UnityEvent<bool> { }

[CreateAssetMenu]
public class UIElement : Selectable
{

    public bool hover;
    public bool click;
    public bool gaze;

    public UIBoolEvent hoverEvent;
    public UIBoolEvent clickEvent;
    public UIBoolEvent gazeEvent; 

    public SelectableState hoverState;
    public SelectableState clickState;
    public SelectableState gazeState;

}
