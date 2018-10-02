using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBundle : ScriptableObject {

    public SelectionBundle(SelectionGroup group, List<SelectableState> states, List<SelectableState> inverse)
    {
        selectionGroup = group;
        selectableStates = states;
        inverseStates = inverse;
    }

    public SelectionGroup selectionGroup;

    public List<SelectableState> selectableStates;

    public List<SelectableState> inverseStates;

}
