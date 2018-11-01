using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateToPosition : MonoBehaviour {

    public Vector3 startingOffset;

    public bool useRigidbody = false;

    public bool playOnAwake = false;

    public float runTime = 2f;

    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    Vector3 desination;

    Rigidbody rb;

	// Use this for initialization
	void Start () {
        if (useRigidbody)
            rb = GetComponent<Rigidbody>();
        desination = transform.position;
        transform.position = transform.position + startingOffset;
	}
	
    [ContextMenu("Play")]
    public void Play()
    {
        StartCoroutine(AnimatePosition());
    }

    IEnumerator AnimatePosition()
    {
        bool wasKinematic = false;
        if (rb != null)
        {
            wasKinematic = rb.isKinematic;
            rb.isKinematic = true;
        }
            
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;
        while (elapsedTime <= runTime)
        {
            Vector3 nextPosition = Vector3.LerpUnclamped(startingPosition, desination, animationCurve.Evaluate(elapsedTime / runTime));
            if (useRigidbody)
                rb.MovePosition(nextPosition);
            else
                transform.position = nextPosition;

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        if (rb != null)
        {
            rb.isKinematic = wasKinematic;
        }
    }

}
