using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to extend SelectableStateAction but undesirable destroy effects are making me change that
[ExecuteInEditMode]
public class MaterialSwitchState : MonoBehaviour {

	//public Material material;
	protected Material savedMaterial;
	private bool active;

	new public MeshRenderer renderer;

	[SerializeField]
	protected Color emissionColor = new Color(63/255f, 63/255f, 63/255f);

	private float none = 0;
	private float cylinder = 6;

	public bool makeSolid = true;

	private void OnEnable()
	{
		/*if (element == null && GetComponent<SelectableElement>() != null) {
			element = GetComponent<SelectableElement>();
		}*/
#if UNITY_EDITOR
		if (renderer == null) {
			if (transform.childCount > 0 && transform.GetChild(0).GetComponentInChildren<MeshRenderer>()) {
				renderer = transform.GetChild(0).GetComponent<MeshRenderer>();
				if (renderer == null) {
					UnityEditor.Undo.DestroyObjectImmediate(this);
				}
			} else {
				renderer = GetComponent<MeshRenderer>();
			}
		}
#endif
	}
	Material matTemp;
	public /*override*/ void Activate()
    {
		//base.Activate();
		if (active) {
			return;
		}
		active = true;
		if (propertyBlock != null && renderer.HasPropertyBlock()) {
			MaterialPropertyBlock block = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(block);
			renderer.SetPropertyBlock(null);
			savedMaterial = renderer.sharedMaterial;
			renderer.SetPropertyBlock(block);
		} else {
			savedMaterial = renderer.sharedMaterial;
		}

		renderer.material.DisableKeyword("_DISSOLVEGLOBALCONTROL_ALL");
		renderer.material.DisableKeyword("_DISSOLVEGLOBALCONTROL_MASK_ONLY");
		renderer.material.DisableKeyword("_DISSOLVEGLOBALCONTROL_MASK_AND_EDGE");
		renderer.material.EnableKeyword("_EmissionColor");
		//renderer.sharedMaterial.EnableKeyword("_DissolveMaskInvert");

		renderer.material.SetColor("_Color", emissionColor);
		//renderer.material.SetTexture("_EmissionMap", renderer.sharedMaterial.mainTexture);
		if (makeSolid) {
			renderer.material.SetFloat("_DissolveGlobalControl", none);
			renderer.material.SetFloat("_DissolveMaskInvert", 0);
		}


		/*
		MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
		//Emission needs to be enabled in the material ahead of time for this to work
		propertyBlock.SetColor("_EmissionColor", emissionColor);
		propertyBlock.SetTexture("_EmissionMap", renderer.sharedMaterial.mainTexture);
		if (makeSolid) {
			propertyBlock.SetFloat("_DissolveGlobalControl", none);
			propertyBlock.SetFloat("_DissolveMaskInvert", 0);
		}
		renderer.SetPropertyBlock(propertyBlock);
		*/
    }

    public /*override*/ void Deactivate()
    {
		//base.Deactivate();

		renderer.sharedMaterial = savedMaterial;
		active = false;
		//renderer.SetPropertyBlock(null);
	}

	public void HighlightBlip(Color c)
	{
		highlightColor = c;
		timeVal = 0;
	}
	private float timeVal = 1f;
	private Color highlightColor;
	MaterialPropertyBlock propertyBlock;
	private float duration = 1f;
	private void Update()
	{
		return;
		if (timeVal < duration) {
			propertyBlock = new MaterialPropertyBlock();
			propertyBlock.SetColor("_EmissionColor", Color.Lerp(highlightColor, Color.black, timeVal / duration));
			//propertyBlock.SetColor("_Color", highlightColor);
			renderer.SetPropertyBlock(propertyBlock);
		} else {
			if (renderer.HasPropertyBlock()) {
				propertyBlock = null;
				//renderer.SetPropertyBlock(propertyBlock);
			}
			timeVal = duration;
		}
	}
}
