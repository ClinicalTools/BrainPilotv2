using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DroneManager : MonoBehaviour {

	public DroneController drone;
	public SequenceManager sequenceManager;

	// Use this for initialization
	void Start () {
		if (drone == null) {
			drone = GetComponentInChildren<DroneController>();
		}
		Invoke("GrabSequence", 2f);
	}
	
	public void GrabSequence()
	{
		GameObject[] objs = SceneManager.GetSceneAt(1).GetRootGameObjects();
		foreach(GameObject obj in objs) {
			if (obj.GetComponent<SequenceManager>()) {
				print("Got em");
				sequenceManager = obj.GetComponent<SequenceManager>();
				drone.BeginSequence(sequenceManager.GetSequence());
			}
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
