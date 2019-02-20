using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DroneListener : MonoBehaviour {

	public DroneController drone;
	public UnityEvent asdfqwer;
	public List<Sequence> events;

	private void OnEnable()
	{
		//somecontroller.RegisterListener(this);
	}

	private void OnDisable()
	{
		//somecontroller.UnregisterListener(this);
	}

	public void Invoke()
	{
		
	}
}
