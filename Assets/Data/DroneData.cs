using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DroneData : ScriptableObject {

	public Selectable selection;
	public Sequence sequence;

	public List<DroneListener> list;

	public void RegisterListener(DroneListener listener)
	{
		if (list == null) {
			list = new List<DroneListener>();
		}
		if (!list.Contains(listener)) {
			list.Add(listener);
		}
	}

	public void UnregisterListener(DroneListener listener)
	{
		if (list == null) {
			list = new List<DroneListener>();
		}
		if (list.Contains(listener)) {
			list.Remove(listener);
		}
	}
	
	public void UpdateSelectable(Selectable s)
	{
		foreach (DroneListener listener in list) {
			listener.Invoke(false);
		}
		selection = s;
		foreach(DroneListener listener in list) {
			listener.Invoke(true);
		}
	}

	public void HighlightSelected(Selectable selected)
	{
		if (selection == null) {
			return;
		}
		if (selected == selection) {
			//Highlight is being called after the selection has been updated
			foreach(SelectableListener l in selection.listeners) {
				l?.GetComponent<MaterialSwitchState>()?.Activate();
			}
		} else {
			foreach (SelectableListener l in selection.listeners) {
				l?.GetComponent<MaterialSwitchState>()?.Deactivate();
			}
		}
	}
}
