using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DroneManager : MonoBehaviour {

	public DroneController drone;
	public SequenceManager sequenceManager;
	int activeScene = 1;

	// Use this for initialization
	void Start () {
		if (drone == null) {
			drone = GetComponentInChildren<DroneController>();
		}
		Invoke("GrabActiveSequence", 2f);
	}

	public void GrabActiveSequence()
	{
		GrabSequenceAt(activeScene);
	}

	public void GrabNextSequence()
	{
		activeScene++;
		GrabSequenceAt(activeScene);
	}

	public void GrabPreviousSequence()
	{
		activeScene--;
		GrabSequenceAt(activeScene);
	}

	public void GrabSequenceAt(int sceneIdx)
	{
		if (sceneIdx >= SceneManager.sceneCount) {
			HideDrone();
			return;
		} else if (sceneIdx <= 0) {
			sceneIdx = 1;
			activeScene = 1;
		}
		GameObject[] objs = SceneManager.GetSceneAt(sceneIdx).GetRootGameObjects();

		//Parse through the scene's game objects looking for what we want
		foreach(GameObject obj in objs) {
			if (obj.GetComponent<SequenceManager>()) {
				sequenceManager = obj.GetComponent<SequenceManager>();
				drone.ResumeSequence(sequenceManager.GetSequence());
			}
		}
	}

	public void HideDrone()
	{
		drone.SetActive(false);
	}
}
