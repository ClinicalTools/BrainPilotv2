using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object that wraps a list of prefabs, intended to be loaded as SelectableStateActions. Also serves as an 'unlimted enum' that can be checked for equivalence.
/// </summary>
/// 
[CreateAssetMenu]
public class SelectableState : ScriptableObject, ISelectableState
{

    public List<GameObject> actions;

}
