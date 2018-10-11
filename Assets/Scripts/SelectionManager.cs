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

    public SelectionBundle currentBundle;

    public void LoadNewBundle(SelectionBundle bundle)
    {
        currentBundle = bundle;
        var group = bundle.selectionGroup;
        foreach(var state in bundle.loadedStates)
        {
            LoadStateOnGroup(state, group);
        }
        foreach (var state in bundle.inverseLoadedStates)
        {
            LoadStateOnInverse(state, group);
        }
    }

    public void ApplyNewBundle(SelectionBundle bundle)
    {
        if (bundle != currentBundle)
        {
            LoadNewBundle(bundle);
        }
        var group = bundle.selectionGroup;
        foreach (var state in bundle.activeStates)
        {
            ActivateStateOnGroup(state, group);
        }
        foreach (var state in bundle.inverseActiveStates)
        {
            ActivateStateOnInverse(state, group);
        }
    }

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
        var inverseOfGroup = ScriptableObject.CreateInstance<SelectionGroup>();
        inverseOfGroup.selectables = new List<Selectable>(allSelectables.FindAll(selectable => !group.selectables.Contains(selectable)));

        ActivateStateOnGroup(state, inverseOfGroup);
    }

    public void LoadStateOnInverse(SelectableState state, SelectionGroup group)
    {
        var inverseOfGroup = ScriptableObject.CreateInstance<SelectionGroup>();

        inverseOfGroup.selectables = new List<Selectable>(allSelectables.FindAll(selectable => !group.selectables.Contains(selectable)));

        LoadStateOnGroup(state, inverseOfGroup);
    }

    public void UnloadStateOnInverse(SelectableState state, SelectionGroup group)
    {
        var inverseOfGroup = ScriptableObject.CreateInstance<SelectionGroup>();

        inverseOfGroup.selectables = new List<Selectable>(allSelectables.FindAll(selectable => !group.selectables.Contains(selectable)));
        
        UnloadStateOnGroup(state, inverseOfGroup);
    }


    public void DeactiveateStateOnInverse(SelectableState state, SelectionGroup group)
    {
        var inverseOfGroup = ScriptableObject.CreateInstance<SelectionGroup>();

        inverseOfGroup.selectables = new List<Selectable>(allSelectables.FindAll(selectable => !group.selectables.Contains(selectable)));
        
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

    public void ResetAllSelectables()
    {
        FindAllSelectables();
        allSelectables.ForEach(selectable => selectable.ResetAll());
    }

    public void ResetAllGroups()
    {
        FindAllSelectionGroups();
        allSelectionGroups.ForEach(group => group.UnloadAll());
    }

    public void ClearAllStates(SelectionGroup group)
    {
        group?.DeactiveateAll();
        group?.UnloadAll();
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
