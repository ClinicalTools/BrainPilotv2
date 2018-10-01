using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectionGroup
{

    void RegisterSelectable(Selectable selectable);

    void DeregisterSelecable(Selectable selectable);

    void LoadState(SelectableState state);

    void UnloadState(SelectableState state);

    void ActivateState(SelectableState state, bool deactivateAllOthers = false);

    void DeactivateState(SelectableState state);

}
