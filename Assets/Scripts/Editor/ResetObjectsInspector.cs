using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ResetObjects))]
public class ResetObjectsInspector : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var resetter = (ResetObjects)target;

        if (GUILayout.Button("Reset All"))
        {
            resetter.ResetAll();
        }
        if (GUILayout.Button("Build Reset List"))
        {
            resetter.BuildResetList();
        }
        
    }


}
