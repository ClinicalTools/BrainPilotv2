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
	public SceneField mainScene = null;

	//public List<string> scenesToLoad;
	public BrainSceneReferences scenesToLoad;
	public static bool loadingScenes = false;

	[ContextMenu("Switch Scenes")]
	public void SwitchScenes()
	{
		if (loadingScenes) {
			return;
		}
		if (mainScene == null || mainScene.SceneName.Length == 0) {
			Debug.LogError("You must specify the base scene to load first!");
			return;
		}
		loadingScenes = true;
		try {
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainScene.SceneName, LoadSceneMode.Single);
			asyncLoad.allowSceneActivation = false;
			if (scenesToLoad != null) {
				foreach (SceneField s in scenesToLoad.sceneReferences) {
#if	UNITY_EDITOR
					if (!ContainsScene(s.SceneName)) {
						EditorBuildSettingsScene addedScene = AddSceneToBuildList(s);
						List<EditorBuildSettingsScene> list = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
						int i = 0;
						while (list.Find((EditorBuildSettingsScene x) => x.path == addedScene.path) != null) {
							i++;
							if (i > 5000) {
								break;
							}
							continue;
						}
					}
#endif
					SceneManager.LoadScene(s.SceneName, LoadSceneMode.Additive);
					//#endif
				}
			}
			loadingScenes = false;
			asyncLoad.allowSceneActivation = true;
		} catch (System.NullReferenceException) {
			loadingScenes = false;
			return;
		}
	}

	public void SetSceneGroup(BrainSceneReferences scenes)
	{
		scenesToLoad = scenes;
	}

#if UNITY_EDITOR
	public bool ContainsScene(string sceneName)
	{
		List<EditorBuildSettingsScene> list = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
		Debug.Log(sceneName);
		foreach(EditorBuildSettingsScene scene in list) {
			if (scene.path.EndsWith(sceneName + ".unity")) {
				return true;
			}
		}
		return false;
	}

	[ContextMenu("Run Test")]
	public EditorBuildSettingsScene AddSceneToBuildList2()
	{
		string sceneName = "New Scene";
		Debug.Log(ContainsScene(sceneName));
		List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
		Debug.Log(sceneName);
		List<string> paths = new List<string>(AssetDatabase.FindAssets(sceneName + " t:Scene", new[] { "Assets/Scenes/Layered Scenes" }));
		for(int i = 0; i < paths.Count; i++) {
			paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
			Debug.Log(paths[i]);
		}
		string assetPath = paths.Find((string x) => x.EndsWith(sceneName + ".unity"));
		Debug.Log(assetPath);
		EditorBuildSettingsScene newScene = new EditorBuildSettingsScene(assetPath, true);
		buildScenes.Add(newScene);
		EditorBuildSettings.scenes = buildScenes.ToArray();
		return newScene;
	}

	public EditorBuildSettingsScene AddSceneToBuildList(SceneField scene)
	{
		List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
		List<string> paths = new List<string>(AssetDatabase.FindAssets(scene.SceneName + " t:Scene", new[] { "Assets/Scenes/Layered Scenes" }));
		for (int i = 0; i < paths.Count; i++) {
			paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
		}
		string assetPath = paths.Find((string x) => x.EndsWith(scene.SceneName + ".unity"));
		EditorBuildSettingsScene newScene = new EditorBuildSettingsScene(assetPath, true);
		buildScenes.Add(newScene);
		EditorBuildSettings.scenes = buildScenes.ToArray();
		return newScene;
	}
#endif
}
