using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class DroneListener : MonoBehaviour {

	public DroneData data;
	public SelectableTargetEvent activeDataEvent;
	public SelectableTargetEvent deactivateDataEvent;

	private void OnEnable()
	{
		//somecontroller.RegisterListener(this);
#if UNITY_EDITOR

		string[] guids = UnityEditor.AssetDatabase.FindAssets("t:DroneData", new string[] { "Assets/Data" });
		if (guids.Length > 0) 
			data = UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]), typeof(DroneData)) as DroneData;
#endif

		data?.RegisterListener(this);
	}

	private void OnDisable()
	{
		//somecontroller.UnregisterListener(this);
		data?.UnregisterListener(this);
	}

	Selectable updatingWith;

	public void UpdateDataSelectable(Selectable s)
	{
		updatingWith = s;
		if (s == null || s is UIElement) {

			s = null;
		}
		data.UpdateSelectable(s);
	}

	public void Invoke(bool active)
	{
		if (active) {
			activeDataEvent.Invoke(data.selection);
		} else {
			deactivateDataEvent.Invoke(updatingWith);
		}
	}

	public void Highlight(Selectable s)
	{
		data.HighlightSelected(s);
	}

	public void Highlight(bool b)
	{
		data.HighlightSelected(b);
	}
}
