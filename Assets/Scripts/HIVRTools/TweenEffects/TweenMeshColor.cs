using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TweenMeshColor : MonoBehaviour
{

    public MeshRenderer targetMesh;
    private Material targetMaterial;
	private MaterialPropertyBlock properties;
    public string propertyName = "_Color";

    public float transitionTime = 0.3f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public ColorResource activeColor;
    public ColorResource inActiveColor;
    [SerializeField]
    public bool ActiveState
    {
        get
        {
            return activeState;
        }
        set
        {
            if (activeState != value)
                SetActiveState(value);
        }
    }
    [SerializeField]
    private bool activeState;

    private void Start()
    {
		properties = new MaterialPropertyBlock();
		targetMesh.GetPropertyBlock(properties);
		//targetMaterial = targetMesh.material;

		if (ActiveState) {
			targetMesh.SetPropertyBlock(null);
			//properties.SetColor(propertyName, activeColor.Color);
			//targetMesh.SetPropertyBlock(properties);
			//targetMaterial.SetColor(propertyName, activeColor.Color);
		} else {
			properties.SetColor(propertyName, inActiveColor.Color);
			targetMesh.SetPropertyBlock(properties);
			//targetMaterial.SetColor(propertyName, inActiveColor.Color);
		}

    }

    public void SetActiveState(bool stateIsOn)
    {
        StopAllCoroutines();


        if (stateIsOn)
        {
            StartCoroutine(RunTransition(inActiveColor.Color, activeColor.Color));
        }
        else
        {
            StartCoroutine(RunTransition(activeColor.Color, inActiveColor.Color));
        }

        activeState = stateIsOn;
    }

    private IEnumerator RunTransition(Color startingColor, Color endingColor)
    {

        float elapsedTime = 0f;
        while (elapsedTime <= transitionTime)
        {
            float ratio = elapsedTime / transitionTime;
            Color nextColor = Color.LerpUnclamped(startingColor, endingColor, curve.Evaluate(ratio));
			properties.SetColor(propertyName, nextColor);
			targetMesh.SetPropertyBlock(properties);
			//targetMaterial.SetColor(propertyName, nextColor);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

}
