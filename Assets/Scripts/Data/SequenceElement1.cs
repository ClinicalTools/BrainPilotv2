using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SequenceElement1 : MonoBehaviour {

	/**
	 * Possible data which would be needed by an event
	 * 
	 * This could include waypoint/location data, scale,
	 * events to call, etc.
	 */
	[System.Serializable]
	private class PlatformInformation
	{
		public Vector3 waypointLocation;
		//public Transform waypointLocation;

		public Transform lookAtPoint;
		//public Vector3 lookAtDirection;

		public float scaleVal;
	}


	[SerializeField]
	private PlatformInformation platformInformation;

	public string textToDisplay;

	public MaterialSwitchState[] brainPiecesToHighlight;
	
	public UnityEvent OnEventBegin;
	public UnityEvent OnEventEnd;


	//This could help with pages of strings. We'd have control of the content
	//per page. Difficult to include images/other though
	//public string[] infoPages;

	private void OnEnable()
	{
		//Check to see if the loaded element is in the active part
		if (gameObject.scene == UnityEngine.SceneManagement.SceneManager.GetActiveScene()) {
			//Set the sequence as the drone's chosen sequence
		}
	}

	public void Activate()
	{
		OnEventBegin.Invoke();
		HighlightBrainPieces();
		HandlePlatformMovement();
	}

	public void Deactivate()
	{
		OnEventEnd.Invoke();
		UnhighlightBrainPieces();
	}

	public void HighlightBrainPieces()
	{
		foreach (MaterialSwitchState element in brainPiecesToHighlight) {
			element?.Activate();
		}
	}

	public void UnhighlightBrainPieces()
	{
		foreach (MaterialSwitchState element in brainPiecesToHighlight) {
			element?.Deactivate();
		}
	}

	public void HandlePlatformMovement()
	{
		StartCoroutine(IterateMovement());
	}

	private IEnumerator IterateMovement()
	{
		float time = 0;
		float totalTime = 3f;
		while (time < totalTime) {
			/*LerpMovement();
			LerpRotation();
			LerpScale(); */
			time += Time.deltaTime;
			yield return null;
		}
	}
}
