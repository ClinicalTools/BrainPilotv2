using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base interface for objects that can pass states (lists of effects) to their listeners (game objects).
/// </summary>
public interface ISelectable
{
    /// <summary>
    /// Register a listener to get updates from the selectable asset.  
    /// </summary>
    /// <param name="listener"></param>
    void RegisterListener(SelectableListener listener);

    /// <summary>
    /// Unregister a listener from a selectable.
    /// </summary>
    /// <param name="listener"></param>
    void UnregisterListener(SelectableListener listener);

    /// <summary>
    /// Load a selectablestate's actions as a child gameobject of it's SelectableElement. Does not activate.
    /// </summary>
    /// <param name="state"></param>
    void LoadState(SelectableState state);

    /// <summary>
    /// Remove a selectablestate's actions from the available states. 
    /// </summary>
    /// <param name="state"></param>
    void UnloadState(SelectableState state);

    /// <summary>
    /// Activate a state on the given selectable. Ideally should be pre-loaded to save elecution time. 
    /// </summary>
    /// <param name="state"></param>
    /// <param name="deactivateAllOthers">If true will set all active states on the selection to deactive *before* activating.</param>
    void ActivateState(SelectableState state, bool deactivateAllOthers = false);

    /// <summary>
    /// Deactivates a given state on the selectable.
    /// </summary>
    /// <param name="state"></param>
    void DeactivateState(SelectableState state);

    /// <summary>
    /// Returns a list of Active States.
    /// </summary>
    /// <returns></returns>
    List<SelectableState> GetActiveStates();

    /// <summary>
    /// Returns a list of loaded states.
    /// </summary>
    /// <returns></returns>
    List<SelectableState> GetLoadedStates();

    /// <summary>
    /// Returns a list of all listeners curretly active on this state.
    /// </summary>
    /// <returns></returns>
    List<SelectableListener> GetListeners();
}
