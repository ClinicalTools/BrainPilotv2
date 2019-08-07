using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to extend SelectableStateAction but undesirable destroy effects are making me change that
[ExecuteInEditMode]
public class MaterialSwitchState : MonoBehaviour {

	//public Material material;
	protected Material savedMaterial;
	public bool active;

	new public MeshRenderer renderer;

	[SerializeField]
	private Color emissionColor = new Color(63/255f, 63/255f, 63/255f);
	public Color brightenColor = new Color(.8f, .8f, .8f, 1f);

	private readonly float none = 0;
	//private float cylinder = 6;

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

	public void ActivateWithColor(Color c)
	{
		Color temp = emissionColor;
		emissionColor = c;
		Activate();
		emissionColor = temp;
	}

	public bool bright;
	public void Brighten()
	{
		if (active) {
			print("Already active");
			return;
		}
		if (savedMaterial != null) {
			//renderer.sharedMaterial = savedMaterial;
		} else {
			savedMaterial = renderer.sharedMaterial;
		}

		renderer.material.SetColor("_Color", brightenColor);
		bright = true;

		if (makeSolid) {
			MakeSolid();
		}
	}


	public void Darken()
	{
		bright = false;
		Deactivate();
	}

	Material matTemp;
	public /*override*/ void Activate()
	{
		//base.Activate();
		if (active) {
			return;
		}
		if (savedMaterial != null) {
			//renderer.sharedMaterial = savedMaterial;
		} else {
			savedMaterial = renderer.sharedMaterial;
		}
		active = true;
		
		renderer.material.EnableKeyword("_EmissionColor");
		renderer.material.SetColor("_Color", emissionColor);
		//renderer.material.SetTexture("_EmissionMap", renderer.sharedMaterial.mainTexture);

		if (makeSolid) {
			MakeSolid();
		}
	}

	private void MakeSolid()
	{
		renderer.material.DisableKeyword("_DISSOLVEGLOBALCONTROL_ALL");
		renderer.material.DisableKeyword("_DISSOLVEGLOBALCONTROL_MASK_ONLY");
		renderer.material.DisableKeyword("_DISSOLVEGLOBALCONTROL_MASK_AND_EDGE");
		//renderer.sharedMaterial.EnableKeyword("_DissolveMaskInvert");

		//renderer.material.SetFloat("_DissolveGlobalControl", none);
		renderer.material.SetFloat("_DissolveMaskCount", 0);
		renderer.material.SetFloat("_DissolveMaskInvert", 0);
	}

	public /*override*/ void Deactivate()
	{
		//base.Deactivate();
		active = false;
		if (bright) {
			Brighten();
		} else {
			if (savedMaterial != null) {
				renderer.sharedMaterial = savedMaterial;
				savedMaterial = null;
			}
		}
		//renderer.enabled = false;
		//renderer.SetPropertyBlock(null);
	}

	private void OldActivateCode()
	{
		//renderer.enabled = true;
		/*if (renderer.HasPropertyBlock()) {
			MaterialPropertyBlock block = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(block);
			renderer.SetPropertyBlock(null);
			savedMaterial = renderer.sharedMaterial;
			renderer.SetPropertyBlock(block);
		} else {
			savedMaterial = renderer.sharedMaterial;
		}*/

		//Changing renderer keywords

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

		ParticleSystemRenderer r = new ParticleSystemRenderer();
		r.SetPropertyBlock(propertyBlock);
		*/
	}

	/*
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
		/*
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
	}*/
}
