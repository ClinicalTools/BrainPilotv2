using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class SubSceneManager : ScriptableObject {

	[SerializeField]
	public int activeScene = 1;
	public SequenceManager sequenceManager;
	public SignalManagerManager signalManager;

	private List<SubSceneListener> listeners;

	public void RegisterListener(SubSceneListener listener)
	{
		if (listeners == null) {
			listeners = new List<SubSceneListener>();
		}
		listeners.Add(listener);
	}

	public void UnregisterListener(SubSceneListener listener)
	{
		if (listeners == null) {
			listeners = new List<SubSceneListener>();
		}
		listeners.Remove(listener);
	}
	
	public void LoadSequence(int sceneIdx)
	{
		if (sceneIdx >= SceneManager.sceneCount) {
			return;
		} else if (sceneIdx <= 0) {
			sceneIdx = 1;
			activeScene = 1;
		}
		GameObject[] objs = SceneManager.GetSceneAt(sceneIdx).GetRootGameObjects();

		//Parse through the scene's game objects looking for what we want
		foreach (GameObject obj in objs) {
			if (obj.GetComponent<SequenceManager>()) {
				sequenceManager = obj.GetComponent<SequenceManager>();
			} else if (obj.GetComponent<SignalManagerManager>()) {
				signalManager = obj.GetComponent<SignalManagerManager>();
			}
		}

		foreach(SubSceneListener listener in listeners) {
			listener.Invoke();
		}
	}

	public void UpdateSelectedScene(int i)
	{
		activeScene = i;
		LoadSequence(activeScene);
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(SubSceneManager))]
	public class SubSceneManagerInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			var _target = (SubSceneManager)target;
			GUI.enabled = false;
			EditorGUILayout.IntField(new GUIContent("Active Scene Idx"), _target.activeScene);
			GUI.enabled = true;
			//serializedObject.Update();
			EditorGUILayout.ObjectField(new GUIContent("Sequence Manager"), _target.sequenceManager, typeof(SequenceManager), true, null);
			EditorGUILayout.ObjectField(new GUIContent("Signal Manager"), _target.signalManager, typeof(SignalManager), true, null);
			//serializedObject.ApplyModifiedProperties();
		}
	}
#endif
}
