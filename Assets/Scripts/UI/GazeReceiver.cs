using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GazeUpdateEvent : UnityEvent<bool> { }

public class GazeReceiver : MonoBehaviour
{

    public float activeDegrees = 25f;

    public bool gazedAt;

    public GazeUpdateEvent updateEvent;

    public UnityEvent gazeStartEvent;

    public UnityEvent gazeEndEvent;

    private void Start()
    {
        gazedAt = false;
        AnnounceGazeReceiver();
    }

    private void OnDestroy()
    {
        RemoveGazeReceiver();
    }

    private void RemoveGazeReceiver()
    {
        foreach (var selector in GameObject.FindObjectsOfType<GazeAreaSelector>())
        {
            selector.UnregisterGazeReciever(this);
        }
    }

    private void AnnounceGazeReceiver()
    {
        foreach(var selector in GameObject.FindObjectsOfType<GazeAreaSelector>())
        {
            selector.RegisterGazeReceiver(this);
        }
    }

    public bool GazeUpdate(float degrees)
    {
        

        bool gazeStatus = degrees <= activeDegrees;

        if (gazeStatus == gazedAt)
            return gazeStatus;

        if (gazeStatus)
        {
            gazeStartEvent.Invoke();
        }
        else
        {
            gazeEndEvent.Invoke();
        }

        updateEvent.Invoke(gazeStatus);

        gazedAt = gazeStatus;

        return gazeStatus;
    }

}
