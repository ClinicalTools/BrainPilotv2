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
	private LineCastSelector selector;

	public TMPro.TextMeshPro textField;

	private bool _active = false;
	private float distance;
	private Vector3 goalLoc;
	private MeshRenderer mesh;
	[SerializeField]
	private Selectable selection;
	[SerializeField]
	private Sequence sequence;

	public bool gazeBased;
	//Used without gaze
	private int activeGoalIdx;


	// Use this for initialization
	void Start () {
		OVRManager.tiledMultiResLevel = OVRManager.TiledMultiResLevel.LMSMedium;
		settings.maxDistance *= settings.maxDistance;
		mesh = GetComponent<MeshRenderer>();

		if (!gazeBased) {
			mainCamera = transform.parent;
		}
	}

	/**
	 * GazeBased is current implementation
	 * Otherwise will have the drone spawn in one of the predetermined
	 * spots and not move until the Move button is pressed.
	 * Then it will recalculate the closest goal.
	 */
	
	// Update is called once per frame
	void Update () {
		if (gazeBased && _active) {
			//Rotate the drone to face the player
			transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.position);

			//Move the drone towards the nearest goal location
			//goalLoc = mainCamera.position + mainCamera.forward * 3 + mainCamera.right * 3;
			goalLoc = GetGoalOffset(PickClosestGoalIdx());
			Debug.DrawLine(mainCamera.position, goalLoc);
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

			//Update the text on the drone
			UpdateText();
		} else if (_active) {
			transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.position);

			UpdateText();
		}
	}

	public void SetActive(bool active)
	{
		_active = active;
		StartCoroutine(FadeMesh(_active ? 0f : 1f));

		if (!gazeBased) {
			if (!_active) {
				goalLoc = Vector3.zero;
			} else {
				TeleportToClosestGoal();
			}
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
		}
	}

	/// <summary>
	/// Fades the Disolve shader in or out.
	/// 1 = invisible
	/// </summary>
	/// <param name="endVal"></param>
	private IEnumerator FadeMesh(float endVal, float duration = 2f)
	{
		float val = mesh.sharedMaterial.GetFloat("_DissolveCutoff");
		float lerpVal = 0f;
		float interval = Time.deltaTime / settings.fadeDuration;
		while (val != endVal) {
			lerpVal += interval;
			val = Mathf.Lerp(val, endVal, lerpVal);
			mesh.sharedMaterial.SetFloat("_DissolveCutoff", val);
			yield return null;
		}
	}

#region TextDisplay
	//This should be put into a listener or something I guess.
	private void UpdateText()
	{
		if (sequence != null && sequence.IsActive()) {
			textField.text = sequence.GetActiveStep().stepDisplayInfo;
		} else if (selection != null && selection is BrainElement) {
			//Update with a description of the selected brain piece
			textField.text = ((BrainElement)selection).description;
		} else {
			//Other uses
		}
	}

	public void SetSelectable(Selectable selectable)
	{
		selection = selectable;
	}

	/// <summary>
	/// Loads a sequence from the start
	/// </summary>
	/// <param name="s"></param>
	public void BeginSequence(Sequence s)
	{
		if (!_active) {
			SetActive(true);
		}
		sequence = s;
		sequence.ResetSequence();
		sequence.StartSequence();
		UpdateText();
	}

	/// <summary>
	/// Loads a sequence and resumes it.
	/// </summary>
	/// <param name="s">The sequence to run</param>
	public void ResumeSequence(Sequence s)
	{
		if (!_active) {
			SetActive(true);
		}
		sequence = s;
		sequence.StartSequence();
		UpdateText();
	}

	/// <summary>
	/// Advances the active sequence to the next area
	/// </summary>
	public void AdvanceSequence()
	{
		if (sequence == null) {
			print("Null sequence");
		} else {
			print("Advancing sequence");
			sequence.AdvanceSequence();
			if (!sequence.IsActive()) {
				sequence = null;
				SetActive(false);
			} else {
				UpdateText();
			}
		}
	}

	/// <summary>
	/// Retretes the sequence to the previous area
	/// </summary>
	public void RecedeSequence()
	{
		if (sequence == null) {
			print("Null sequence");
		} else {
			sequence.RecedeSequence();
			UpdateText();
		}
	}

	public void ClearSelectable()
	{
		selection = null;
	}
#endregion


#region Movement

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
