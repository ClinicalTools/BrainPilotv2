using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class BeginLesson : MonoBehaviour {

	/*
	 * Unity doesn't provide an inspector slot for Scene without a custom editor.
	 * A custom editor will be the end solution, but for now, use strings
	 * as a prototype
	 */

	//public List<string> scenesToLoad;
	public BrainSceneReferences scenesToLoad;

	[ContextMenu("Switch Scenes")]
	public void SwitchScenes()
	{
		//Scene mainScene = SceneManager.GetSceneByName("Main Scene");
		SceneAsset a = AssetDatabase.LoadAssetAtPath<SceneAsset>("asdf");
		Scene mainScene = SceneManager.GetSceneByPath(AssetDatabase.GetAssetPath(a));
		if (mainScene.buildIndex == -1) {
			List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
			buildScenes.Add(new EditorBuildSettingsScene(mainScene.path, true));
			EditorBuildSettings.scenes = buildScenes.ToArray();
		}


		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainScene.buildIndex, LoadSceneMode.Single);
		asyncLoad.allowSceneActivation = false;
		foreach (SceneField s in scenesToLoad.sceneReferences) {
			//SceneManager.LoadScene(s.SceneName, LoadSceneMode.Additive);
#if UNITY_EDITOR
			EditorSceneManager.OpenScene(EditorSceneManager.GetSceneByName(s.SceneName).path, OpenSceneMode.Additive);
#else
			SceneManager.LoadScene(s.SceneName, LoadSceneMode.Additive);
#endif
		}
		SceneManager.SetActiveScene(mainScene);
		asyncLoad.allowSceneActivation = true;
	}

	public void SetSceneGroup(BrainSceneReferences scenes)
	{
		scenesToLoad = scenes;
	}
}
