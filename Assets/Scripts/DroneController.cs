using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DroneController))]
public class DroneControllerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var controller = (DroneController)target;
		if (GUILayout.Button("Activate")) {
			controller.Activate();
		}
		if (GUILayout.Button("Deactivate")) {
			controller.Deactivate();
		}
	}
}

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
	private Vector3[] potentialGoals;
	[SerializeField]
	private Transform mainCamera;
	[SerializeField]
	private LineCastSelector selector;

	public TMPro.TextMeshPro textField;

	private bool _active = false;
	private float distance;
	private Vector3 goalLoc;
	private MeshRenderer mesh;
	private Selectable selection;
	private Sequence sequence;


	// Use this for initialization
	void Start () {
		settings.maxDistance *= settings.maxDistance;
		mesh = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (_active) {
			transform.LookAt(mainCamera);

			//goalLoc = mainCamera.position + mainCamera.forward * 3 + mainCamera.right * 3;
			goalLoc = GetClosestGoalOffset();

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
			if (selection != null) {
				UpdateText();
			}
		}
	}

	

	public void SetSelectable(Selectable selectable)
	{
		selection = selectable;
	}

	public void RunSequence(Sequence s)
	{
		sequence = s;
		sequence.StartSequence();
	}

	public void ClearSelectable()
	{
		selection = null;
	}

	//This should be put into a listener or something I guess.
	private void UpdateText()
	{
		if (sequence != null && sequence.IsActive()) {
			textField.text = sequence.GetActiveStep().stepDisplayInfo;
		} else if (selection is BrainElement) {
			//Update with a description of the selected brain piece
			textField.text = ((BrainElement)selection).description;
		} else {
			//Other uses
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

	private Vector3 GetClosestGoalOffset()
	{
		Vector3 goal = potentialGoals[PickClosestGoalIdx()];
		return mainCamera.position + ScaleByVec3(mainCamera, goal);
	}

	private int PickClosestGoalIdx()
	{
		int lowestIdx = 0;
		float lowestDist = 0;
		for(int i = 0; i < potentialGoals.Length; i++) {
			Vector3 potentialGoalPos = mainCamera.position + ScaleByVec3(mainCamera, potentialGoals[i]);
			if (i == 0 || (transform.position - potentialGoalPos).sqrMagnitude < lowestDist) {
				lowestIdx = i;
				lowestDist = (transform.position - potentialGoalPos).sqrMagnitude;
			}
		}
		return lowestIdx;
	}

	public void Activate()
	{
		_active = true;
		StartCoroutine(FadeMesh(0f));
	}

	public void Deactivate()
	{
		_active = false;
		StartCoroutine(FadeMesh(1f));
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
}
