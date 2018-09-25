using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectionGroup
{

    void RegisterSelectable(ISelectable selectable);

    void DeregisterSelecable(ISelectable selectable);

    void LoadState(ISelectableState state);

    void UnloadState(ISelectableState state);

    void ActivateState(ISelectableState state, bool deactivateAllOthers = false);

    void DeactivateState(ISelectableState state);

}
