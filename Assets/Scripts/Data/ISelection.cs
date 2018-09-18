using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelection {

    void SetSelect(Selection selection);

    bool IsSelected(Selection selection);

}
