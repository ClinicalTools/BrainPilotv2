using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SelectionGroup : ScriptableObject, ISelectionGroup
{

    public List<Selectable> selectables;

    public void ActivateState(SelectableState state, bool deactivateAllOthers = false)
    {
        if (selectables == null)
            return;

        foreach(Selectable selectable in selectables)
        {
            selectable.ActivateState(state, deactivateAllOthers);
        }
    }

    public void DeactivateState(SelectableState state)
    {
        if (selectables == null)
            return;

        foreach(Selectable selectable in selectables)
        {
            selectable.DeactivateState(state);
            
        }
    }

    public void DeregisterSelecable(Selectable selectable)
    {
        if (selectables == null)
        {
            selectables = new List<Selectable>();
        }

        if (selectables.Contains(selectable))
        {
            selectables.Remove(selectable);
        }
    }

    public void LoadState(SelectableState state)
    {
        if (selectables == null)
            return;

        foreach (Selectable selectable in selectables)
        {
            selectable.LoadState(state);
        }
    }

    public void RegisterSelectable(Selectable selectable)
    {
        if (selectables == null)
        {
            selectables = new List<Selectable>();
        }

        if (!selectables.Contains(selectable))
        {
            selectables.Add(selectable);
        }

    }

    public void UnloadState(SelectableState state)
    {
        if (selectables == null)
            return;

        foreach (Selectable selectable in selectables)
        {
            selectable.UnloadState(state);
        }
    }
}
