using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateActionManager : MonoBehaviour 
{

    public Dictionary<ISelectableState, List<SelectableStateAction>> loadedStateDictionary;
    public Dictionary<ISelectableState, List<SelectableStateAction>> activeStateDictionary;


    /// <summary>
    /// Loads the state's prefab GameObjects as children of this GameObject, and runs Load() on all SelectableStateAction scripts on the child.
    /// </summary>
    /// <param name="state">Selectable State Asset</param>
    public void LoadState(ISelectableState state)
    {
        if (loadedStateDictionary == null)
        {
            loadedStateDictionary = new Dictionary<ISelectableState, List<SelectableStateAction>>();
        }

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
            action.Load();
        }

        // index these actions
        loadedStateDictionary.Add(state, actions);
    }

    /// <summary>
    /// Unloads the state, but will not remove a GameObject (this should be done by the state actions!)
    /// </summary>
    /// <param name="state">State.</param>
    public void UnloadState(ISelectableState state)
    {
        if (loadedStateDictionary.ContainsKey(state))
        {
            foreach (var action in loadedStateDictionary[state])
            {
                action.Remove();
            }
            loadedStateDictionary.Remove(state);
        }
    }

    /// <summary>
    /// Activates a state on an element, sending a message to all the associated actions previously loaded by that <paramref name="state"/>.
    /// </summary>
    /// <param name="state">State.</param>
    public void ActivateState(ISelectableState state)
    {
        if (!loadedStateDictionary.ContainsKey(state))
        {
            Debug.LogWarning("A state was called on ActiveateState but has not been loaded! Nothing will happen.");
            return;
        }

        if (activeStateDictionary == null)
        {
            activeStateDictionary = new Dictionary<ISelectableState, List<SelectableStateAction>>();
        }

        activeStateDictionary.Add(state, loadedStateDictionary[state]);

        foreach(var action in activeStateDictionary[state])
        {
            action.Activate();
        }

    }

    /// <summary>
    /// Deactivates a state if it was active. 
    /// </summary>
    /// <param name="state">State.</param>
    public void DeactivateState(ISelectableState state)
    {
        if (!activeStateDictionary.ContainsKey(state))
        {
            Debug.Log("A state was told to deactivate but it was not active. Nothing will happen");
            return;
        }

        foreach(var action in activeStateDictionary[state])
        {
            action.Deactivate();
        }

        activeStateDictionary.Remove(state);
    }


}
