using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class DroneListener : MonoBehaviour {

	public DroneData data;
	public SelectableTargetEvent dataEvent;

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

	public void UpdateDataSelectable(Selectable s)
	{
		data.UpdateSelectable(s);
	}

	public void Invoke()
	{
		dataEvent.Invoke(data.selection);
	}
}
