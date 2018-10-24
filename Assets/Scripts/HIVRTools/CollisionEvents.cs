using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvents : MonoBehaviour
{
    // just takes the standard unity messages and turns them into events & delegates for behaviors to subscript to on their own

    public UnityEvent CollisionEnterUnityEvent;
    public delegate void CollisionEnterDelegate(CollisionEvents events, Collision collision);
    public event CollisionEnterDelegate CollisionEnterEvent;

    public UnityEvent CollisionExitUnityEvent;
    public delegate void CollisionExitDelegate(CollisionEvents events, Collision collision);
    public event CollisionExitDelegate CollisionExitEvent;

    public UnityEvent TriggerEnterUnityEvent;
    public delegate void TriggerEnterDelegate(CollisionEvents events, Collider other);
    public event TriggerEnterDelegate TriggerEnterEvent;

    public UnityEvent TriggerExitUnityEvent;
    public delegate void TriggerExitDelegate(CollisionEvents events, Collider other);
    public event TriggerExitDelegate TriggerExitEvent;

    protected virtual void OnCollisionEnter(Collision collision)
    {
        CallCollisionEntered(collision);
    }

    protected virtual void CallCollisionEntered(Collision collision)
    {
        CollisionEnterUnityEvent.Invoke();
        if (CollisionEnterEvent != null)
            CollisionEnterEvent(this, collision);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        TriggerEnterUnityEvent.Invoke();
        TriggerEnterEvent?.Invoke(this, other);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        TriggerExitUnityEvent.Invoke();
        TriggerExitEvent?.Invoke(this, other);
    }

    protected virtual void OnCollisionExit(Collision collision)
    {
        CallCollisionExit(collision);
    }

    protected virtual void CallCollisionExit(Collision collision)
    {
        CollisionExitUnityEvent.Invoke();
        if (CollisionExitEvent != null)
            CollisionExitEvent(this, collision);
    }
}
