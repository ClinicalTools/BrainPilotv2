using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SelectionGroup : ScriptableObject, ISelectionGroup
{

    public List<Selectable> selectables;

    public List<SelectableState> loadedStates;
    public List<SelectableState> activeStates;

    public void ActivateState(SelectableState state, bool deactivateAllOthers = false)
    {
        if (selectables == null)
            return;

        if (activeStates == null)
            activeStates = new List<SelectableState>();

        foreach(Selectable selectable in selectables)
        {
            selectable.ActivateState(state, deactivateAllOthers);
        }
        if (!activeStates.Contains(state))
        {
            activeStates.Add(state);
        }
    }

    public void DeactiveateAll()
    {
        foreach(var state in activeStates)
        {
            DeactivateState(state);
        }
        activeStates.Clear();
    }

    public void UnloadAll()
    {
        for(int i = loadedStates.Count - 1; i >= 0; i--)
        {
            var state = loadedStates[i];
            DeactivateState(state);
            UnloadState(state);
        }
    }

    public void DeactivateState(SelectableState state)
    {
        if (selectables == null)
            return;

        if(activeStates == null)
        {
            activeStates = new List<SelectableState>();
        }

        foreach(Selectable selectable in selectables)
        {
            selectable.DeactivateState(state);
            
        }
        if (activeStates.Contains(state))
        {
            activeStates.Remove(state);
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

        if (loadedStates == null)
            loadedStates = new List<SelectableState>();

        foreach (Selectable selectable in selectables)
        {
            selectable.LoadState(state);
        }
        if (!loadedStates.Contains(state))
        {
            loadedStates.Add(state);
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

        if (loadedStates == null)
            loadedStates = new List<SelectableState>();

        foreach (Selectable selectable in selectables)
        {
            selectable.UnloadState(state);
        }
        if (loadedStates.Contains(state))
        {
            loadedStates.Remove(state);
        }
    }
}
