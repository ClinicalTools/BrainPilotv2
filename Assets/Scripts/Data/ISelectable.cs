using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable {

    void RegisterNewSelection(Selection selection);

    void RemoveSelection(Selection selection);

    void OnSelect(Selection selection);

    void OnDeselect(Selection selection);

}
