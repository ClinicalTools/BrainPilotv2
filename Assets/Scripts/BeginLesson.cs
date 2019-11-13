using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginLesson : MonoBehaviour {

	public SceneField mainScene = null;

	//public List<string> scenesToLoad;
	public BrainSceneReferences scenesToLoad;
	public static bool loadingScenes = false;
	public bool useLoadingScreen = false;
	public OVRScreenFade fade;
	public Camera loadingScreenCamera;
	public CanvasGroup loadingScreen;

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
		if (useLoadingScreen) {
			StartCoroutine(SwitchScenesCoroutine());
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
					//SceneManager.LoadScene(s.SceneName, LoadSceneMode.Additive);
					SceneManager.LoadSceneAsync(s.SceneName, LoadSceneMode.Additive).allowSceneActivation = true;
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

	/// <summary>
	/// Used in conjunction with a loading screen
	/// </summary>
	/// <returns></returns>
	private IEnumerator SwitchScenesCoroutine()
	{
		loadingScenes = true;
		//Fade screen
		fade.fadeTime = 1f;
		fade.FadeOut();
		int count = 0;
		while (fade.currentAlpha < 1) {
			count++;
			if (count > 200) {
				yield break;
			}
			yield return null;
		}
		FindObjectOfType<OVRManager>().gameObject.SetActive(false);
		loadingScreenCamera.gameObject.SetActive(true);
		Application.backgroundLoadingPriority = ThreadPriority.Low;
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainScene.SceneName, LoadSceneMode.Single);
		asyncLoad.allowSceneActivation = false;
		StartCoroutine(FadeLoadingScreen(asyncLoad));
		if (scenesToLoad != null) {
			foreach (SceneField s in scenesToLoad.sceneReferences) {
				if (s == null) {
					Debug.LogWarning("Scene is null!");
					continue;
				}
				AsyncOperation async = SceneManager.LoadSceneAsync(s.SceneName, LoadSceneMode.Additive);
				async.allowSceneActivation = true;
				yield return new WaitForSeconds(1.5f);
			}
		}
		loadingScenes = false;
		loadingScreenCamera.GetComponent<OVRScreenFade>()?.FadeOut();
		//asyncLoad.allowSceneActivation = true;
	}

	private IEnumerator FadeLoadingScreen(AsyncOperation asyncLoad)
	{
		float minAlpha = .2f;
		/*IEnumerator FadeIn() {
			while (loadingScreen.alpha < 1) {
				loadingScreen.alpha += Time.deltaTime;
				yield return null;
			}
			loadingScreen.alpha = 1f;
		};
		IEnumerator FadeToMin() {
			while (loadingScreen.alpha > minAlpha) {
				yield return null;
				loadingScreen.alpha -= Time.deltaTime;
			}
			loadingScreen.alpha = minAlpha;
		}
		IEnumerator FadeOut()
		{
			while (loadingScreen.alpha > 0) {
				yield return null;
				loadingScreen.alpha -= Time.deltaTime;
			}
			loadingScreen.alpha = 0;
		}
		*/
		while (loadingScenes) {
			//Fade in loading screen
			while (loadingScreen.alpha < 1) {
				loadingScreen.alpha += Time.deltaTime;
				yield return null;
			}

			//Fade out loading screen
			while (loadingScreen.alpha > minAlpha) {
				yield return null;
				loadingScreen.alpha -= Time.deltaTime;
			}
			loadingScreen.alpha = minAlpha;
		}
		while (loadingScreen.alpha > 0) {
			yield return null;
			loadingScreen.alpha -= Time.deltaTime;
		}
		loadingScreen.alpha = 0;
		asyncLoad.allowSceneActivation = true;
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

	[CustomEditor(typeof(BeginLesson))]
	private class BeginLessonEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Load Lesson")) {
				((BeginLesson)target).SwitchScenes();
			}
		}
	}
#endif
}
