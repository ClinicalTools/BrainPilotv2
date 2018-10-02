using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(SelectionManager))]
public class SelectionManagerInspector : Editor {

    SelectionManager manager;

    SelectionGroup targetGroup;

    Vector2 selectableScrollPos;
    string selectableSearch = "";

    Vector2 groupScrollPos;
    string groupInProgressName;
    List<Selectable> groupInProgress;

    readonly string PathToSelectionGroups = "Assets/Data/SelectionGroups/";

    List<SelectableState> availableStates;
    string stateSearch = "";
    Vector2 stateScrollPos;

    List<SelectableState> loadedStates;
    List<SelectableState> inverseLoadedStates;
    List<SelectableState> activeStates;
    List<SelectableState> inverseActiveStates;


    public override void OnInspectorGUI()
    {
        if (groupInProgress == null)
            groupInProgress = new List<Selectable>();

        base.OnInspectorGUI();
        manager = target as SelectionManager;
        manager.LoadAssetList();

        availableStates = manager.allSelectionStates;

        if (loadedStates == null)
            loadedStates = new List<SelectableState>();
        if (inverseActiveStates == null)
            inverseActiveStates = new List<SelectableState>();
        if (activeStates == null)
            activeStates = new List<SelectableState>();
        if (inverseLoadedStates == null)
            inverseLoadedStates = new List<SelectableState>();

        DisplayGroupSelector();

        DisplayGroupEditor();

        DisplayStateHandler();

    }

    private void DisplayStateHandler()
    {
        EditorGUILayout.BeginHorizontal();
        

        DisplayStateSelector();

        DisplayLoadedStates();

        DisplayActiveStates();
        
        EditorGUILayout.EndHorizontal();
    }

    private void DisplayStateSelector()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.Height(300f), GUILayout.Width(300f));

        EditorGUILayout.LabelField("States", GUILayout.Width(Screen.width * .3f));
        stateSearch = EditorGUILayout.TextField(stateSearch);

        stateScrollPos = EditorGUILayout.BeginScrollView(stateScrollPos);
        foreach(var state in manager.allSelectionStates)
        {
            if (!state.name.ToLowerInvariant().Contains(stateSearch.ToLowerInvariant()) || loadedStates.Contains(state) || inverseLoadedStates.Contains(state))
            {
                continue;
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(state.name, GUILayout.Width(Screen.width * .18f));

            if(GUILayout.Button("Load"))
            {
                if (!loadedStates.Contains(state))
                    loadedStates.Add(state);
                
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }



    /// <summary>
    /// Shows the loaded states boxes on top of one another.
    /// </summary>
    private void DisplayLoadedStates()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.Height(300f));

        EditorGUILayout.BeginVertical("box", GUILayout.Height(145f));
        EditorGUILayout.LabelField("Loaded States", GUILayout.Width(Screen.width * .3f));

        
        if (loadedStates == null)
            loadedStates = new List<SelectableState>();
        
        for(int i = loadedStates.Count - 1; i >= 0; i--)
        {
            if (activeStates.Contains(loadedStates[i]))
            {
                continue;
            }
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(loadedStates[i].name, GUILayout.Width(Screen.width * .2f));
            if (GUILayout.Button("Unload", GUILayout.Width(100f)))
            {
                loadedStates.RemoveAt(i);
            }
            if (GUILayout.Button("\u2195"))
            {
                inverseLoadedStates.Add(loadedStates[i]);
                loadedStates.RemoveAt(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        // loaded states
        EditorGUILayout.EndVertical();



        // *** INVERSE LOADED STATES ***
        EditorGUILayout.BeginVertical("box", GUILayout.Height(145f));
        
        EditorGUILayout.LabelField("Inverse Loaded States", GUILayout.Width(Screen.width * .3f));

        for (int i = inverseLoadedStates.Count - 1; i >= 0; i--)
        {
            if (inverseActiveStates.Contains(inverseLoadedStates[i]))
            {
                continue;
            }
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(inverseLoadedStates[i].name, GUILayout.Width(Screen.width * .2f));
            if (GUILayout.Button("Unload", GUILayout.Width(100f)))
            {
                inverseLoadedStates.RemoveAt(i);
            }
            if (GUILayout.Button("\u2195"))
            {
                loadedStates.Add(inverseLoadedStates[i]);
                inverseLoadedStates.RemoveAt(i);
            }

            EditorGUILayout.EndHorizontal();
        }


        // inverse loaded states
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
    }

    private void DisplayActiveStates()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.Height(300f), GUILayout.Width(Screen.width * .3f));

        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField("Active States", GUILayout.Width(Screen.width * .3f));
        // loaded states
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField("Inverse Active States", GUILayout.Width(Screen.width * .3f));
        // inverse loaded states
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DisplayGroupSelector()
    {
        EditorGUILayout.BeginHorizontal("box");

        targetGroup = (SelectionGroup)EditorGUILayout.ObjectField(targetGroup, typeof(SelectionGroup), false);

        if (targetGroup != null)
        {
            groupInProgress = targetGroup.selectables;
            groupInProgressName = targetGroup.name;
        }

        if (GUILayout.Button("New", GUILayout.MaxWidth(50f)))
        {
            targetGroup = null;
            groupInProgress.Clear();
            groupInProgressName = "";
        }

        EditorGUILayout.EndHorizontal();
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
            loadedStates.Clear();
            activeStates.Clear();
            inverseLoadedStates.Clear();
            inverseActiveStates.Clear();
        }
        if (GUILayout.Button("Save as New"))
        {
            SaveNewGroup();
        }
        if(GUILayout.Button("Save Changes"))
        {
            SaveChangesToGroup();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void SaveChangesToGroup()
    {
        if (targetGroup == null)
        {
            SaveNewGroup();
            return;
        }

        targetGroup.selectables = groupInProgress;
        EditorUtility.SetDirty(targetGroup);
        AssetDatabase.SaveAssets();
    }

    private void SaveNewGroup()
    {
        groupInProgressName?.Trim();
        if (groupInProgressName == null || groupInProgressName == "")
        {
            groupInProgressName = "UntitledSelectionGroup";
        }
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
