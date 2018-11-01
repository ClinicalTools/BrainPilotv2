using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class UIElement : Selectable
{

    public bool hover;

    public UnityEvent hoverEvent;
    public UnityEvent clickEvent;
    public UnityEvent gazeEvent; 

    public SelectableState hoverState;
    public SelectableState clickState;
    public SelectableState gazeState;

}
