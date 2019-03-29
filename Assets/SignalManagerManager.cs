using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalManagerManager : MonoBehaviour {

	SignalManager[] managers;

	// Use this for initialization
	void Start () {
		managers = GetComponentsInChildren<SignalManager>();
		StopAll();
	}

	public void PlayAll()
	{
		foreach (SignalManager manager in managers) {
			manager.Play();
			manager.active = true;
		}
	}

	public void Play(int i)
	{
		managers[i].Play();
		managers[i].active = true;
	}

	public void StopAll()
	{
		foreach (SignalManager manager in managers) {
			manager.StopAll();
			manager.active = false;
		}
	}

	public void Stop(int i)
	{
		managers[i].StopAll();
		managers[i].active = false;
	}
}
