using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Higher level script to tie together UIElements' data layer and open-close actions on menu prefabs. 
/// </summary>
[RequireComponent(typeof(MenuOpenClose))]
[RequireComponent(typeof(MenuPositionController))]
[RequireComponent(typeof(GameplayEventListener))]
public class MenuInstanceGenerator : MonoBehaviour
{
    public UISelectableElement parentSelectableElement;
    public UIElementMenu uiElementMenu;
    public GameObject menuItemPrefab;

    private MenuPositionController menuPositionController;

    [ContextMenu("Generate")]
    public void Generate()
    {
        if (!uiElementMenu)
        {
            Debug.Log("Menu Instance Generator needs a UI Element Menu resource.");
            return;
        }
        if (!menuItemPrefab)
        {
            
            Debug.Log("Menu Instance Generator needs a menu item prefab.");
            return;

        }
        if (!parentSelectableElement)
            parentSelectableElement = FindParentSelectable();

        SetUpMenu(uiElementMenu);
  
        CreateMenuItems(uiElementMenu, menuItemPrefab);
    }

    [ContextMenu("Clear All")]
    public void Clear()
    {
# if UNITY_EDITOR
        foreach(Transform child in transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
#else
        foreach(Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
#endif
       
    }

    private void CreateMenuItems(UIElementMenu uiElementMenu, GameObject menuItemPrefab)
    {
        uiElementMenu.elements.ForEach(element =>
        {
            GenerateNewMenuItem(element, menuItemPrefab);
        });
    }

    private void GenerateNewMenuItem(UIElement element, GameObject menuItemPrefab)
    {
        GameObject newMenuItem = Instantiate(menuItemPrefab, transform);
        AssignSelectable(element, newMenuItem);
        AssignMenuName(element, newMenuItem);
        AssignPositionController(newMenuItem);
        SetUpTweenAction(newMenuItem);
    }

    private void AssignMenuName(UIElement element, GameObject newMenuItem)
    {
        newMenuItem.name = element.name + "_menuItem";
    }

    private void SetUpTweenAction(GameObject newMenuItem)
    {
        var tweenAction = newMenuItem.GetComponent<TweenToTransformTarget>();
        if (tweenAction)
        {
            tweenAction.originTransform = transform;
        }
    }

    /// <summary>
    /// Assigns a sub menu item's controller to it's parent which should calculate anchor positions
    /// </summary>
    /// <param name="newMenuItem"></param>
    private void AssignPositionController(GameObject newMenuItem)
    {
        var itemPosition = newMenuItem.GetComponent<MenuItemPosition>();
        if (!itemPosition)
        {
            itemPosition = newMenuItem.AddComponent<MenuItemPosition>();
        }
        itemPosition.controller = GetComponent<MenuPositionController>();
    }

    /// <summary>
    /// assign a selectable item to a wrapper for a menu item
    /// </summary>
    /// <param name="element"></param>
    /// <param name="newMenuItem"></param>
    private void AssignSelectable(UIElement element, GameObject newMenuItem)
    {
        var uiObject = newMenuItem.GetComponent<UISelectableObject>();
        if (!uiObject)
        {
            uiObject = newMenuItem.AddComponent<UISelectableObject>();
        }

        uiObject.SetNewUIElement(element);
    }

    private void SetUpMenu(UIElementMenu uiElementMenu)
    {
        

        menuPositionController = GetComponent<MenuPositionController>();
        menuPositionController.numberOfSubItems = uiElementMenu.elements.Count; // set our menu position controller to the correct number of sub items, and tell it to generate anchors
        menuPositionController.GenerateAnchors();

        if (!parentSelectableElement)
            parentSelectableElement = FindParentSelectable();
        if (!parentSelectableElement)
        {
            Debug.LogWarning(transform.name + " did not find a selectable parent object");
        }
        parentSelectableElement.clickActionEvent.AddListener(uiElementMenu.ToggleVisible);  // set the parent's click action to toggle visible state on our ui element list

    }

    private UISelectableElement FindParentSelectable()
    {
        return GetComponentInParent<UISelectableElement>();
    }
}
