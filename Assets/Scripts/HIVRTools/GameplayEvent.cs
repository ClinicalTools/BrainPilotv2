using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameplayEvent : ScriptableObject
{

    public List<GameplayEventListener> listeners;

    public virtual void RaiseEvent()
    {
        //Debug.Log(this.name + " was raised.");
        for (int i = listeners.Count -1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }

    public virtual void Initialize()
    {

    }

    public void RegisterListener(GameplayEventListener listener)
    {
        if (listeners == null)
            listeners = new List<GameplayEventListener>();

        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnregisterListener(GameplayEventListener listener)
    {
        if (listeners == null)
            return;
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }


}
