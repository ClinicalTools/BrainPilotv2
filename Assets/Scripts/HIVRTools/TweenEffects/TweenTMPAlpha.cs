using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TweenTMPAlpha : MonoBehaviour {

    [Range(0f, 1f)]
    public float fadeOutAlpha;
    [Range(0f, 1f)]
    public float fadeInAlpha;

    public float fadeTime = 0.3f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);

    public TextMeshPro textMesh;

    public void SetFadeStatus(bool status) {
        if (status)
            FadeIn();
        else FadeOut();
    }

    [ContextMenu("Fade In")]
    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(Fade(fadeOutAlpha, fadeInAlpha));
    }

    [ContextMenu("Fade Out")]
    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(Fade(fadeInAlpha, fadeOutAlpha));
    }

    protected IEnumerator Fade(float start, float end)
    {
        float elapsedTime = 0;
        textMesh = textMesh ?? GetComponent<TextMeshPro>();

        while (elapsedTime <= fadeTime)
        {
            float t = elapsedTime / fadeTime;
            float alpha = Mathf.LerpUnclamped(start, end, curve.Evaluate(t));
            textMesh.alpha = alpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

}
