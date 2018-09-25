using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void LoadState(SelectableState state);

    void UnloadState(SelectableState state);

    void ActivateState(SelectableState state, bool deactivateAllOthers = false);

    void DeactivateState(SelectableState state);

    List<SelectableState> GetActiveStates();

}
