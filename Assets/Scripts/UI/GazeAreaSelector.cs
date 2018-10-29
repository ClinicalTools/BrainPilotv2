using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeAreaSelector : MonoBehaviour {

    public List<GazeReceiver> receivers;

    public List<GazeReceiver> gazedReceivers;

    public Transform gazeTransform;
    public Vector3 gazeDirection = Vector3.forward;

    private void Start()
    {
        receivers = new List<GazeReceiver>(GameObject.FindObjectsOfType<GazeReceiver>());
    }

    public void RegisterGazeReceiver(GazeReceiver receiver)
    {
        if (receivers == null)
            receivers = new List<GazeReceiver>(GameObject.FindObjectsOfType<GazeReceiver>());

        if (!receivers.Contains(receiver))
            receivers.Add(receiver);
    }

    public void UnregisterGazeReciever(GazeReceiver receiver)
    {
        if (receivers.Contains(receiver))
            receivers.Remove(receiver);
    }

    float GazeAngle(GazeReceiver receiver)
    {
        Vector3 receiverDirection = (receiver.transform.position - gazeTransform.position);
        Debug.Log("Direction: " + receiverDirection);
        return Vector3.Angle(receiverDirection, transform.forward);
    }

    private void Update()
    {
        List<GazeReceiver> currentGaze = new List<GazeReceiver>();
        foreach(GazeReceiver receiver in receivers)
        {
            if (receiver.GazeUpdate(GazeAngle(receiver)) && !currentGaze.Contains(receiver))
            {
                currentGaze.Add(receiver);
            }
            else
            {
                currentGaze.Remove(receiver);
            }
        }

        gazedReceivers = currentGaze;
    }

}
