using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayEventListener : MonoBehaviour {

    public GameplayEvent gameplayEvent;
    public UnityEvent eventRaised;

	public virtual void OnEventRaised()
    {
        eventRaised.Invoke();
    }

    protected void OnEnable()
    {
        gameplayEvent?.RegisterListener(this);
    }

    protected void OnDisable()
    {
        gameplayEvent?.UnregisterListener(this);
    }
}
