using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitchState : SelectableStateAction {

    public Material material;

    protected Material savedMaterial;

    public override void Activate()
    {
        base.Activate();

        savedMaterial = element.meshRenderer.sharedMaterial;
        element.meshRenderer.sharedMaterial = material;
    }

    public override void Deactivate()
    {
        base.Deactivate();

        element.meshRenderer.sharedMaterial = savedMaterial;
    }

}
