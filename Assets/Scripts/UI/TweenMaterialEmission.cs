using System.Collections;
using UnityEngine;

public class TweenMaterialEmission : MonoBehaviour
{

    public Material material;

    public float transitionTime = .3f;
    public AnimationCurve curve;

    public Color tweenToColor = Color.red;

    protected Color startingColor;

    private void Start()
    {

        material = GetComponent<MeshRenderer>().material;
        //generate an instance of our material and assign it to the object

        startingColor = material.GetColor("_EmissionColor");
    }

    [ContextMenu("TweenToColor")]
    public void TweenToVariableColor()
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
