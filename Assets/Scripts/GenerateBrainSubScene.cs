using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class GenerateBrainSubScene {

	const string menuText = "Create Brain Sub Scene";
	const string savePath = "Assets/Scenes/Layered Scenes";
	const string brainLocation = "Assets/_Prefabs/BrainModel.prefab";
	const string sequenceLocation = "Assets/_Prefabs/SequenceManager.prefab";

#if UNITY_EDITOR
	[MenuItem("Brain Scene/" + menuText)]
	public static void GenerateScene()
	{
		Scene temp = SceneManager.GetActiveScene();
		Scene s = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
		SceneManager.SetActiveScene(s);
		try {
			Object o = AssetDatabase.LoadAssetAtPath(brainLocation, typeof(Object));
			GameObject.Instantiate(o).name = "BrainModel";
		} catch (System.ArgumentException e) {
			Debug.LogError("No brain model prefab found at path: " + brainLocation);
		}

		try {
			Object o = AssetDatabase.LoadAssetAtPath(sequenceLocation, typeof(Object));
			GameObject.Instantiate(o).name = "SequenceManager";
		} catch (System.ArgumentException e) {
			Debug.LogError("No sequence manager found at path: " + sequenceLocation);
		}
	}
#endif
}
