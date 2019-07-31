using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(DroneController))]
public class DroneControllerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var controller = (DroneController)target;
		if (GUILayout.Button("Activate")) {
			controller.SetActive(true);
		}
		if (GUILayout.Button("Deactivate")) {
			controller.SetActive(false);
		}

		if (GUILayout.Button("Advance Sequence")) {
			controller.AdvanceSequence();
		}

		if (GUILayout.Button("Receed Sequence")) {
			controller.RecedeSequence();
		}

		if (GUILayout.Button("Update Drone Display Settings")) {
			controller.ChangeDisplayMethod(controller.displayIdx);
		}

		if (GUILayout.Button("Start lesson")) {
			controller.BeginSequenceAt(1);
		}
	}
}
#endif

[System.Serializable]
public class DroneSettings
{
	//UI Element Menu reference for setting text?
	public float maxDistance;
	public float maxSpeed;
	public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public float fadeDuration = 2f;
}

public class DroneController : MonoBehaviour {

	public DroneSettings settings;

	[SerializeField]
	private List<Vector3> potentialGoals;
	[SerializeField]
	private Transform mainCamera;
	[SerializeField]
	private Transform platform;
	[SerializeField]
	private LineCastSelector selector;

	public TMPro.TextMeshPro textField;
	public bool isPositionStatic;

	private bool _active = false;
	private float distance;
	private Vector3 goalLoc;
	private MeshRenderer[] mesh;
	[SerializeField]
	private Selectable selection;
	[SerializeField]
	private Sequence sequence;
	private bool ignoreSequence = false;

	public bool gazeBased;
	//Used without gaze
	private int activeGoalIdx;

	public GameObject buttons;

	public float val = 100;

	private bool hasActiveSequence
	{
		get
		{
			return !ignoreSequence && sequence != null && sequence.IsActive();
		}
	}
	private bool lockedSelection;

	public enum DisplayMethod
	{
		AlwaysOn,
		SelectedOnly,
		Off
	}
	public DisplayMethod _display = DisplayMethod.AlwaysOn;

	public int displayIdx;

	private Coroutine fadeMesh;

	// Use this for initialization
	void Start () {
		OVRManager.tiledMultiResLevel = OVRManager.TiledMultiResLevel.LMSMedium;
		settings.maxDistance *= settings.maxDistance;
		if (mesh == null) {
			mesh = GetComponentsInChildren<MeshRenderer>();
		}
		foreach (MeshRenderer m in mesh) {
			m.sharedMaterial.SetFloat("_DissolveCutoff", 1f);
		}
		if (!gazeBased) {
			//mainCamera = transform.parent;
			mainCamera = GameObject.Find("new_platform01").transform;
		}

		GetComponent<DroneListener>().data.selection = null;
	}

	/**
	 * GazeBased is current implementation
	 * Otherwise will have the drone spawn in one of the predetermined
	 * spots and not move until the Move button is pressed.
	 * Then it will recalculate the closest goal.
	 */

	// Update is called once per frame
	void Update () {
		for(int i = 0; i < val; i++) {
			Debug.Log("Hello");
		}

		//Assumes we're showing the drone
		if (_active) {
			if (gazeBased) {
				//Rotate the drone to face the player
				transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.position);

				//Move the drone towards the nearest goal location
				//goalLoc = mainCamera.position + mainCamera.forward * 3 + mainCamera.right * 3;
				goalLoc = GetGoalOffset(PickClosestGoalIdx());
				Debug.DrawLine(mainCamera.position, goalLoc, Color.red);
				distance = (transform.position - goalLoc).sqrMagnitude;
				if (distance < 0.1f) {
					return;
				}

				//float lerpVal = SmoothStep.SmoothStop(3, distance / maxDistance);
				float lerpVal = settings.curve.Evaluate(distance / settings.maxDistance);

				transform.position = Vector3.MoveTowards(
										transform.position,
										goalLoc,
										lerpVal * settings.maxSpeed);
			} else {
				goalLoc = GetGoalOffset(activeGoalIdx);
				if (isPositionStatic) {
					transform.position = goalLoc;
				} else {
					distance = (transform.position - goalLoc).sqrMagnitude;
					if (distance > .02f) {
						float lerpVal = settings.curve.Evaluate(distance / settings.maxDistance);
						Debug.DrawLine(mainCamera.position, goalLoc, Color.red);

						transform.position = Vector3.MoveTowards(
												transform.position,
												goalLoc,
												lerpVal * settings.maxSpeed);
					}
				}
				transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.position);
			}
			UpdateText();
		}
	}

	public void SetActive(bool active)
	{
		if (active == _active) {
			return;
		}
		_active = active;
		if (active) {
			transform.Find("TextContainer").gameObject.SetActive(active);
		}

		GetComponentInChildren<TweenItemScaleBetweenVec3Resources>().SetActiveState(active);
		//StopAllCoroutines();
		if (fadeMesh != null)
			StopCoroutine(fadeMesh);

		//inactive sequence &(off or selection w/o piece) = off
		//active sequence or on or selection w/ piece = on
		if (!hasActiveSequence && 
			(_display == DisplayMethod.Off || 
			(_display == DisplayMethod.SelectedOnly && !lockedSelection))) {
			fadeMesh = StartCoroutine(FadeMesh(1));
		} else {
			fadeMesh = StartCoroutine(FadeMesh(_active ? 0f : 1f));
		}

		/*if (!active) {
			transform.Find("TextContainer").gameObject.SetActive(active);
		}*/

		if (!gazeBased) {
			if (!_active) {
				goalLoc = Vector3.zero;
			} else {
				TeleportToClosestGoal();
			}
		}
	}

	public void ChangeDisplayMethod(int idx)
	{
		_display = (DisplayMethod)idx;
		HandleDisplay();
	}

	//Sets the display to the accurate setting
	public void HandleDisplay()
	{
		//StopAllCoroutines();
		if (fadeMesh != null)
			StopCoroutine(fadeMesh);
		if (hasActiveSequence) {
			fadeMesh = StartCoroutine(FadeMesh(0));
			return;
		}
		switch (_display) {
			case DisplayMethod.AlwaysOn:
				fadeMesh = StartCoroutine(FadeMesh(0));
				break;
			case DisplayMethod.SelectedOnly:
				if (!lockedSelection) {
					fadeMesh = StartCoroutine(FadeMesh(1));
				} else {
					fadeMesh = StartCoroutine(FadeMesh(0));
				}
				break;
			case DisplayMethod.Off:
				fadeMesh = StartCoroutine(FadeMesh(1));
				break;
		}
	}

	//Ask the drone to show or hide
	public void HandleDisplay(bool show)
	{
		if (selection != null && show) {
			lockedSelection = true;
		} else {
			lockedSelection = false;
		}
		//StopAllCoroutines();
		if (fadeMesh != null)
			StopCoroutine(fadeMesh);
		if (_display == DisplayMethod.SelectedOnly) {
			fadeMesh = StartCoroutine(FadeMesh(show | hasActiveSequence ? 0f : 1f));
		}
	}

	public void ToggleActive()
	{
		SetActive(!_active);
	}

	public void TeleportToClosestGoal()
	{
		if (goalLoc == Vector3.zero) {
			transform.position = GetGoalOffset(PickClosestGoalIdx());
			goalLoc = transform.position;
		} else {
			activeGoalIdx++;
			activeGoalIdx = activeGoalIdx % potentialGoals.Count;
			transform.position = GetGoalOffset(activeGoalIdx);
			goalLoc = transform.position;
		}
	}

	/// <summary>
	/// Fades the Disolve shader in or out.
	/// 1 = invisible
	/// </summary>
	/// <param name="endVal"></param>
	private IEnumerator FadeMesh(float endVal, float duration = 2f)
	{
		if (mesh == null) mesh = GetComponentsInChildren<MeshRenderer>();

		float val = mesh[0].sharedMaterial.GetFloat("_DissolveCutoff");
		if (val == endVal) {
			yield break;
		}

		if (endVal == 1) {
			transform.Find("TextContainer/TextMeshPro").gameObject.SetActive(false);
		} else if (endVal == 0) {
			transform.Find("TextContainer/TextMeshPro").gameObject.SetActive(true);
			transform.Find("TextContainer").gameObject.SetActive(true);
		}
		
		float lerpVal = 0f;
		float interval = Time.deltaTime / settings.fadeDuration;
		while (val != endVal) {
			lerpVal += interval;
			val = Mathf.Lerp(val, endVal, lerpVal);
			foreach (MeshRenderer m in mesh) {
				m.sharedMaterial.SetFloat("_DissolveCutoff", val);
			}
			yield return null;
		}
		if (endVal == 1) {
			transform.Find("TextContainer").gameObject.SetActive(false);
		}
	}

#region TextDisplay
	string displayString;
	//This should be put into a listener or something I guess.
	private void UpdateText()
	{
		if (hasActiveSequence) {
			textField.text = sequence.GetActiveStep()?.textToDisplay;

			//Display highlight name?
			//sequence.GetActiveStep()?.brainPiecesToHighlight
		} else if (selection != null && selection is BrainElement) {
			//Update with a description of the selected brain piece
			displayString = "<size=60%><b><line-height=80%><color=#00FFFF>";
			displayString += ((BrainElement)selection).elementName;
			displayString += "</color></line-height></b>:</size>\n";
			textField.text = displayString + ((BrainElement)selection).description;
		} else {
			//Other uses
		}
	}

	public void UpdateDroneButtons()
	{
		if (hasActiveSequence) {
			buttons.SetActive(true);

			//Disable the previous button if on the first step
			buttons.transform.GetChild(0).gameObject.SetActive(sequence.GetActiveIndex() != 0);
		} else {
			buttons.SetActive(false);
		}
	}

	public void SetSelectable(Selectable selectable)
	{
		selection = selectable;
	}

	private void UpdatePlatform()
	{
		if (hasActiveSequence) {
			if (!_active) {
				SetActive(true);
			}
		}/* else {
				fadeMesh = StartCoroutine(FadeMesh(0));
			}
		} else if (_display == DisplayMethod.Off) {
			//StopAllCoroutines();
			if (fadeMesh != null)
				StopCoroutine(fadeMesh);
			fadeMesh = StartCoroutine(FadeMesh(1));
		}*/
		HandleDisplay();
		UpdateText();
		UpdateDroneButtons();
		UpdateHighlightedNames();
		StopMovingPlatform(IsPlatformInfoEmpty());
		movePlatformCoroutine = StartCoroutine(MovePlatform());
	}

	private bool IsPlatformInfoEmpty()
	{
		if (info == null) return true;
		return
			info.waypointLocation == Vector3.zero &&
			info.scaleVal == 0 &&
			info.lookAtPoint == null;
	}
	private Coroutine movePlatformCoroutine;

	/// <summary>
	/// Loads a sequence from the start
	/// </summary>
	/// <param name="s"></param>
	public void BeginSequence(Sequence s)
	{
		ignoreSequence = false;
		sequence = s;
		sequence.ResetSequence();
		sequence.StartSequence();

		UpdatePlatform();
	}

	public void BeginSequenceAt(int i)
	{
		GetComponentInParent<DroneManager>().GetSequenceAt(1);
	}

	public void ResumeSequence()
	{
		ignoreSequence = false;
		sequence.ResumeSequence();
		UpdatePlatform();
	}
	
	/// <summary>
	/// Loads a sequence and resumes it.
	/// </summary>
	/// <param name="s">The sequence to run</param>
	public void ResumeSequence(Sequence s)
	{
		if (s == null) {
			Debug.LogWarning("Sequence is null!");
			return;
		} else if (s.steps.Length == 0) {
			Debug.LogWarning("Sequence has no elements!");
			return;
		}
		ignoreSequence = false;
		sequence = s;
		sequence.StartSequence();

		UpdatePlatform();
	}

	public void PauseSequence()
	{
		ignoreSequence = true;
		sequence?.PauseSequence();
		UpdatePlatform();
	}

	/// <summary>
	/// Advances the active sequence to the next area
	/// </summary>
	public void AdvanceSequence()
	{
		if (ignoreSequence) {
			return;
		}
		if (sequence == null) {
			print("Null sequence");
		} else {
			print("Advancing sequence");
			sequence.AdvanceSequence();
			if (!sequence.IsActive()) {
				//FOR ECGC



				GameObject.FindObjectOfType<AnchorUXController>().EnableInput();
				//To hide the buttons
				UpdateDroneButtons();

				//To check how the drone should be displayed now
				HandleDisplay();
				
				//To clear out any leftover names/text
				UpdateHighlightedNames();


				//BIG SPACE TO GET MY ATTENTION
				//Review the implementation of lessons and decide what to do
				//When lessons finish








				//Since we're manually starting sequences now, we'll disable this
				//GetComponentInParent<DroneManager>().GrabNextSequence();
				/*sequence = null;
				SetActive(false);*/
			} else {
				UpdatePlatform();
			}
		}
	}

	GameObject displayText;
	public bool showHighlightedNames;
	public TMPro.TextMeshPro highlightText;
	private void UpdateHighlightedNames()
	{
		if (!showHighlightedNames || !hasActiveSequence) {
			return;
		}

		if (highlightText != null) {
			MaterialSwitchState[] s = sequence.GetActiveStep()?.brainPiecesToHighlight;
			string text = "Highlighted:\n";
			if (s.Length == 0) {
				text = "";
			} else {
				for (int i = 0; i < s.Length; i++) {
					text += ((BrainElement)(s[i].GetComponent<SelectableElement>().selectable)).elementName + "\n";
				}
			}
			highlightText.text = text;
		} else {
			//Spawning only a predetermined name under one piece
			//Remove the old highlighted pieces' display
			if (displayText != null) {
				Destroy(displayText);
			}

			if (sequence.IsActive()) {
				MaterialSwitchState[] s = sequence.GetActiveStep()?.brainPiecesToHighlight;
				//Don't do anything if there's no text to display
				if (sequence.GetActiveStep()?.highlightedText.Length > 0) {
					//Instantiate the display text
					displayText = Instantiate(Resources.Load("Display Text"), s[0].transform.position, transform.rotation) as GameObject;

					//Set the name
					displayText.GetComponentInChildren<TMPro.TextMeshPro>().text = sequence.GetActiveStep()?.highlightedText;

					//Set the position
					displayText.transform.position = s[0].transform.position;
				}
			}

			/*
			//For spawning the name under each piece
			MaterialSwitchState[] s = sequence.GetActiveStep()?.brainPiecesToHighlight;
			//Remove the old highlighted pieces' names
			if (displayTexts != null) {
				for (int i = 0; i < displayTexts.Length; i++) {
					Destroy(displayTexts[i]);
				}
			}

			displayTexts = new GameObject[s.Length];
			for (int i = 0; i < s.Length; i++) {
				//Instantiate the display text
				displayTexts[i] = Instantiate(Resources.Load("Display Text")) as GameObject;

				//Set the name
				string name = ((BrainElement)(s[i].GetComponent<SelectableElement>().selectable)).elementName;
				displayTexts[i].GetComponentInChildren<TMPro.TextMeshPro>().text = name;

				//Set the position
				displayTexts[i].transform.position = s[i].transform.position;
			}
			*/
		}
	}

	/// <summary>
	/// Retretes the sequence to the previous area
	/// </summary>
	public void RecedeSequence()
	{
		if (ignoreSequence) {
			return;
		}
		if (sequence == null) {
			print("Null sequence");
		} else {
			//if at start, get previous lesson

			if (sequence.RecedeSequence()) {
				UpdatePlatform();
			} else {
				GetComponentInParent<DroneManager>().PreviousSequence();

			}
		}
	}

	public void ClearSequence()
	{
		sequence = null;
		UpdatePlatform();
	}

	public void ClearSelectable()
	{
		selection = null;
	}
#endregion


#region Movement

	SequenceElement.PlatformInformation info;
	bool moving = false;

	private IEnumerator MovePlatform()
	{
		if (!hasActiveSequence) {
			yield break;
		}
		info = sequence.GetActiveStep()?.GetPlatformInfo();
		if (info == null) {
			yield break;
		}
		float totalDuration = 5f;
		float _time = 0f;
		float lerpVal;
		Vector3 platformLoc = platform.position;
		Quaternion platformRotQ = platform.rotation;
		Vector3 platformScale = platform.localScale;

		Vector3 pos = platform.position;
		if (info.waypointLocation != Vector3.zero) {
			pos = info.waypointLocation;
		}

		bool
			posDone = false,
			rotDone = false,
			scaleDone = false;

		if (info.waypointLocation != Vector3.zero) {
			if (platform.position == info.waypointLocation) {
				//info.waypointLocation = Vector3.zero;
				posDone = true;
			}
		} else {
			posDone = true;
		}
		if (info.lookAtPoint != null) {
			if (platform.rotation == Quaternion.LookRotation(info.lookAtPoint.position - pos, Vector3.up)) {
				//info.lookAtPoint = null;
				rotDone = true;
			}
		} else {
			rotDone = true;
		}
		if (info.scaleVal > 0) {
			if (info.scaleVal == platformScale.x) {
				//info.scaleVal = 0;
				scaleDone = true;
			} else {
				platform.GetComponent<TweenScaleByFactor>().TweenToScale(info.scaleVal, totalDuration);
			}
		} else {
			scaleDone = true;
		}

		

		//Lerp to match the values over totalDuration
		moving = true;
		while (moving && _time < totalDuration && !(posDone && rotDone && scaleDone)) {
			_time += Time.deltaTime;
			lerpVal = _time / totalDuration;

			//Position
			if (!posDone) {
				if (info.waypointLocation != Vector3.zero) {
					platform.position = Vector3.Lerp(platformLoc, info.waypointLocation, lerpVal);
					if (platform.position == info.waypointLocation) {
						posDone = true;
					}
				}
			}

			//Scale is handled by TweenScaleByFactor, called above

			//Rotation
			if (!rotDone) {
				if (info.lookAtPoint != null) {
					pos = platform.position;
					if (info.waypointLocation != Vector3.zero) {
						pos = info.waypointLocation;
					}
					Quaternion lookAt = Quaternion.LookRotation(info.lookAtPoint.position - pos, Vector3.up);

					//Could use Quaternion.Lerp
					platform.rotation = Quaternion.Lerp(platformRotQ, lookAt, lerpVal);
					//platform.rotation = Quaternion.RotateTowards(platform.rotation, Quaternion.LookRotation(info.lookAtPoint.position - pos, Vector3.up), deltaAngle);
					pos = platform.rotation.eulerAngles;
					pos.x = 0;
					pos.z = 0;
					platform.eulerAngles = pos;

					if (platform.rotation == lookAt) {
						rotDone = true;
					}
				}
			}

			yield return null;
		}
		moving = false;
		info = null;
	}

	private void StopMovingPlatform(bool finishUpdating)
	{
		//Only called when advancing/receeding the sequence
		if (moving && finishUpdating) {
			if (info.waypointLocation != Vector3.zero) {
				platform.position = info.waypointLocation;
			}
			
			if (info.scaleVal > 0) {
				platform.localScale = Vector3.one * info.scaleVal;
			}

			if (info.lookAtPoint != null) {
				Quaternion lookAt = Quaternion.LookRotation(info.lookAtPoint.position - platform.position, Vector3.up);

				platform.rotation = lookAt;
				Vector3 eulers = platform.rotation.eulerAngles;
				eulers.x = 0;
				eulers.z = 0;
				platform.eulerAngles = eulers;
			}
		}
		moving = false;
		info = null;
		if (movePlatformCoroutine != null) {
			StopCoroutine(movePlatformCoroutine);
		}
	}
	
	/// <summary>
	/// Scales the x, y, and z of the input's position by the x, y, and z of the scaleVals
	/// </summary>
	/// <param name="input"></param>
	/// <param name="scaleVals"></param>
	/// <returns></returns>
	private Vector3 ScaleByVec3(Transform input, Vector3 scaleVals)
	{
		return
			input.right * scaleVals.x +
			input.up * scaleVals.y +
			input.forward * scaleVals.z;
	}

	private Vector3 GetGoalOffset(int idx)
	{
		Vector3 goal = potentialGoals[idx];
		return mainCamera.position + ScaleByVec3(mainCamera, goal);
	}

	private int PickClosestGoalIdx()
	{
		int lowestIdx = 0;
		float lowestDist = 0;
		for(int i = 0; i < potentialGoals.Count; i++) {
			Vector3 potentialGoalPos = mainCamera.position + ScaleByVec3(mainCamera, potentialGoals[i]);
			if (i == 0 || (transform.position - potentialGoalPos).sqrMagnitude < lowestDist) {
				lowestIdx = i;
				lowestDist = (transform.position - potentialGoalPos).sqrMagnitude;
			}
		}

		activeGoalIdx = lowestIdx;
		return lowestIdx;
	}

	#endregion
}
