using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionGroup : ScriptableObject, ISelectionGroup
{

    public List<Selectable> selectables;

    public void ActivateState(ISelectableState state, bool deactivateAllOthers = false)
    {
        throw new System.NotImplementedException();
    }

    public void DeactivateState(ISelectableState state)
    {
        throw new System.NotImplementedException();
    }

    public void DeregisterSelecable(ISelectable selectable)
    {
        throw new System.NotImplementedException();
    }

    public void LoadState(ISelectableState state)
    {
        throw new System.NotImplementedException();
    }

    public void RegisterSelectable(ISelectable selectable)
    {
        throw new System.NotImplementedException();
    }

    public void UnloadState(ISelectableState state)
    {
        throw new System.NotImplementedException();
    }
}
