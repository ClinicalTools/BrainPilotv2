using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct TimedEvent
{
    public float delay;
    public UnityEvent unityEvent;
}

public class EventSeries : MonoBehaviour {

    public List<TimedEvent> events;

    public void TriggerEvents()
    {
        StartCoroutine(RunEventCycle());
    }

    protected IEnumerator RunEventCycle()
    {
        yield return null;
    }

}
