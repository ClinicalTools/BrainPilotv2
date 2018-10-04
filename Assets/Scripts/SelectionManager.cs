using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class SelectionManager : ScriptableObject
{

    public List<Selectable> allSelectables;

    public List<SelectionGroup> allSelectionGroups;

    public List<SelectableState> allSelectionStates;

    public void LoadAssetList()
    {
        FindAllSelectables();
        FindAllSelectionGroups();
        FindAllSelectionStates();
    }

    public void LoadStateOnGroup(SelectableState state, SelectionGroup group)
    {
        group.LoadState(state);
    }

    public void UnloadStateOnGroup(SelectableState state, SelectionGroup group)
    {
        group.UnloadState(state);
    }

    public void ActivateStateOnGroup(SelectableState state, SelectionGroup group)
    {
        group.ActivateState(state);
    }

    public void DeactivateStateOnGroup(SelectableState state, SelectionGroup group)
    {
        group.DeactivateState(state);
    }

    public void ActivateStateOnInverse(SelectableState state, SelectionGroup group)
    {
        var inverseOfGroup = new SelectionGroup
        {
            selectables = new List<Selectable>(allSelectables.FindAll(selectable => !group.selectables.Contains(selectable)))
        };
        ActivateStateOnGroup(state, inverseOfGroup);
    }

    public void LoadStateOnInverse(SelectableState state, SelectionGroup group)
    {
        var inverseOfGroup = new SelectionGroup
        {
            selectables = new List<Selectable>(allSelectables.FindAll(selectable => !group.selectables.Contains(selectable)))
        };
        LoadStateOnGroup(state, inverseOfGroup);
    }

    public void UnloadStateOnInverse(SelectableState state, SelectionGroup group)
    {
        var inverseOfGroup = new SelectionGroup
        {
            selectables = new List<Selectable>(allSelectables.FindAll(selectable => !group.selectables.Contains(selectable)))
        };
        UnloadStateOnGroup(state, inverseOfGroup);
    }


    public void DeactiveateStateOnInverse(SelectableState state, SelectionGroup group)
    {
        var inverseOfGroup = new SelectionGroup
        {
            selectables = new List<Selectable>(allSelectables.FindAll(selectable => !group.selectables.Contains(selectable)))
        };
        DeactivateStateOnGroup(state, inverseOfGroup);
    }

    public void ApplyStateToAll(SelectableState state)
    {
        var all = new SelectionGroup
        {
            selectables = new List<Selectable>(allSelectables)
        };
        ActivateStateOnGroup(state, all);
    }

    [ContextMenu("Find All Selectables")]
    void FindAllSelectables()
    {
        allSelectables = new List<Selectable>(Resources.FindObjectsOfTypeAll<Selectable>());
    }

    [ContextMenu("Find All Selection Groups")]
    void FindAllSelectionGroups()
    {
        allSelectionGroups = new List<SelectionGroup>(Resources.FindObjectsOfTypeAll<SelectionGroup>());
    }

    [ContextMenu("Find All Selection States")]
    void FindAllSelectionStates()
    {
        allSelectionStates = new List<SelectableState>(Resources.FindObjectsOfTypeAll<SelectableState>());
    }


}
