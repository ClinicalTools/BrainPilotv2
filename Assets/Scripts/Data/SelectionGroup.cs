using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SelectionGroup : ScriptableObject, ISelectionGroup
{

    public List<ISelectable> selectables;

    public void ActivateState(ISelectableState state, bool deactivateAllOthers = false)
    {
        if (selectables == null)
            return;

        foreach(Selectable selectable in selectables)
        {
            selectable.ActivateState(state, deactivateAllOthers);
        }
    }

    public void DeactivateState(ISelectableState state)
    {
        if (selectables == null)
            return;

        foreach(Selectable selectable in selectables)
        {
            selectable.DeactivateState(state);
        }
    }

    public void DeregisterSelecable(ISelectable selectable)
    {
        if (selectables == null)
        {
            selectables = new List<ISelectable>();
        }

        if (selectables.Contains(selectable))
        {
            selectables.Remove(selectable);
        }
    }

    public void LoadState(ISelectableState state)
    {
        if (selectables == null)
            return;

        foreach (Selectable selectable in selectables)
        {
            selectable.LoadState(state);
        }
    }

    public void RegisterSelectable(ISelectable selectable)
    {
        if (selectables == null)
        {
            selectables = new List<ISelectable>();
        }

        if (!selectables.Contains(selectable))
        {
            selectables.Add(selectable);
        }

    }

    public void UnloadState(ISelectableState state)
    {
        if (selectables == null)
            return;

        foreach (Selectable selectable in selectables)
        {
            selectable.UnloadState(state);
        }
    }
}
