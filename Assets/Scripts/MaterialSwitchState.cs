using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitchState : SelectableStateAction {

    public Material material;

    protected Material savedMaterial;

	[SerializeField]
	protected Color emissionColor;

	private float none = 0;
	private float cylinder = 6;

	public override void Activate()
    {
        base.Activate();
		
		//savedMaterial = element.meshRenderer.sharedMaterial;
		//element.meshRenderer.sharedMaterial = material;

		MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
		//propertyBlock.SetColor("_EmissionColor", emissionColor);
		propertyBlock.SetFloat("_DissolveMask", none);
		element.meshRenderer.SetPropertyBlock(propertyBlock);
    }

    public override void Deactivate()
    {
        base.Deactivate();

		//element.meshRenderer.sharedMaterial = savedMaterial;

		element.meshRenderer.SetPropertyBlock(null);
	}

}
