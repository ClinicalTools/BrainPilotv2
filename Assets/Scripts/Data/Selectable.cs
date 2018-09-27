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

    public List<ISelectableListener> listeners;

    public List<ISelectableState> loadedStates;

    public List<ISelectableState> activeStates;


    /// <summary>
    /// Activates the state. 
    /// </summary>
    /// <param name="state">State.</param>
    /// <param name="deactivateAllOthers">If set to <c>true</c> deactivate all others.</param>
    public void ActivateState(ISelectableState state, bool deactivateAllOthers = false)
    {
        if (activeStates == null)
        {
            activeStates = new List<ISelectableState>();
        }
        

        if (deactivateAllOthers)
        {
            activeStates.Clear();
        }
        if (!loadedStates.Contains(state))
        {
            LoadState(state);
        }
        if (!activeStates.Contains(state))
        {
            activeStates.Add(state);
        }
        UpdateListeners();
    }


    public void DeactivateState(ISelectableState state)
    {
        if (activeStates.Contains(state))
        {
            activeStates.Remove(state);
            UpdateListeners();
        }
    }

    public List<ISelectableState> GetActiveStates()
    {
        return activeStates;
    }

    public List<ISelectableListener> GetListeners()
    {
        return listeners;
    }

    public List<ISelectableState> GetLoadedStates()
    {
        return loadedStates;
    }

    public void LoadState(ISelectableState state)
    {
        if (loadedStates == null)
        {
            loadedStates = new List<ISelectableState>();
        }

        if (!loadedStates.Contains(state))
        {
            loadedStates.Add(state);
        }
    }

    public void RegisterListener(ISelectableListener listener)
    {
        if(listeners == null)
        {
            listeners = new List<ISelectableListener>();
        }

        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnloadState(ISelectableState state)
    {
        if (loadedStates == null)
        {
            loadedStates = new List<ISelectableState>();
        }

        if (loadedStates.Contains(state))
        {
            loadedStates.Remove(state);
        }
    }

    public void UnregisterListener(ISelectableListener listener)
    {
        if (listeners == null)
        {
            listeners = new List<ISelectableListener>();
        }

        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }

    protected void UpdateListeners()
    {
        foreach(var listener in listeners)
        {
            listener.SelectableUpdated();
        }
    }


}
