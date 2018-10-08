using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Selectable asset. Listeners should subscribe to get updates. Models selection states only. Will call UpdateListeners on every change, so the listener side
/// should manage how often the view is actually updated. 
/// </summary>

[CreateAssetMenu]
[Serializable]
public class Selectable : ScriptableObject, ISelectable
{

    public List<SelectableListener> listeners;

    public List<SelectableState> loadedStates;

    public List<SelectableState> activeStates;






    /// <summary>
    /// Activates the state. 
    /// </summary>
    /// <param name="state">State.</param>
    /// <param name="deactivateAllOthers">If set to <c>true</c> deactivate all others.</param>
    public void ActivateState(SelectableState state, bool deactivateAllOthers = false)
    {
        if (activeStates == null)
        {
            activeStates = new List<SelectableState>();
        }
        

        if (deactivateAllOthers)
        {
            activeStates.Clear();
        }
        if (!loadedStates.Contains(state as SelectableState))
        {
            LoadState(state);
        }
        if (!activeStates.Contains(state as SelectableState))
        {
            activeStates.Add(state as SelectableState);
        }
        UpdateListeners();
    }

    public void ResetAll()
    {
        activeStates.Clear();
        loadedStates.Clear();
        UpdateListeners();
    }

    public void DeactivateState(SelectableState state)
    {
        if (activeStates.Contains(state))
        {
            activeStates.Remove(state);
            
        }
        UpdateListeners();
    }

    public List<SelectableState> GetActiveStates()
    {
        return activeStates;
    }

    public List<SelectableListener> GetListeners()
    {
        return listeners;
    }

    public List<SelectableState> GetLoadedStates()
    {
        return loadedStates;
    }

    public void LoadState(SelectableState state)
    {
        if (loadedStates == null)
        {
            loadedStates = new List<SelectableState>();
        }

        if (!loadedStates.Contains(state))
        {
            loadedStates.Add(state);
        }
        UpdateListeners();
    }

    public void RegisterListener(SelectableListener listener)
    {
        if(listeners == null)
        {
            listeners = new List<SelectableListener>();
        }

        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnloadState(SelectableState state)
    {
        if (loadedStates == null)
        {
            loadedStates = new List<SelectableState>();
        }

        if (activeStates.Contains(state))
        {
            activeStates.Remove(state);
            UpdateListeners();
        }

        if (loadedStates.Contains(state))
        {
            loadedStates.Remove(state);
            
        }
        UpdateListeners();
    }

    public void UnregisterListener(SelectableListener listener)
    {
        if (listeners == null)
        {
            listeners = new List<SelectableListener>();
        }

        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }

    protected void UpdateListeners()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            if (listeners[i] == null)
                listeners.RemoveAt(i);

            listeners[i].SelectableUpdated();
        }
    }


}
