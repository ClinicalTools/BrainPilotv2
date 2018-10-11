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
    readonly string PathToBundles = "Assets/Data/Bundles/";

    SelectableState targetState;

    List<SelectableState> loadedStates;
    List<SelectableState> inverseLoadedStates;
    List<SelectableState> activeStates;
    List<SelectableState> inverseActiveStates;

    string bundleName = "";
    SelectionBundle bundleAsset;


    public override void OnInspectorGUI()
    {
        if (groupInProgress == null)
            groupInProgress = new List<Selectable>();

        base.OnInspectorGUI();
        manager = target as SelectionManager;
        manager.LoadAssetList();

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

        DisplayBundleArea();

    }

    private void DisplayBundleArea()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .9f));
        EditorGUILayout.LabelField("Bundles");
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal("box", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .9f));

        EditorGUILayout.LabelField("Bundle Name:", GUILayout.Width(100f));
        bundleName = EditorGUILayout.TextArea(bundleName);

        if (GUILayout.Button("Save Changes", GUILayout.Width(200f)))
        {
            SaveChangesToBundle();
        }
        if (GUILayout.Button("Save as New", GUILayout.Width(200f)))
        {
            SaveNewBundle();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal("box", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .9f));

        bundleAsset = (SelectionBundle) EditorGUILayout.ObjectField(bundleAsset, typeof(SelectionBundle), false);

        if (GUILayout.Button("Load Bundle", GUILayout.Width(200f)))
        {
            manager.LoadNewBundle(bundleAsset);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void SaveChangesToBundle()
    {
        if (bundleAsset == null)
        {
            SaveNewBundle();
            return;
        }

        bundleAsset.loadedStates = loadedStates;
        bundleAsset.inverseLoadedStates = inverseLoadedStates;
        bundleAsset.activeStates = activeStates;
        bundleAsset.inverseActiveStates = inverseActiveStates;

        EditorUtility.SetDirty(bundleAsset);
        AssetDatabase.SaveAssets();
        
    }

    private void SaveNewBundle()
    {
        if (targetGroup == null)
        {
            Debug.Log("Cannot Save a bundle without a group");
        }

        bundleName?.Trim();
        if (bundleName == null || bundleName == "")
        {
            bundleName = "UntitledBundle";
        }
        var bundle = ScriptableObject.CreateInstance<SelectionBundle>();
        bundle.selectionGroup = targetGroup;
        bundle.loadedStates = loadedStates;
        bundle.inverseLoadedStates = inverseLoadedStates;
        bundle.activeStates = activeStates;
        bundle.inverseActiveStates = inverseActiveStates;

        string path = AssetDatabase.GenerateUniqueAssetPath(PathToBundles + bundleName + ".asset");
        AssetDatabase.CreateAsset(bundle, path);
        AssetDatabase.SaveAssets();
    }

    private void DisplayStateHandler()
    {
        
        DisplayStateSelector();

        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .9f));

        DisplayLoadedStates();

        DisplayActiveStates();
        
        EditorGUILayout.EndHorizontal();

        DisplayApplyButtons();
    }

    private void DisplayApplyButtons()
    {

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal("box", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .9f));

        if (GUILayout.Button("Apply", GUILayout.MaxWidth(200f)))
        {
            DoGroupApplyActions();
        }
        if (GUILayout.Button("Clear", GUILayout.MaxWidth(200f)))
        {
            manager.ClearAllStates(targetGroup);
            
        }
        if (GUILayout.Button("Reset All", GUILayout.MaxWidth(200f)))
        {
            manager.ResetAllSelectables();
            manager.ResetAllGroups();
        }
        
        EditorGUILayout.EndHorizontal();

    }

    private void DoGroupApplyActions()

        // TODO: replace with a bundle creation and pass to selection manager
    {

        var bundle = ScriptableObject.CreateInstance<SelectionBundle>();
        bundle.Setup(targetGroup, loadedStates, inverseLoadedStates, activeStates, inverseActiveStates);
        manager.ApplyNewBundle(bundle);
    
    }

    private void DisplayStateSelector()
    {

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal("box", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .9f));

        targetState = (SelectableState)EditorGUILayout.ObjectField(targetState, typeof(SelectableState), false);

        if (GUILayout.Button("Load"))
        {
            if (targetState != null && !loadedStates.Contains(targetState))
                loadedStates.Add(targetState);
        }
        if (GUILayout.Button("Load Inverse"))
        {
            if (targetState != null && !inverseLoadedStates.Contains(targetState))
                inverseLoadedStates.Add(targetState);
        }

        EditorGUILayout.EndHorizontal();
    }



    /// <summary>
    /// Shows the loaded states boxes on top of one another.
    /// </summary>
    private void DisplayLoadedStates()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.Height(300f), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .45f));

        EditorGUILayout.BeginVertical("box", GUILayout.Height(145f));
        EditorGUILayout.LabelField("Loaded States", GUILayout.MinWidth(100f));

        
        if (loadedStates == null)
            loadedStates = new List<SelectableState>();
        
        for(int i = loadedStates.Count - 1; i >= 0; i--)
        {
            if (activeStates.Contains(loadedStates[i]))
            {
                continue;
            }
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(loadedStates[i].name, GUILayout.MinWidth(100f));
            if (GUILayout.Button("->", GUILayout.Width(50f)))
            {
                activeStates.Add(loadedStates[i]);
            }
            if (GUILayout.Button("\u2195", GUILayout.Width(50f)))
            {
                inverseLoadedStates.Add(loadedStates[i]);
                loadedStates.RemoveAt(i);
            }
            if (GUILayout.Button("X", GUILayout.Width(50f)))
            {
                loadedStates.RemoveAt(i);
            }


            EditorGUILayout.EndHorizontal();
        }

        // loaded states
        EditorGUILayout.EndVertical();



        // *** INVERSE LOADED STATES ***
        EditorGUILayout.BeginVertical("box", GUILayout.Height(145f));
        
        EditorGUILayout.LabelField("Inverse Loaded States", GUILayout.MinWidth(100f));

        for (int i = inverseLoadedStates.Count - 1; i >= 0; i--)
        {
            if (inverseActiveStates.Contains(inverseLoadedStates[i]))
            {
                continue;
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(inverseLoadedStates[i].name, GUILayout.MinWidth(100f));

            if (GUILayout.Button("->", GUILayout.Width(50f)))
            {
                inverseActiveStates.Add(inverseLoadedStates[i]);
            }
            if (GUILayout.Button("\u2195", GUILayout.Width(50f)))
            {
                loadedStates.Add(inverseLoadedStates[i]);
                inverseLoadedStates.RemoveAt(i);
            }
            if (GUILayout.Button("X", GUILayout.Width(50f)))
            {
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
        EditorGUILayout.BeginVertical("box", GUILayout.Height(300f), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .45f));

        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .45f), GUILayout.Height(145f));
        EditorGUILayout.LabelField("Active States", GUILayout.MinWidth(100f));

        // ****ACTIVE STATES******
        for (int i = activeStates.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(activeStates[i].name, GUILayout.MinWidth(100f));

            if (GUILayout.Button("<-", GUILayout.Width(50f)))   // just need to remove this entry from our own list, since it stays in loaded states but is not displayed
            {
                activeStates.RemoveAt(i);
            }
            if (GUILayout.Button("\u2195", GUILayout.Width(50f))) // switch to inverse, so need to switch pretty much everything! 
            {
                inverseActiveStates.Add(activeStates[i]);
                inverseLoadedStates.Add(activeStates[i]);
                loadedStates.Remove(activeStates[i]);
                activeStates.RemoveAt(i);
            }
            if (GUILayout.Button("X", GUILayout.Width(50f))) // remove entirely
            {
                loadedStates.Remove(activeStates[i]);
                activeStates.RemoveAt(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .45f), GUILayout.Height(145f));
        EditorGUILayout.LabelField("Inverse Active States", GUILayout.MinWidth(100f));

        // ******INVERSE ACTIVE STATES*************
        for (int i = inverseActiveStates.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(inverseActiveStates[i].name, GUILayout.MinWidth(100f));

            if (GUILayout.Button("<-", GUILayout.Width(50f)))
            {
                inverseActiveStates.RemoveAt(i);
            }
            if (GUILayout.Button("\u2195", GUILayout.Width(50f))) // switch everything! 
            {
                loadedStates.Add(inverseActiveStates[i]);
                activeStates.Add(inverseActiveStates[i]);
                inverseLoadedStates.Remove(inverseActiveStates[i]);
                inverseActiveStates.RemoveAt(i);
            }
            if (GUILayout.Button("X", GUILayout.Width(50f)))
            {
                inverseLoadedStates.Remove(inverseActiveStates[i]);
                inverseActiveStates.RemoveAt(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
    }

    private void DisplayGroupSelector()
    {
        EditorGUILayout.BeginHorizontal("box", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .9f));

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
        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .9f));
         
        DisplaySelectionList();
        DisplayGroupList();

        EditorGUILayout.EndHorizontal();

        DisplayGroupOptions();
    }

    private void DisplayGroupOptions()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .9f));
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
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .45f));
        EditorGUILayout.LabelField("Group:" + groupInProgressName, GUILayout.Width(100f));

        groupScrollPos = EditorGUILayout.BeginScrollView(groupScrollPos, GUILayout.Height(300f));
        for(int i = groupInProgress.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(groupInProgress[i].name, GUILayout.MinWidth(100f));
            if (GUILayout.Button("-", GUILayout.Width(50f)))
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
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .45f));
        EditorGUILayout.LabelField("Selectables", GUILayout.Width(100f));
        selectableSearch = EditorGUILayout.TextField(selectableSearch);

        selectableScrollPos = EditorGUILayout.BeginScrollView(selectableScrollPos, GUILayout.Height(300f), GUILayout.Width(EditorGUIUtility.currentViewWidth * .45f));
        foreach (var selectable in manager.allSelectables)
        {
            if (!selectable.name.ToLowerInvariant().Contains(selectableSearch.ToLowerInvariant()) || groupInProgress.Contains(selectable)) // checks for case insensitive match
                continue;

            EditorGUILayout.BeginHorizontal();
            

            EditorGUILayout.LabelField(selectable.name, GUILayout.MinWidth(100f));
            if (GUILayout.Button("+", GUILayout.Width(50f)))
            {
                groupInProgress.Add(selectable);
            }
            
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}
