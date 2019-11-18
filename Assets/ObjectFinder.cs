using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectFinder : MonoBehaviour
{
	public GameObject target;

	[HideInInspector]
	public List<Component> components;

	[HideInInspector]
	public int selected;

	public bool debug;
	
}
#if UNITY_EDITOR
[CustomEditor(typeof(ObjectFinder))]
public class ObjectFinderEditor : Editor
{
	ObjectFinder _target;

	GameObject oldTarget;

	string[] options;

	public override void OnInspectorGUI()
	{
		_target = (ObjectFinder)target;
		
		base.OnInspectorGUI();

		if (_target.target == null) {
			options = new string[] { "--" };
		} else {
			options = GetTargetComponentNames();
		}

		_target.selected = EditorGUILayout.Popup("Component", _target.selected, options);

		if (GUILayout.Button("Find GameObject references")) {
			FindObjectsReferencingTarget(false, true);
		}

		if (GUILayout.Button("Find selected component references")) {
			FindObjectsReferencingTarget(true, false);
		}

		if (GUILayout.Button("Find component and GameObject references")) {
			FindObjectsReferencingTarget(true, true);
		}

		if (GUILayout.Button("Find all references")) {
			FindObjectsReferencingTarget(true, true, true);
		}
	}

	string[] GetTargetComponentNames()
	{
		_target.components = new List<Component>(_target.target.GetComponents<Component>());
		string[] names = new string[_target.components.Count];
		for(int i = 0; i < names.Length; i++) {
			names[i] = _target.components[i].GetType().ToString();
		}
		return names;
	}

	public void FindObjectsReferencingTarget(bool findComponent, bool findGameObject, bool findAnyComponent = false)
	{
		GameObject[] allObjects = GetAllGameObjects();
		
		List<SerializedObject> matches = new List<SerializedObject>();

		SerializedObject sObj;
		SerializedProperty sProp;
		int runs = 0;
		foreach (GameObject obj in allObjects) {
			runs++;
			//if (runs > 100) return;
			Component[] objComponents = obj.GetComponents<Component>();
			foreach (Component c in objComponents) {
				if (c == null) continue;
				sObj = new UnityEditor.SerializedObject(c);
				sProp = sObj.GetIterator();
				while (sProp != null) {
					if (sProp.propertyType != SerializedPropertyType.ObjectReference) {
						if (!sProp.Next(true)) break;
						continue;
					}

					if (findComponent) {
						if (findAnyComponent) {
							if (_target.components.Find(d => d.GetInstanceID().Equals(sProp.objectReferenceInstanceIDValue)) != null) {
								if (!sProp.propertyPath.Equals("m_Father") && !sProp.propertyPath.StartsWith("m_Children")) {
									matches.Add(sObj);
									if (_target.debug) {
										Debug.Log(sProp.propertyPath);
										Debug.Log(sProp.serializedObject.targetObject.name);
										Debug.Log(sProp.displayName);
									}
									break;
								}
							}
						} else if (sProp.objectReferenceInstanceIDValue == _target.components[_target.selected].GetInstanceID()) {
							if (!sProp.propertyPath.Equals("m_Father") && !sProp.propertyPath.StartsWith("m_Children")) {
								matches.Add(sObj);
								if (_target.debug) {
									Debug.Log(sProp.propertyPath);
									Debug.Log(sProp.serializedObject.targetObject.name);
									Debug.Log(sProp.displayName);
								}
								break;
							}
						}
					} 
					if (findGameObject) {
						if (sProp.objectReferenceInstanceIDValue == _target.target.GetInstanceID()) {
							if (!sProp.propertyPath.Equals("m_GameObject")) {
								matches.Add(sObj);
								if (_target.debug) {
									Debug.Log(sProp.propertyPath);
									Debug.Log(sProp.serializedObject.targetObject.name);
									Debug.Log(sProp.displayName);
								}
								break;
							}
						}
					}
					if (!sProp.Next(true)) break;
				}
			}
		}

		foreach(SerializedObject so in matches) {
			//Debug.Log(so.targetObject.name, so.targetObject);
			if ((so.targetObject as Component) != _target) {
				Debug.Log(so.targetObject.name + ": " + (so.targetObject as Component).GetType(),
						so.targetObject);
			}
		}
	}

	private GameObject[] GetAllGameObjects()
	{
		List<GameObject> results = new List<GameObject>(SceneManager.GetActiveScene().GetRootGameObjects());
		int len = results.Count;
		for (int i = 0; i < len; i++) {
			new List<Transform>(results[i].GetComponentsInChildren<Transform>(true)).ForEach(t => results.Add(t.gameObject));
		}
		return results.ToArray();
	}
}
#endif