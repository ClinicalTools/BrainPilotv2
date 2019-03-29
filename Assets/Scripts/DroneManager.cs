using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//ActiveSceneController
public class DroneManager : SubSceneListener {

	public DroneController drone;
	public SequenceManager sequenceManager;
	public SignalManagerManager signalManager;
	private int activeScene => subSceneManager.activeScene;
	private bool started = false;

	// Use this for initialization
	void Start () {
		if (drone == null) {
			drone = GetComponentInChildren<DroneController>();
		}
		if (subSceneManager == null) {
			RetrieveSubSceneManager();
		}
		subSceneManager.RegisterListener(this);
		subSceneManager.activeScene = 1;
		Invoke("GrabActiveSequence", 2f);
	}

	public void GrabActiveSequence()
	{
		GrabSequenceAt(activeScene);
	}

	public void GrabNextSequence()
	{
		GrabSequenceAt(activeScene + 1);
	}

	public void GrabPreviousSequence()
	{
		GrabSequenceAt(activeScene - 1);
	}





	public void GrabSequenceAt(int sceneIdx)
	{
		if (!started || sceneIdx != subSceneManager.activeScene) {
			signalManager?.StopAll();
			subSceneManager.UpdateSelectedScene(sceneIdx);
		}
		started = true;
		LoadLesson();
		LoadSignals();
	}

	private void LoadLesson()
	{
		if (activeScene >= SceneManager.sceneCount) {
			HideDrone();
			return;
		} else if (activeScene <= 0) {
			subSceneManager.activeScene = 1;
		}
		drone.ResumeSequence(sequenceManager.GetSequence());
		
		//Handled from Invoke
		/*
		//Parse through the scene's game objects looking for what we want
		foreach (GameObject obj in objs) {
			if (obj.GetComponent<SequenceManager>()) {
				sequenceManager = obj.GetComponent<SequenceManager>();
				drone.ResumeSequence(sequenceManager.GetSequence());
			}
		}*/
	}

	private void LoadSignals()
	{
		signalManager.PlayAll();
	}

	public void HideDrone()
	{
		drone.SetActive(false);
	}

	public override void Invoke()
	{
		if (subSceneManager == null) {
			print("I was null");
			RetrieveSubSceneManager();
		}
		sequenceManager = subSceneManager.sequenceManager;
		signalManager = subSceneManager.signalManager;
	}

	
}
