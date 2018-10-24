using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TweenItemToTarget : MonoBehaviour
{
    public float effectTime = 0.6f;
    public AnimationCurve animationCurve;
    public Transform target;

    public UnityEvent effectComplete;

    public void GetNewRB(Rigidbody rb)
    {
        StartCoroutine(MoveRBToTarget(rb));
    }

    IEnumerator MoveRBToTarget(Rigidbody rb)
    {
        rb.isKinematic = true;
        float elapsedTime = 0;
        Vector3 startingPosition = rb.position;
        Vector3 targetPosition = target.position;
        

        while (elapsedTime <= effectTime && rb != null)
        {
            Vector3 nextPosition = Vector3.LerpUnclamped(startingPosition, targetPosition, animationCurve.Evaluate(elapsedTime / effectTime));
            rb.MovePosition(nextPosition);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        effectComplete.Invoke();
    }
     

}
