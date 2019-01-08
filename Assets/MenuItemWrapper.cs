using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;

public class MenuItemWrapper : MonoBehaviour 
{

	[Tooltip("Click Action -- will find and connect to all children with a UI Element Component")]
	public UnityEvent clickActionEvent;

	[SerializeField]
	private List<UISelectableElement> childElements;

	void Start()
	{
		
		SetupChildElementClickActions();
	}

/// <summary>
/// Link all children UI Selectable Elements Click Actions to Trigger the 'main' click action -- this is useful because we are creating compound components with 3D icons and screens and both should be connected
/// </summary>
    private void SetupChildElementClickActions()
    {
        childElements = new List<UISelectableElement>(gameObject.GetComponentsInChildren<UISelectableElement>());

		foreach (var element in childElements) 
		{
			element.clickActionEvent.AddListener(clickActionEvent.Invoke);
		}


    }
}
