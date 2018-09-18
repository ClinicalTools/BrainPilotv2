using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class SelectionEvent : UnityEvent<Selection> { }

public class Selectable : MonoBehaviour, ISelectable
{

    public bool selected;

    public List<Selection> selections;

    public Selection lastSelectedBy;

    public SelectionEvent onSelectEvent;

    public SelectionEvent onDeselectEvent;

    private void OnEnable()
    {
        if (selections == null)
            return;
        foreach(var selection in selections)
        {
            selection.RegisterSelectable(this);
        }
    }

    private void OnDisable()
    {
        if (selections == null)
            return;
        foreach (var selection in selections)
        {
            selection.DeregisterSelectable(this);
        }
    }

    public virtual void OnDeselect(Selection selection)
    {
        selected = false;
        onDeselectEvent.Invoke(selection);
    }

    public virtual void OnSelect(Selection selection)
    {
        selected = true;
        lastSelectedBy = selection;
        onSelectEvent.Invoke(selection);
    }

    public void RegisterNewSelection(Selection selection)
    {
        if (selections == null)
            selections = new List<Selection>();

        if (!selections.Contains(selection))
        {
            selections.Add(selection);
        }
    }

    public void RemoveSelection(Selection selection)
    {
        if (selections.Contains(selection))
        {
            selections.Remove(selection);
        }
    }
}
