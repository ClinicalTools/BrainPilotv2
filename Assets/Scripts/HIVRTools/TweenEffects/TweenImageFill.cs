using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TweenImageFill : MonoBehaviour {

    public Image image;

    public float effectTime;

    public AnimationCurve curve;

    public UnityEvent effectComplete;

    public void SetNewFill(float targetFill)
    {
        StartCoroutine(SetFill(targetFill));
    }

    IEnumerator SetFill(float targetFill)
    {
        float startingFill = image.fillAmount;
        float elapsedTime = 0;
        while (elapsedTime <= effectTime)
        {
            image.fillAmount = Mathf.LerpUnclamped(startingFill, targetFill, curve.Evaluate(elapsedTime / effectTime));
            elapsedTime += Time.deltaTime;
            yield return null;

        }
        effectComplete.Invoke();
    }


}
