using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TimedEvent
{
    public float delay;
    public UnityEvent unityEvent;
}

public class EventSeries : MonoBehaviour {

    public List<TimedEvent> events;

    /// <summary>
    /// Triggers a new Event Cycle. Will stop any cycles currently in progress.
    /// </summary>
    [ContextMenu("Trigger Event Cycle")]
    public void TriggerEvents()
    {
        StopAllCoroutines();
        StartCoroutine(RunEventCycle());
    }
    /// <summary>
    /// Stops all current event cycles.
    /// </summary>
    public void StopEvents()
    {
        StopAllCoroutines();
    }

    protected IEnumerator RunEventCycle()
    {
        float elapsedTime = 0f;
        float maxTime = events.Max(e => e.delay);
        var executedEvents = new List<TimedEvent>();

        while (elapsedTime <= maxTime)
        {
            // increment elapsedTime now so that the last event(s) will be called correctly
            elapsedTime += Time.deltaTime;

            // select all events that need to run this frame and have not been run already
            var eventsToRun = events.Where(e =>
            {
                return !executedEvents.Contains(e) && elapsedTime >= e.delay;
            });

            // run all our events
            foreach(var e in eventsToRun)
            {
                e.unityEvent.Invoke();
            }

            // add to the list of executed events so they aren't run twice
            executedEvents.AddRange(eventsToRun);

            yield return null;
        }

    }

}
