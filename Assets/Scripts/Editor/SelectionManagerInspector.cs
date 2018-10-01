using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(SelectionManager))]
public class SelectionManagerInspector : Editor {

    SelectionManager manager;

    Vector2 selectableScrollPos;
    string selectableSearch = "";

    Vector2 groupScrollPos;
    string groupInProgressName;
    List<Selectable> groupInProgress;

    readonly string PathToSelectionGroups = "Assets/Data/SelectionGroups/";


    public override void OnInspectorGUI()
    {
        if (groupInProgress == null)
            groupInProgress = new List<Selectable>();

        base.OnInspectorGUI();
        manager = target as SelectionManager;

        DisplayGroupEditor();



    }

    private void DisplayGroupEditor()
    {
        EditorGUILayout.BeginHorizontal();

        DisplaySelectionList();
        DisplayGroupList();

        EditorGUILayout.EndHorizontal();

        DisplayGroupOptions();
    }

    private void DisplayGroupOptions()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.Space();

        groupInProgressName = EditorGUILayout.TextField("Group Name", groupInProgressName);
        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button("Clear"))
        {
            groupInProgress.Clear();
        }
        if (GUILayout.Button("Save"))
        {
            SaveGroupInProgress();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void SaveGroupInProgress()
    {

        var group = ScriptableObject.CreateInstance<SelectionGroup>();
        group.selectables = groupInProgress;
        string path = AssetDatabase.GenerateUniqueAssetPath(PathToSelectionGroups + groupInProgressName + ".asset");
        AssetDatabase.CreateAsset(group, path);
        AssetDatabase.SaveAssets();

    }

    private void DisplayGroupList()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Group:" + groupInProgressName);

        groupScrollPos = EditorGUILayout.BeginScrollView(groupScrollPos, GUILayout.Width(Screen.width * .45f),  GUILayout.Height(300f));
        for(int i = groupInProgress.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(groupInProgress[i].name, GUILayout.Width(Screen.width * .3f));
            if (GUILayout.Button("-", GUILayout.ExpandWidth(true)))
            {
                groupInProgress.RemoveAt(i);
            }

            EditorGUILayout.EndHorizontal();
        }
        groupInProgress.TrimExcess();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        
    }

    private void DisplaySelectionList()
    {
        Rect rect = (Rect)EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Selectables");
        selectableScrollPos = EditorGUILayout.BeginScrollView(selectableScrollPos, GUILayout.Width(Screen.width * .45f), GUILayout.Height(300f));
        selectableSearch = EditorGUILayout.TextField(selectableSearch);
        foreach (var selectable in manager.allSelectables)
        {
            if (!selectable.name.ToLowerInvariant().Contains(selectableSearch.ToLowerInvariant()) || groupInProgress.Contains(selectable)) // checks for case insensitive match
                continue;

            EditorGUILayout.BeginHorizontal();
            

            EditorGUILayout.LabelField(selectable.name, GUILayout.Width(Screen.width * .3f));
            if (GUILayout.Button("+"))
            {
                groupInProgress.Add(selectable);
            }
            
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}
