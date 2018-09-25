using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[Serializable]
public class Selectable : ScriptableObject, ISelectable
{

    public List<SelectableListener> listeners;

    public List<SelectableState> loadedStates;

    public List<SelectableState> activeStates;



    public void ActivateState(SelectableState state, bool deactivateAllOthers = false)
    {
        throw new System.NotImplementedException();
    }

    public void DeactivateState(SelectableState state)
    {
        throw new System.NotImplementedException();
    }

    public List<SelectableState> GetActiveStates()
    {
        throw new System.NotImplementedException();
    }

    public List<SelectableListener> GetListeners()
    {
        throw new System.NotImplementedException();
    }

    public List<SelectableState> GetLoadedStates()
    {
        throw new System.NotImplementedException();
    }

    public void LoadState(SelectableState state)
    {
        throw new System.NotImplementedException();
    }

    public void RegisterListener(SelectableListener listener)
    {
        throw new System.NotImplementedException();
    }

    public void UnloadState(SelectableState state)
    {
        throw new System.NotImplementedException();
    }

    public void UnregisterListener(SelectableListener listener)
    {
        throw new System.NotImplementedException();
    }
}
