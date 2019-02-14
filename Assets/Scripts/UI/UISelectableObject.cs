using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility Class that wraps all child UI Selectable Element Monobehaviors.
/// Syncs up all children to use the same UISelectable data asset.
/// </summary>
public class UISelectableObject : MonoBehaviour {

    public UIElement uiElement;

    private void OnValidate()
    {
        if (uiElement)
            SetAllChildrenUIElements();
    }

    public void SetNewUIElement(UIElement newElement)
    {
        uiElement = newElement;
        SetAllChildrenUIElements();
    }

    [ContextMenu("SetAllChildren")]
    private void SetAllChildrenUIElements()
    {
        var uiList = GetComponentsInChildren<UISelectableElement>();

        foreach (var uiSelectableElement in uiList)
        {
            uiSelectableElement.selectable = uiElement;
        }
    }

}
