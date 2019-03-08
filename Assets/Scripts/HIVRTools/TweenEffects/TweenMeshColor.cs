using System;
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


	private class MatTup {
		public MeshRenderer renderer; public Material original;

		public MatTup(Material original, MeshRenderer renderer)
		{
			this.original = original;
			this.renderer = renderer;
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof(MatTup)) {
				return false;
			}

			return original == ((MatTup)obj).original && renderer.sharedMaterial == ((MatTup)obj).renderer.sharedMaterial;
		}

		public override int GetHashCode()
		{
			var hashCode = 1957053554;
			hashCode = hashCode * -1521134295 + EqualityComparer<Material>.Default.GetHashCode(original);
			hashCode = hashCode * -1521134295 + EqualityComparer<MeshRenderer>.Default.GetHashCode(renderer);
			return hashCode;
		}
	}

	private static List<MatTup> originalMatList;
	private static bool unityAppQuitCalled = false;

	private void FixMaterials()
	{
		foreach(MatTup m in originalMatList) {
			m.renderer.sharedMaterial = m.original;
		}
	}

	private void Start()
    {
		if (!unityAppQuitCalled) {
			unityAppQuitCalled = true;
			//Application.quitting += FixMaterials;
		}

		properties = new MaterialPropertyBlock();
		targetMesh.GetPropertyBlock(properties);
		//targetMaterial = targetMesh.material;

		//In order to not mess up the order, swap the sharedmaterial's
		//state with the inactive one to help batching
		//targetMesh.sharedMaterial.SetColor(propertyName, activeColor.Color);
		//return;

		if (originalMatList == null) {
			originalMatList = new List<MatTup>();
		}
		if (originalMatList.Find((MatTup x) => x.original.Equals(targetMesh.sharedMaterial)) != null) {
			Material m = new Material(targetMesh.sharedMaterial);
			targetMesh.sharedMaterial.SetColor(propertyName, inActiveColor.Color);
			originalMatList.Add(new MatTup(m, targetMesh));
		}
		targetMesh.sharedMaterial.SetColor(propertyName, inActiveColor.Color);

		if (!ActiveState) {
			targetMesh.SetPropertyBlock(null);
			//properties.SetColor(propertyName, activeColor.Color);
			//targetMesh.SetPropertyBlock(properties);

			//targetMaterial.SetColor(propertyName, activeColor.Color);
		} else {
			properties.SetColor(propertyName, activeColor.Color);
			targetMesh.SetPropertyBlock(properties);
			
			//properties.SetColor(propertyName, inActiveColor.Color);
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
