using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UITargetEvent : UnityEvent<UIElement> { }

[System.Serializable]
public class SelectableTargetEvent : UnityEvent<Selectable> { }

[RequireComponent(typeof(LineRenderer))]
public class LineCastSelector : MonoBehaviour
{

    public LineRenderer line;
    public Transform origin;

    public SelectionGroup selection;
    public Selectable furthestSelectable;
    public UIElement uiTarget;
    

    public UITargetEvent uiTargetEvent;
    public SelectableTargetEvent selectableTargetEvent;

    public Vec2Resource inputAxis;

    public float distance;
    public Vector3 direction = Vector3.forward;

    public float maxDistance = 10f;
    public float inputEffectFactor = 1f;

    public bool isActive;

	/*public bool accelerateOnHold;
	public AnimationCurve speedIncrease;
	public float speedFactor;
	private float timeHeld;*/

    protected Vector3 originPosition;
    protected Vector3 targetPosition;

    public Transform cursor;
    protected Vector3 cursorSavedPosition;
    protected Quaternion cursorSavedRotation;

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
            
            cursor.position = cursorSavedPosition;
            cursor.LookAt(transform);
			cursor.GetComponentInChildren<MeshRenderer>().enabled = false;
            //cursor.rotation = cursorSavedRotation;
        }
        cursor.LookAt(transform);
    }

    public void GetClickDown(bool clickDown)
    {
        cursorSavedPosition = cursor.position;
        //cursorSavedRotation = cursor.rotation;
        isActive = !clickDown;

		cursor.GetComponentInChildren<MeshRenderer>().enabled = isActive;
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
            // invoke our event if ui target if we are deselecting a ui target
            if (uiTarget != null)
                uiTargetEvent.Invoke(null);

            cursor.transform.position = transform.position;
			cursor.gameObject.SetActive(false);
            if (furthestSelectable != null)
            {
                selectableTargetEvent.Invoke(null);
            }
            furthestSelectable = null;
            uiTarget = null;
            return;
        }
		cursor.gameObject.SetActive(true);
            
        List<RaycastHit> hitList = new List<RaycastHit>(hits);

        // select out the hits with Selectables on them
        var hitListSelectables = hitList.FindAll(hit => hit.collider.GetComponent<SelectableElement>());

        if (hitListSelectables.Count == 0)
            return;

        // sort our hitlist by distance
        var sortedPoints = hitListSelectables.OrderByDescending(hit => Vector3.SqrMagnitude(hit.point - originPosition)).ToList();

        // map over to a list of selectables
        var selectionHitList = sortedPoints.Select(hit => hit.transform.GetComponent<SelectableElement>().selectable).ToList();

        // check to see if we have a UI Element and if so pick the first one out
        
        if (selectionHitList.Any(selectable => selectable is UIElement))
        {
            var ui = selectionHitList.First(selectable => selectable is UIElement) as UIElement;
            if (ui != uiTarget)
            {
                uiTarget = ui;
				//Enter UI
                uiTargetEvent.Invoke(uiTarget);
            }
            cursor.transform.position = sortedPoints[selectionHitList.IndexOf(ui)].point;
            selectionHitList = new List<Selectable>
            {
                ui
            };
        }
        else
        {
			// invoke our event if ui target if we are deselecting a ui target
			if (uiTarget != null) {
				//UI to brain
				uiTargetEvent.Invoke(null);
			}

            cursor.transform.position = sortedPoints[0].point;
            if (uiTarget != null || furthestSelectable != sortedPoints[0].transform.GetComponent<SelectableElement>().selectable)
            {
                furthestSelectable = sortedPoints[0].transform.GetComponent<SelectableElement>().selectable;
				//Brain to Brain
                selectableTargetEvent.Invoke(furthestSelectable);
            }
            uiTarget = null;

        }

        // add or remove selectables from our selection
        var selectablesToRegister = selectionHitList.Except(selection.selectables).ToList();
        var selectablesToUnregister = selection.selectables.Except(selectionHitList).ToList();

        selectablesToRegister.ForEach(selectable => selection.RegisterSelectable(selectable));
        selectablesToUnregister.ForEach(selectable => selection.DeregisterSelecable(selectable));
    }

    /// <summary>
    /// Update the positions of the line renderer
    /// </summary>
    private void UpdateLine()
    {
        line.positionCount = 2;
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
