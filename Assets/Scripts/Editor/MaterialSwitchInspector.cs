using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MaterialSwitchState))]
public class MaterialSwitchInspector : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MaterialSwitchState action = (MaterialSwitchState)target;

        if (GUILayout.Button("Activate"))
        {
            action.Activate();
        }
        if (GUILayout.Button("Deactivate"))
        {
            action.Deactivate();
        }
    }

}
