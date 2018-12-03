using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour {

    public Transform target;

    public float speed = 0.1f;
    public Vector3 rotation = Vector3.up;

    protected bool running = false;

    private void Start()
    {
        running = false;
    }

    public void SetActiveState(bool state)
    {
        if (!state)
        {
            StopAllCoroutines();
            running = false;
            return;
        }
        else
        {
            if (!running)
            {
                StartCoroutine(DoRotation());
                running = true;
            }
        }
    }

    IEnumerator DoRotation()
    {
        while (true)
        {
            target.Rotate(rotation * speed);
            yield return null;
        }
    }


}
