using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Selectable Element indexes common dependencies. 
/// </summary>
[RequireComponent(typeof(SelectableStateController))]
[RequireComponent(typeof(SelectableListener))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class SelectableElement : MonoBehaviour 
{

    public Selectable selectable;

    public SelectableStateController stateController;
    public SelectableListener listener;
    public MeshRenderer meshRenderer;

    protected virtual void Awake()
    {
        stateController = GetComponent<SelectableStateController>();
        listener = GetComponent<SelectableListener>();
        meshRenderer = GetComponent<MeshRenderer>();

        listener.selectable = selectable;
    }

}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(SelectableElement))]
public class SelectableElementEditor : UnityEditor.Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if (GUILayout.Button("Remove components")) {
			var _target = (SelectableElement)target;
			UnityEditor.Undo.RecordObject(_target.gameObject, "Remove selectable elements");
			DestroyImmediate(_target.GetComponent<MeshCollider>());
			DestroyImmediate(_target.GetComponent<MaterialSwitchState>());
			DestroyImmediate(_target.GetComponent<BrainElementSelectionBuilder>());
			var comp1 = _target.GetComponent<SelectableStateController>();
			var comp2 = _target.GetComponent<SelectableListener>();
			var comp3 = _target.GetComponent<StateActionManager>();
			DestroyImmediate(_target.GetComponent<SelectableElement>());
			DestroyImmediate(comp1);
			DestroyImmediate(comp2);
			DestroyImmediate(comp3);
		}
	}
}
#endif
