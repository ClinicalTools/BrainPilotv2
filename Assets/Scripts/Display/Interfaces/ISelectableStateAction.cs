using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for loading and activating effects on a range of unknown objects in a scene.
/// </summary>
public interface ISelectableStateAction
{

    void Load();

    void Remove();

    void Activate();

    void Deactivate();

    SelectableState State
    {
        get;
        set;
    }

}
