using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(UIElementMenu))]
public class UIElementMenuInspector : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var menu = (UIElementMenu)target;

        if (GUILayout.Button("Set Visible On"))
        {
            menu.ToggleVisible(true);
        }
        if (GUILayout.Button("Set Visible Off"))
        {
            menu.ToggleVisible(false);
        }
        
    }


}
