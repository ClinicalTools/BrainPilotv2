using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ClickUpdateEvent : UnityEvent<bool> { }

public class ClickInput : MonoBehaviour
{

    public FloatResource triggerResource;

    [Range(0f, 1f)]
    public float clickThreshhold;

    public bool isActive;
    public bool clickDown;

    public ClickUpdateEvent clickUpdateEvent;

    private void Start()
    {
        clickDown = false;
    }

    private void Update()
    {
        if (isActive)
        {
            bool click = triggerResource.Value >= clickThreshhold;
            if (click != clickDown)
            {
                clickUpdateEvent.Invoke(click);
                clickDown = click;
            }
            
        }
    }


}
