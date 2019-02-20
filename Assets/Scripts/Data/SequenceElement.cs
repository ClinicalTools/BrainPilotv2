using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Sequences/SequenceElement")]
public class SequenceElement : ScriptableObject {

	/**
	 * Possible data which would be needed by an event
	 * 
	 * This could include waypoint/location data, scale,
	 * events to call, etc.
	 */

	public Vector3 waypointLocation;
	//public Transform waypointLocation;

	public Transform lookAtPoint;
	//public Vector3 lookAtDirection;

	public float scaleVal;

	public SelectableElement[] piecesToActivate;
	public BrainElement[] piecesToActivate2;
	
	public UnityEvent OnEventBegin;
	public UnityEvent OnEventEnd;

	public string stepDisplayInfo;
	
	//This could help with pages of strings. We'd have control of the content
	//per page. Difficult to include images/other though
	//public string[] infoPages;

	public void Activate()
	{
		OnEventBegin.Invoke();
		foreach(SelectableElement element in piecesToActivate) {
			element?.GetComponent<MaterialSwitchState>()?.Activate();
		}
		foreach(BrainElement element in piecesToActivate2) {
			foreach(SelectableListener obj in element.listeners) {
				obj?.GetComponent<MaterialSwitchState>()?.Activate();
			}
		}
	}

	public void Deactivate()
	{
		OnEventEnd.Invoke();
		foreach (SelectableElement element in piecesToActivate) {
			element?.GetComponent<MaterialSwitchState>()?.Deactivate();
		}
		foreach (BrainElement element in piecesToActivate2) {
			foreach (SelectableListener obj in element.listeners) {
				obj?.GetComponent<MaterialSwitchState>()?.Deactivate();
			}
		}
	}
}
