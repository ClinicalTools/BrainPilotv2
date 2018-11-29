using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverSender : MonoBehaviour {

    public UIElement targetElement = null;

    public bool hovering = false;

    private void Start()
    {
        targetElement = null;
        hovering = false;
    }

    public void GetTarget(UIElement newTarget)
    {
        if (newTarget == targetElement)
            return;

        Debug.Log("Get Target Called, with value = " + newTarget?.name);

        // first do a null check on new target if so we should call hover end on previous element
        if (newTarget == null)
        {
            Debug.Log("nullcheck passed");
            targetElement.hover = false;
            targetElement.DeactivateState(targetElement.hoverState);
            targetElement.hoverEvent.Invoke(false);
        } // else we have a new target so call hover begin actions
        else
        {
            if (newTarget != null)
                newTarget.hover = true;
            newTarget?.ActivateState(newTarget.hoverState);
            newTarget?.hoverEvent.Invoke(true);
        }

        targetElement = newTarget;
    }


}
