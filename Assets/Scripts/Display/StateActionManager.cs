using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StateActionManager : MonoBehaviour 
{

    private void OnEnable()
    {
        Reset();
    }

    [ContextMenu("Reset")]
    private void Reset()
    {
        UnloadState(null);
        // this will remove all states left over that have lost their state property reference

    }

    /// <summary>
    /// Loads the state's prefab GameObjects as children of this GameObject, and runs Load() on all SelectableStateAction scripts on the child.
    /// </summary>
    /// <param name="state">Selectable State Asset</param>
    public void LoadState(SelectableState state)
    {

        if (state == null)
            return;

        var actions = new List<SelectableStateAction>();

        var prefabs = state.GetPrefabs();

        // Iterate through all the prefabs inside the state, load them and combine their actions in a list
        foreach(var prefab in prefabs)
        {
            var child = Instantiate(prefab, transform);
            var actionsOnPrefab = child.GetComponents<SelectableStateAction>();

            if (actionsOnPrefab == null)
            {
                Debug.Log("No actions were found on loaded prefab " + child.name);
            }
            else
            {
                actions.AddRange(actionsOnPrefab);
            }
        }

        // Call load on each action
        foreach(var action in actions)
        {
            action.State = state;
            action.Load();
        }

    }

    /// <summary>
    /// Unloads the state, but will not remove a GameObject (this should be done by the state actions!)
    /// </summary>
    /// <param name="state">State.</param>
    public void UnloadState(SelectableState state)
    {
        DeactivateState(state);
        var actions = new List<SelectableStateAction>(GetComponentsInChildren<SelectableStateAction>());
        actions.FindAll(action => action.State == state).ForEach(action => action.Remove());
    }

    /// <summary>
    /// Activates a state on an element, sending a message to all the associated actions previously loaded by that <paramref name="state"/>.
    /// </summary>
    /// <param name="state">State.</param>
    public void ActivateState(SelectableState state)
    {
        var actions = new List<SelectableStateAction>(GetComponentsInChildren<SelectableStateAction>());
        actions.FindAll(action => action.State == state).ForEach(action => action.Activate());
    }

    /// <summary>
    /// Deactivates a state if it was active. 
    /// </summary>
    /// <param name="state">State.</param>
    public void DeactivateState(SelectableState state)
    {
        var actions = new List<SelectableStateAction>(GetComponentsInChildren<SelectableStateAction>());
        actions.FindAll(action => action.State == state).ForEach(action => action.Deactivate());
    }




}
