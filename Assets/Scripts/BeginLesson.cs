using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginLesson : MonoBehaviour {

	/*
	 * Unity doesn't provide an inspector slot for Scene without a custom editor.
	 * A custom editor will be the end solution, but for now, use strings
	 * as a prototype
	 */

	public List<string> scenesToLoad;

	[ContextMenu("Switch Scenes")]
	public void SwitchScenes()
	{
		Scene mainScene = SceneManager.GetSceneByName("Main Scene");
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainScene.buildIndex, LoadSceneMode.Single);
		asyncLoad.allowSceneActivation = false;
		foreach (string s in scenesToLoad) {
			SceneManager.LoadScene(s, LoadSceneMode.Additive);
		}
		SceneManager.SetActiveScene(mainScene);
		asyncLoad.allowSceneActivation = true;
	}

	public void AddSceneToLoad(string s)
	{
		scenesToLoad.Add(s);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
