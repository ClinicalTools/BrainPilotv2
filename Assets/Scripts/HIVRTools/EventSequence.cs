using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class SequenceEvent
{
    public bool done = false;
    public int group;
    public UnityEventSequenced thisEvent;


}

[System.Serializable]
public class UnityEventSequenced: UnityEvent<EventSequence, SequenceEvent> { }

[CreateAssetMenu]
public class EventSequence : MonoBehaviour
{

    public List<SequenceEvent> sequenceEvents;

    [SerializeField]
    private int currentSeqGroup;

    [SerializeField]
    private List<int> groupList;
    private int groupIndex;

    [ContextMenu("Run Sequence")]
    public void StartSequence()
    {
        ResetAll();
        BuildGroupList();

        currentSeqGroup = groupList[groupIndex];
        RunEventGroup(currentSeqGroup);
    }

    [ContextMenu("Reset All Events")]
    private void ResetAll()
    {
        if (sequenceEvents == null)
            sequenceEvents = new List<SequenceEvent>();

        foreach (var e in sequenceEvents)
        {
            e.done = false;
        }
    }

    private void RunEventGroup(int group)
    {
        foreach (var e in sequenceEvents.Where(someEvent => someEvent.group == group))
        {
            e.thisEvent.Invoke(this, e);
        }
    }

    private void BuildGroupList()
    {
        if (sequenceEvents == null)
            sequenceEvents = new List<SequenceEvent>();

        groupList = new List<int>();
        groupIndex = 0;

        foreach(var e in sequenceEvents)
        {
            if (!groupList.Contains(e.group))
                groupList.Add(e.group);
        }
        groupList.Sort();
    }


    private bool SequenceCanAdvance()
    {
        // returns true if all events in the current group are done
        return (sequenceEvents.Where(e => e.group == currentSeqGroup).All(e => e.done));
    }

    /// <summary>
    /// Gets an event from a EventSequence Listener, and if the event is in the current group, will check to see if we can advance the group. Does not modify the SequenceEvent that it is passed.
    /// </summary>
    /// <param name="updatedEvent"></param>
    public void UpdateEventState(SequenceEvent updatedEvent)
    {
        if (sequenceEvents.Where(e => e.group == currentSeqGroup).Contains(updatedEvent)
            && SequenceCanAdvance())
        {

            AdvanceSequence();
        }

    }

    private void AdvanceSequence()
    {
        currentSeqGroup = groupList[++groupIndex];
        RunEventGroup(currentSeqGroup);
    }

    public void FinishGroup(int group)
    {
        foreach (var e in sequenceEvents.Where(someEvent => someEvent.group == group))
        {
            e.done = true;
        }
    }

}
