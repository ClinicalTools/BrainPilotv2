using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class BrainSceneReferences : ScriptableObject {

	public string title;
	public string description;

	public List<SceneField> sceneReferences = new List<SceneField>();

	public SceneField GetScene(string sceneName)
	{
		return sceneReferences[0];
	}

	public SceneField GetScene(int buildIdx)
	{
		return sceneReferences[0];
	}
}
