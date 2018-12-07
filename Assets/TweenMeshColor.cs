using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TweenMeshColor : MonoBehaviour
{

    public MeshRenderer targetMesh;
    private Material targetMaterial;
    public string propertyName = "_Color";

    public float transitionTime = 0.3f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public ColorResource tweenToColor;
    private Color savedColor;

    private void Start()
    {
        targetMaterial = targetMesh.material;
        savedColor = targetMaterial.GetColor(propertyName);
        
    }

    public void SetActiveState(bool stateIsOn)
    {
        StopAllCoroutines();


        if (stateIsOn)
        {
            StartCoroutine(RunTransition(savedColor, tweenToColor.Color));
        }
        else
        {
            StartCoroutine(RunTransition(tweenToColor.Color, savedColor));
        }
    }

    private IEnumerator RunTransition(Color startingColor, Color endingColor)
    {
        float elapsedTime = 0f;
        while (elapsedTime <= transitionTime)
        {
            float ratio = elapsedTime / transitionTime;
            Color nextColor = Color.LerpUnclamped(startingColor, endingColor, curve.Evaluate(ratio));
            targetMaterial.SetColor(propertyName, nextColor);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

}
