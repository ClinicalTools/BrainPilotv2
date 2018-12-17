using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class MenuOpenClose : MonoBehaviour {

    public UnityEvent menuOpenEvent;
    public UnityEvent menuCloseEvent;

    public bool menuActiveState;

    public void ToggleMenu()
    {
        SetMenuStatus(!menuActiveState);
    }

    public void SetMenuStatus(bool status)
    {
        if (menuActiveState == status)
            return;

        if (menuActiveState)
            menuCloseEvent.Invoke();
        else
            menuOpenEvent.Invoke();

        menuActiveState = status;
    }

}
