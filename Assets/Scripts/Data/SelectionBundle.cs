using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBundle : ScriptableObject {

    public SelectionBundle(SelectionGroup group, List<SelectableState> states, List<SelectableState> inverse)
    {
        selectionGroup = group;
        loadedStates = states;
        inverseLoadedStates = inverse;
    }

    public SelectionGroup selectionGroup;

    public List<SelectableState> loadedStates;

    public List<SelectableState> inverseLoadedStates;

}
