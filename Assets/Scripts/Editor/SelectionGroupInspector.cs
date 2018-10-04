using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SelectionGroup))]
public class SelectionGroupInspector : Editor {

    SelectableState state;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SelectionGroup group = (SelectionGroup)target;

        state = (SelectableState)EditorGUILayout.ObjectField(state, typeof(SelectableState), false);

        if (GUILayout.Button("Load"))
        {
            group.LoadState(state);
        }
        if (GUILayout.Button("Unload"))
        {
            group.UnloadState(state);
        }
        if (GUILayout.Button("Unload All"))
        {
            group.UnloadAll();
        }
        if (GUILayout.Button("Activate"))
        {
            group.ActivateState(state);
        }
    }

}
