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

    public void Setup(SelectionGroup group, List<SelectableState> states, List<SelectableState> inverse, List<SelectableState>active, List<SelectableState> invActive)
    {
        selectionGroup = group;
        loadedStates = states;
        inverseLoadedStates = inverse;
        activeStates = active;
        inverseActiveStates = invActive;
    }

    public SelectionGroup selectionGroup;

    public List<SelectableState> loadedStates;

    public List<SelectableState> inverseLoadedStates;

    public List<SelectableState> activeStates;

    public List<SelectableState> inverseActiveStates;

}
