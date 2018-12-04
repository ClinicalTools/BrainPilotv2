using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSequenceReciever : MonoBehaviour {

    public bool done = false;

    SequenceEvent currentEvent;

    EventSequence currentSequence;

    public UnityEvent unityEvent;

    /// <summary>
    /// Raises an event and begins to checks for a public bool being set. When the bool is set back to true, the originating EventSequence will be updated. If another Sequence is still waiting for completion it will be ignored.
    /// </summary>
    /// <param name="sequence"></param>
    /// <param name="thisEvent"></param>
    public void RaiseEvent(EventSequence sequence, SequenceEvent thisEvent)
    {
        done = false;
        currentSequence = sequence;
        currentEvent = thisEvent;

        unityEvent.Invoke();

        StopAllCoroutines();
        StartCoroutine(CheckForDone());
    }

    IEnumerator CheckForDone()
    {
        while (!done)
            yield return null;

        currentEvent.done = done;
        currentSequence.UpdateEventState(currentEvent);
    }
}
