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

    public float maxDistance = 20f;
	//Used to be .25f
    public float inputEffectFactor = 10f;

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
	public float stickDuration = 1f;
	private float stickTime = 0f;

	//Dissolve Mask Controller for Spotlight Cones
	AdvancedDissolve_Example.Controller_Mask_Cone maskController;
	private Vector3 spotLight3position;
	

    private void Start()
    {
        origin = origin ?? transform;
        line = line ?? GetComponent<LineRenderer>();
		bLine = bLine ?? GetComponent<BezierLineRenderer>();
		maskController = GetComponent<AdvancedDissolve_Example.Controller_Mask_Cone>(); 
		spotLight3position = maskController.spotLight3.transform.localPosition;

    }

	public BezierLineRenderer bLine;

    private void Update()
    {
        if (isActive)
        {
            UpdatePositions();
			if (!bLine.isActive) {
				UpdateLine();
			}
			//UpdateScale();
			UpdateSelection();

			
        }
        else
        {
            
            cursor.position = cursorSavedPosition;
            cursor.LookAt(transform);
			cursor.GetComponentInChildren<MeshRenderer>().enabled = false;
            //cursor.rotation = cursorSavedRotation;
        }
		UpdateScale();
		cursor.LookAt(transform);
    }

	public void Enable(bool isActive = true)
	{
		this.isActive = isActive;
		line.enabled = true;
		if (furthestSelectable != null) {
			cursor.gameObject.SetActive(true);
		}
		//UpdateSelection();
		//SetActive(true);
	}

	public void Disable()
	{
		float tempDist = distance;
		distance = 0;
		//Setting sticktime to less than 0 (and not -1) to trigger the unstick code
		stickTime = -10;
		UpdateSelection(); //To clear the selection
		distance = tempDist;
		line.enabled = false;
		isActive = false;
		//SetActive(false);
	}

	private void SetActive(bool active)
	{
		line.enabled = active;
	}

    public void GetClickDown(bool clickDown)
    {
        cursorSavedPosition = cursor.position;
        //cursorSavedRotation = cursor.rotation;
        isActive = !clickDown;
		if (isActive) {
			stickTime = stickDuration;
		}
		cursor.GetComponentInChildren<MeshRenderer>().enabled = isActive;
	}

	public void SetSavedCursorPos(Vector3 newPos)
	{
		cursorSavedPosition = newPos;
	}

    /// <summary>
    /// Raycasts out and gathers all Selectable Elements we find
    /// Collates those into a SelectionGroup and positions our cursor at the farthest selectable we have
    /// </summary>
    private void UpdateSelection()
    {
		if (!isActive) {
			//Set cursor true
			//Only reaches here if called from disable while over a canvas
			//We want to retain the selection if reselecting, clear if not??
			//How do we handle up presses if over canvas?
			//return;
		}

		Vector3 targetDirection = targetPosition - originPosition;
        targetDirection.Normalize();


        // do our raycast & convert to a list
        RaycastHit[] hits = Physics.RaycastAll(originPosition, targetDirection, distance);
		if (hits.Length == 0)
        {
            // invoke our event if ui target if we are deselecting a ui target
            if (uiTarget != null)
                uiTargetEvent.Invoke(null);
			//cursor.transform.position = transform.position;
			//cursor.gameObject.SetActive(false);
            if (furthestSelectable != null && stickTime == 0)
            {
				//selectableTargetEvent.Invoke(null);
				stickTime = stickDuration;
			}

			if (stickTime > 0) {
				stickTime -= Time.deltaTime;
			} else if (stickTime != -1) {
				TurnOffCursor();
			}

			//Uncomment to revert to pre-stick
			//furthestSelectable = null;
			uiTarget = null;
            return;
        }
		stickTime = 0;
		cursor.gameObject.SetActive(true);
            
        List<RaycastHit> hitList = new List<RaycastHit>(hits);

        // select out the hits with Selectables on them
        var hitListSelectables = hitList.FindAll(hit => hit.collider.GetComponent<SelectableElement>());

		if (hitListSelectables.Count == 0) {
			return;
		}

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

	public void TurnOffCursor()
	{
		Debug.Log("Turning off cursor");
		selectableTargetEvent.Invoke(null);
		cursor.transform.position = transform.position;
		cursor.gameObject.SetActive(false);
		furthestSelectable = null;
		stickTime = -1;
	}

	public void SelectNew(SelectableElement s)
	{
		// invoke our event if ui target if we are deselecting a ui target
		if (uiTarget != null) {
			//UI to brain
			uiTargetEvent.Invoke(null);
		}
		
		//cursor.transform.position = sortedPoints[0].point;
		if (uiTarget != null || furthestSelectable != s.selectable) {
			furthestSelectable = s.selectable;
			//Brain to Brain
			print("SET SELECTABLE TO " + s.selectable.name);
			selectableTargetEvent.Invoke(furthestSelectable);
		}
		uiTarget = null;

		// add or remove selectables from our selection
		//THIS MAINTAINS A LIST OF RAYCASTED SELECTABLES, KEPS IN SELECTIONGROUP LineSelect
		//CALLING THIS METHOD WILL NOT UPDATE THIS LIST, AS IT IS NOT NEEDED
		//IF NEEDED IN THE FUTURE, UNCOMMENT THIS CODE AND FIX ANY ERRORS
		/*var selectablesToRegister = selectionHitList.Except(selection.selectables).ToList();
		var selectablesToUnregister = selection.selectables.Except(selectionHitList).ToList();

		selectablesToRegister.ForEach(selectable => selection.RegisterSelectable(selectable));
		selectablesToUnregister.ForEach(selectable => selection.DeregisterSelecable(selectable));
		*/
	}

	public void SelectNew(SelectableElement s, Vector3 cursorPosition)
	{
		cursor.transform.position = cursorPosition;
		cursorSavedPosition = cursorPosition;
		SelectNew(s);
	}

	private void UpdateScale()
	{
		float distance = (cursor.position - transform.position).magnitude;
		//Multiply distance by ArcTan(x), where x is default size of the cursor we want.
		float y = distance * Mathf.Atan(2);

		//cursor decreases at 1/(sizeDecreaseRate * x)
		float sizeDecreaseRate = 3;
		//The bigger the startSize, the smaller it is. 1 = default size
		float startSize = 2;

		//The adjustment is used to slightly alter the scale of the cursor based on distance.
		float adjustment = 1 / (sizeDecreaseRate * (distance / maxDistance) + startSize);
		y *= adjustment;

		//move the x-ray cones based on the length of the line.
		maskController.spotLight2.range = distance/2;
		maskController.spotLight3.range = distance/2;
		maskController.spotLight3.transform.localPosition = new Vector3(spotLight3position.x, spotLight3position.y, (spotLight3position.z + distance));
		
		cursor.localScale = Vector3.one * y;
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
        float changeInDistance = inputAxis.Value.y * inputAxis.Value.y * (inputAxis.Value.y > 0 ? 1 : -1);
		changeInDistance *= inputEffectFactor;
		changeInDistance *= Time.deltaTime;
        distance += changeInDistance;
        distance = Mathf.Clamp(distance, 0f, maxDistance);


        originPosition = origin.position;
        targetPosition = originPosition + (origin.rotation * direction.normalized * distance);
    }


}
