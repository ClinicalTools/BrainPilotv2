using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalManagerManager : MonoBehaviour {

	SignalManager[] managers;
	public bool startOnAwake;
	private bool active;

	// Use this for initialization
	void Start () {
		managers = GetComponentsInChildren<SignalManager>();
		StopAll();
		active = false;
	}

	public void ToggleEmitter()
	{
		if (active) {
			StopAll();
		} else {
			PlayAll(false);
		}
	}

	public void PlayAll(bool includeNonLoops)
	{
		active = true;
		foreach (SignalManager manager in managers) {
			if (includeNonLoops || manager.loop) {
				manager.Play();
				manager.active = true;
			}
		}
	}

	public void Play(int i)
	{
		managers[i].Play();
		managers[i].active = true;
	}

	public void StopAll()
	{
		active = false;
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
