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
	public bool tutorial = true;

	// Use this for initialization
	void Start () {
		if (drone == null) {
			drone = GetComponentInChildren<DroneController>();
		}
		if (subSceneManager == null) {
			RetrieveSubSceneManager();
		}
		subSceneManager.RegisterListener(this);
		UnityEngine.Application.quitting += delegate { subSceneManager.UnregisterListener(this); };
		subSceneManager.activeScene = 1;
		Invoke("GrabActiveSequence", 2f);
	}

	public void GrabActiveSequence()
	{
		GetSceneSequences(activeScene);
	}

	public void GrabNextSequence()
	{
		if (sequenceManager.isAtEnd) {
			if (tutorial) {
				tutorial = false;
				drone.PauseSequence();
			} else {
				GetSceneSequences(activeScene + 1);
			}
		} else {
			if (tutorial) {
				tutorial = false;
				drone.PauseSequence();
			} else {
				sequenceManager.AdvanceSequence();
				LoadLesson();
			}
		}
	}

	public void GrabPreviousSequence()
	{
		if (sequenceManager.isAtBeginning) {
			GetSceneSequences(activeScene - 1);
		} else {
			sequenceManager.ReceedSequence();
			LoadLesson();
		}
	}

	public void GrabSequenceAt(int i)
	{
		for(int a = 0; a<i; a++) {
			if (a + sequenceManager.count < i) {
				a += sequenceManager.count - 1;
				GetSceneSequences(activeScene + 1);
			} else {
				sequenceManager.AdvanceSequence();
			}
		}
		LoadLesson();
	}

	public void GetSceneSequences(int sceneIdx)
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
			drone.ClearSequence();
			//HideDrone();
			return;
		} else if (activeScene <= 0) {
			subSceneManager.activeScene = 1;
		}
		drone.ResumeSequence(sequenceManager.GetSequence());
		if (!tutorial) {
			GameObject.FindObjectOfType<AnchorUXController>().DisableInput();
		} else {
			GameObject.FindObjectOfType<AnchorUXController>().EnableInput();
		}

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
		if (signalManager != null && signalManager.startOnAwake) {
			signalManager.PlayAll(false);
		}
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
