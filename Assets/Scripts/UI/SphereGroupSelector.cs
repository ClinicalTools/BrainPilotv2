using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SphereGroupSelector : MonoBehaviour {

    public Transform target;

    [Range(0.01f, 20f)]
    public float radius = 3f;

    public SelectionGroup selectionGroup;

    private void FixedUpdate()
    {
        CastOverlapSphere();
    }

    private void CastOverlapSphere()
    {
        Vector3 position = target.position;

        List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(position, radius));

        List <Selectable> contactedSelectables = colliders.FindAll(collider => collider.GetComponent<SelectableElement>())
            .Select(collider => collider.GetComponent<SelectableElement>().selectable).ToList();

        var selectablesToRegister = contactedSelectables.Except(selectionGroup.selectables).ToList();
        var selectablesToUnregister = selectionGroup.selectables.Except(contactedSelectables).ToList();

        selectablesToRegister.ForEach(selectable => selectionGroup.RegisterSelectable(selectable));
        selectablesToUnregister.ForEach(selectable => selectionGroup.DeregisterSelecable(selectable));


    }
}
