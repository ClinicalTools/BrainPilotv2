using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuItemOpenClose : MonoBehaviour {

    public UnityEvent openActions;
    public UnityEvent closeActions;

    public bool menuActiveState;

    public MenuOpenClose menuParent;

    private void Start()
    {
        SubscribeEvents();

        if (menuActiveState)
            openActions.Invoke();
        else
            closeActions.Invoke();
    }

    private void SubscribeEvents()
    {
        if (menuParent == null)
            menuParent = GetComponentInParent<MenuOpenClose>();

        menuParent.menuOpenEvent.AddListener(OpenActions);
        menuParent.menuCloseEvent.AddListener(CloseActions);
        menuActiveState = menuParent.menuActiveState;
    }

    private void CloseActions()
    {
        if (menuActiveState)
        {
            closeActions.Invoke();
            menuActiveState = false;
        }
    }

    private void OpenActions()
    {
        if (!menuActiveState)
        {
            openActions.Invoke();
            menuActiveState = true;
        }
    }
}
