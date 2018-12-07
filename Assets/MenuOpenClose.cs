using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class MenuOpenClose : MonoBehaviour {

    public UnityEvent menuOpenEvent;
    public UnityEvent menuCloseEvent;

    public bool menuIsOpen;

    public void ToggleMenu()
    {
        SetMenuStatus(!menuIsOpen);
    }

    public void SetMenuStatus(bool status)
    {
        if (menuIsOpen == status)
            return;

        if (menuIsOpen)
            menuCloseEvent.Invoke();
        else
            menuOpenEvent.Invoke();

        menuIsOpen = status;
    }

}
