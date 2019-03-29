using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
public class GenerateBrainSubScene {

	const string menuText = "Create Brain Sub Scene";
	const string savePath = "Assets/Scenes/Layered Scenes";
	const string brainLocation = "Assets/_Prefabs/BrainModel.prefab";
	const string sequenceLocation = "Assets/_Prefabs/SequenceManager.prefab";
	const string particleManagerLocation = "Assets/_Prefabs/ParticleManager.prefab";

	private static Dictionary<System.Type, GameObject> references;// = new Dictionary<System.Type, GameObject>();


#if UNITY_EDITOR
	[MenuItem("Brain Scene/" + menuText)]
	public static void GenerateScene()
	{
		Scene temp = SceneManager.GetActiveScene();
		Scene s = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
		SceneManager.SetActiveScene(s);
		references = new Dictionary<System.Type, GameObject>();

		try {
			Object o = AssetDatabase.LoadAssetAtPath(brainLocation, typeof(Object));
			GameObject obj = (GameObject)GameObject.Instantiate(o);
			obj.name = "BrainModel";
			references.Add(typeof(Transform), obj);
		} catch (System.ArgumentException) {
			Debug.LogError("No brain model prefab found at path: " + brainLocation);
		}

		try {
			Object o = AssetDatabase.LoadAssetAtPath(sequenceLocation, typeof(Object));
			GameObject obj = (GameObject)GameObject.Instantiate(o);
			obj.name = "SequenceManager";
			references.Add(typeof(SequenceManager), obj);
		} catch (System.ArgumentException) {
			Debug.LogError("No sequence manager found at path: " + sequenceLocation);
		}

		try {
			Object o = AssetDatabase.LoadAssetAtPath(particleManagerLocation, typeof(Object));
			GameObject obj = (GameObject)GameObject.Instantiate(o);
			obj.name = "ParticleManager";
			//references.Add(typeof(ParticleManager), obj);
		} catch (System.ArgumentException) {
			Debug.LogError("No sequence manager found at path: " + sequenceLocation);
		}		
	}

	public static GameObject GetObjectOfType(System.Type type)
	{
		return references[type];
	}

	[MenuItem("Brain Scene/" + "Bake All Lighting")]
	public static void BakeAllLighting()
	{
		s = new List<string>(AssetDatabase.FindAssets("t:scene", new[] { "Assets/Scenes/Layered Scenes" }));
#if false
			previouslyActive = SceneManager.GetActiveScene();
			Lightmapping.BakeMultipleScenes(s.ToArray());
#else
			EditorApplication.update += Update;
#endif
	}

	[MenuItem("Brain Scene/" + "Bake Active Lighting")]
	public static void BakeActiveLighting()
	{
		previouslyActive = SceneManager.GetActiveScene();

		activeSceneList = new List<Scene>();
		for (int i = 0; i < SceneManager.sceneCount; i++) {
			activeSceneList.Add(SceneManager.GetSceneAt(i));
			//EditorSceneManager.CloseScene(activeSceneList[i], true);
			//SceneManager.UnloadSceneAsync(activeSceneList[i]);
		}
		activeSceneListCopy = new List<Scene>(activeSceneList);

		s = new List<string>();
		foreach (Scene asdf in activeSceneList) {
			s.Add(asdf.path);
			Debug.Log(asdf.path);
		}
		//EditorApplication.update -= UpdateActive;


#if false
		Lightmapping.BakeMultipleScenes(s.ToArray());
#else
		EditorApplication.update += UpdateActive;
#endif
	}

	private static bool lightmapFinished;
	private static List<string> s;
	private static List<Scene> activeSceneList;
	private static List<Scene> activeSceneListCopy;
	private static Scene previouslyActive;
	

	private static void Update()
	{
		if (!Lightmapping.isRunning) {
			if (s.Count == 0) {
				EditorApplication.update -= Update;
				SceneManager.SetActiveScene(previouslyActive);
				return;
			}
			string assetPath = AssetDatabase.GUIDToAssetPath(s[0]);
			s.RemoveAt(0);
			Scene scene = SceneManager.GetSceneByPath(assetPath);
			if(!scene.isLoaded) {
				EditorSceneManager.OpenScene(assetPath, OpenSceneMode.Additive);
			}
			StartLightmap(scene);
		}
	}

	private static bool lightmapRunning;
	static int i = 0;
	private static void UpdateActive()
	{
		if (lightmapRunning) {
			if (!Lightmapping.isRunning) {
				lightmapRunning = false;
			}
			return;
		}
		
		i++;
		if (i > 10) {
			EditorApplication.update -= UpdateActive;
			return;
		}

		if (!Lightmapping.isRunning) {
			if (activeSceneList.Count == 0) {
				EditorApplication.update -= UpdateActive;

				//Reload all active scenes
				foreach(Scene scene in activeSceneListCopy) {
					EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
				}
				SceneManager.SetActiveScene(previouslyActive);
				return;
			}
			Debug.Log((activeSceneList == null) + ", " + (activeSceneList[0] == null) + ", " + activeSceneList[0].path);
			EditorSceneManager.OpenScene(activeSceneList[0].path, OpenSceneMode.Single);
			StartLightmap(activeSceneList[0]); 
			activeSceneList.RemoveAt(0);
			lightmapRunning = true;
		}
	} 

	private static void StartLightmap(Scene s)
	{
		//SceneManager.SetActiveScene(s);
		Lightmapping.Bake();
	}
#endif
	}
