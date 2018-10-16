using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineCastSelector : MonoBehaviour
{

    public LineRenderer line;
    public Transform origin;

    public SelectionGroup selection;
    public Selectable furthestSelectable;

    public Vec2Resource inputAxis;

    public float distance;
    public Vector3 direction = Vector3.forward;

    public float maxDistance = 10f;
    public float inputEffectFactor = 1f;

    public bool isActive;

    protected Vector3 originPosition;
    protected Vector3 targetPosition;

    public Transform cursor;

    private void Start()
    {
        origin = origin ?? transform;
        line = line ?? GetComponent<LineRenderer>();


    }

    private void Update()
    {
        if (isActive)
        {
            UpdatePositions();
            UpdateLine();
            UpdateSelection();

        }
        else
        {
            line.SetPositions(new[] { Vector3.zero, Vector3.zero});
        }
    }

    /// <summary>
    /// Raycasts out and gathers all Selectable Elements we find
    /// Collates those into a SelectionGroup and positions our cursor at the farthest selectable we have
    /// </summary>
    private void UpdateSelection()
    {
        Vector3 targetDirection = targetPosition - originPosition;
        targetDirection.Normalize();


        // do our raycast & convert to a list
        RaycastHit[] hits = Physics.RaycastAll(originPosition, targetDirection, distance);
        if (hits.Length == 0)
        {
            cursor.transform.position = transform.position;
            furthestSelectable = null;
            return;
        }
            
        List<RaycastHit> hitList = new List<RaycastHit>(hits);

        // map over to list of selectables
        var hitListSelectables = hitList.FindAll(hit => hit.collider.GetComponent<SelectableElement>());
        var selectionHitList = hitListSelectables.Select(hit => hit.transform.GetComponent<SelectableElement>().selectable).ToList();

        // add or remove selectables from our selection
        var selectablesToRegister = selectionHitList.Except(selection.selectables).ToList();
        var selectablesToUnregister = selection.selectables.Except(selectionHitList).ToList();

        selectablesToRegister.ForEach(selectable => selection.RegisterSelectable(selectable));
        selectablesToUnregister.ForEach(selectable => selection.DeregisterSelecable(selectable));

        if (hitListSelectables.Count == 0)
            return;

        // sort our hitlist to find the furthest away selectable, put the cursor there and save the reference

        //hitListSelectables.Sort((a, b) => Vector3.SqrMagnitude(a.point - originPosition) > Vector3.SqrMagnitude(b.point - originPosition) ? 1 : -1);
        var sortedPoints = hitListSelectables.OrderByDescending(hit => Vector3.SqrMagnitude(hit.point - originPosition)).ToList();
        Debug.Log(sortedPoints[0]);
        cursor.transform.position = sortedPoints[0].point;
        //cursor.transform.rotation = Quaternion.LookRotation(hitListSelectables[0].normal);
        furthestSelectable = sortedPoints[0].transform.GetComponent<SelectableElement>().selectable;
    }

    /// <summary>
    /// Update the positions of the line renderer
    /// </summary>
    private void UpdateLine()
    {
        line.SetPositions(new[] { originPosition, targetPosition });
    }

    /// <summary>
    /// Get the input from our Vec2 Resource and adjust the distance of our line caster accordingly
    /// </summary>
    private void UpdatePositions()
    {
        float changeInDistance = inputAxis.Value.y * inputEffectFactor;
        distance += changeInDistance;
        distance = Mathf.Clamp(distance, 0f, maxDistance);

        originPosition = origin.position;
        targetPosition = originPosition + (origin.rotation * direction.normalized * distance);
    }


}
