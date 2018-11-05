using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenMaterialEmission : MonoBehaviour {

    public Material material;

    public float transitionTime = .3f;
    public AnimationCurve curve;

    public Color tweenToColor = Color.red;

    protected Color startingColor;

    private void Start()
    {
        material = material ?? GetComponent<MeshRenderer>().material;
        startingColor = material.GetColor("_EmissionColor");
    }

    [ContextMenu("TweenToColor")]
    public void TweenToColor()
    {
        StopAllCoroutines();
        StartCoroutine(TweenMaterial(tweenToColor));
    }

    [ContextMenu("ResetColor")]
    public void TweenToStartingColor()
    {
        StopAllCoroutines();
        StartCoroutine(TweenMaterial(startingColor));
    }

    IEnumerator TweenMaterial(Color finalColor)
    {
        float elapsedTime = 0;
        Color initialColor = material.GetColor("_EmissionColor");
        while (elapsedTime <= transitionTime)
        {
            Color color = Color.LerpUnclamped(initialColor, finalColor, curve.Evaluate(elapsedTime / transitionTime));
            material.SetColor("_EmissionColor", color);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

}
