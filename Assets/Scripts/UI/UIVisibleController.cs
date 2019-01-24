using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller class for a UI SelectableElement's visible state. Call methods here to set the visible state from Runtime. 
/// </summary>
[RequireComponent(typeof(UISelectableElement))]
public class UIVisibleController : MonoBehaviour {

    public bool defaultVisibleState = true;
    public UIElement elementData;

	
	void OnEnable () {
        SetVisibleState(defaultVisibleState);
	}
	
	public void SetVisibleState(bool state)
    {
        elementData = elementData ?? GetComponent<UISelectableElement>().uiSelectable;

        elementData.visible = state;
        elementData.visibleEvent.Invoke(state);
    }
}
