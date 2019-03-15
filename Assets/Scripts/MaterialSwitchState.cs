using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaterialSwitchState : SelectableStateAction {

	//public Material material;
	//protected Material savedMaterial;

	public MeshRenderer renderer;

	[SerializeField]
	protected Color emissionColor;

	private float none = 0;
	private float cylinder = 6;

	public bool makeSolid = false;

	private void OnEnable()
	{
		if (element == null && GetComponent<SelectableElement>() != null) {
			element = GetComponent<SelectableElement>();
		}
	}

	public override void Activate()
    {
        base.Activate();
		//savedMaterial = element.meshRenderer.sharedMaterial;
		//element.meshRenderer.sharedMaterial = material;
		
		MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
		//Emission needs to be enabled in the material ahead of time for this to work
		propertyBlock.SetColor("_EmissionColor", emissionColor);
		if (makeSolid) {
			propertyBlock.SetFloat("_DissolveGlobalControl", none);
			propertyBlock.SetFloat("_DissolveMaskInvert", 0);
		}
		renderer.SetPropertyBlock(propertyBlock);

		//element.meshRenderer.SetPropertyBlock(propertyBlock);
    }

    public override void Deactivate()
    {
        base.Deactivate();

		//element.meshRenderer.sharedMaterial = savedMaterial;

		renderer.SetPropertyBlock(null);
		//element.meshRenderer.SetPropertyBlock(null);
	}

}
