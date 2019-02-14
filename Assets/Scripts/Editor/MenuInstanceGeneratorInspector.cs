using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(MenuInstanceGenerator))]
public class MenuInstanceGeneratorInspector : Editor
{

    string menuName = "";
    List<string> menuItemNames;

    UIElementMenu uiElementMenuAssetTarget;

    string PathToMenuAssets = "Assets/Data/UI/Menus";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var generator = (MenuInstanceGenerator)target;

        if (generator.parentSelectableElement == null)
            generator.parentSelectableElement = generator.FindParentSelectable();

        menuItemNames = menuItemNames ?? new List<string>();

        if (uiElementMenuAssetTarget != generator.uiElementMenu)   // if we have an existing UIElementMenu asset, load that first
        {
            LoadInfo(generator.uiElementMenu);
            uiElementMenuAssetTarget = generator.uiElementMenu;
        }

        DisplayHeader();

        DisplayDataPath();

        DisplayMenuTitle();

        DisplayMenuOptions();

        DisplayActionButtons(generator);


    }

    private void LoadInfo(UIElementMenu uiElementMenu)
    {
        menuName = uiElementMenu.name;
        menuItemNames = new List<string>(uiElementMenu.elements?.Select(element => element.name));
    }

    private void DisplayHeader()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.LabelField("Menu Generator", EditorStyles.boldLabel);
    }

    private void DisplayActionButtons(MenuInstanceGenerator generator)
    {
        EditorGUILayout.BeginHorizontal("box", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .9f));

        if (GUILayout.Button("Remove Game Objects"))
        {
            generator.Clear();
        }

        if (GUILayout.Button("Generate Menu"))
        {
            generator.Generate();
        }


        EditorGUILayout.EndHorizontal();
    }

    private void DisplayMenuOptions()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Menu Options");

        if (GUILayout.Button("Add"))
        {
            AddNewMenuOption();
        }
        if (GUILayout.Button("Clear"))
        {
            ClearAllMenuOptions();
        }

        EditorGUILayout.EndHorizontal();

        DisplayOptionsList();
    }

    private void ClearAllMenuOptions()
    {
        menuItemNames = new List<string>();
    }

    private void AddNewMenuOption()
    {
        menuItemNames.Add("");
    }

    private void DisplayOptionsList()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.9f), GUILayout.Height(150f));

        EditorGUILayout.Space();

        //for(int i = 0; i < menuItemNames.Count; i++)
        //{

        //    EditorGUILayout.BeginHorizontal();

        //    menuItemNames[i] = EditorGUILayout.TextArea(menuItemNames[i], GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .7f));
        //    if (GUILayout.Button("X", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .1f))) 
        //    {
        //        menuItemNames.RemoveAt(i);
        //    }

        //    EditorGUILayout.EndHorizontal();
        //}

        for (int i = 0; i < uiElementMenuAssetTarget.elements.Count; i++)
        {

            EditorGUILayout.BeginHorizontal();

            uiElementMenuAssetTarget.elements[i].name = EditorGUILayout.TextArea(uiElementMenuAssetTarget.elements[i].name, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .7f));
            if (GUILayout.Button("X", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * .1f)))
            {
                uiElementMenuAssetTarget.elements.RemoveAt(i);
            }
            

            EditorGUILayout.EndHorizontal();
        }


        EditorGUILayout.EndVertical();
    }

    private void DisplayMenuTitle()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Menu Title:", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(menuName, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.9f));

        EditorGUILayout.EndHorizontal();
    }

    private void DisplayDataPath()
    {
        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.LabelField("Path to data:");
        EditorGUILayout.TextArea(PathToMenuAssets);

        EditorGUILayout.EndHorizontal();
    }
}
