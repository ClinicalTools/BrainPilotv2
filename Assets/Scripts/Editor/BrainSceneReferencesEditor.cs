using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BrainSceneReferences)), CanEditMultipleObjects]
public class BrainSceneReferencesEditor : Editor
{
	[System.Flags]
	public enum EditorListOption
	{
		None = 0,
		ListSize = 1,
		ListLabel = 2,
		ElementLabels = 4,
		Buttons = 8,
		Default = ListSize | ListLabel | ElementLabels | Buttons,
		NoElementLabels = ListSize | ListLabel
	}

	
	public override void OnInspectorGUI()
    { 
		var picker = target as BrainSceneReferences;

		//base.OnInspectorGUI();
		serializedObject.Update();
		//EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneReferences"), true);
		EditorListOption options = (EditorListOption) (
			0 * (int)EditorListOption.ListSize +
			1 * (int)EditorListOption.ListLabel +
			0 * (int)EditorListOption.ElementLabels +
			1 * (int)EditorListOption.Buttons);
		EditorGUILayout.LabelField("Title:");
		picker.title = EditorGUILayout.TextField(picker.title);
		EditorGUILayout.LabelField("Description:");
		picker.description = EditorGUILayout.TextField(picker.description);
		Show(serializedObject.FindProperty("sceneReferences"), options);
		serializedObject.ApplyModifiedProperties();
    }

	private static GUIContent
		moveUpButtonContent = new GUIContent("\u25b2", "move up"),
		moveDownButtonContent = new GUIContent("\u25bc", "move down"),
		//duplicateButtonContent = new GUIContent("+", "duplicate"),
		deleteButtonContent = new GUIContent("-", "delete"),
		addButtonContent = new GUIContent("+", "add element");

	public static void Show(SerializedProperty list, EditorListOption options = EditorListOption.Default)
	{
		bool
			showListLabel = (options & EditorListOption.ListLabel) != 0,
			showListSize = (options & EditorListOption.ListSize) != 0;

		if (showListLabel) {
			EditorGUILayout.PropertyField(list);
			EditorGUI.indentLevel += 1;
		}
		if (!showListLabel || list.isExpanded) {
			SerializedProperty size = list.FindPropertyRelative("Array.size");
			if (showListSize) {
				EditorGUILayout.PropertyField(size);
			}
			if (size.hasMultipleDifferentValues) {
				EditorGUILayout.HelpBox("Not showing lists with different sizes.", MessageType.Info);
			} else {
				ShowElements(list, options);
			}
		}
		if (showListLabel) {
			EditorGUI.indentLevel -= 1;
		}
	}

	private static void ShowElements(SerializedProperty list, EditorListOption options)
	{
		bool
			showElementLabels = (options & EditorListOption.ElementLabels) != 0,
			showButtons = (options & EditorListOption.Buttons) != 0;

		
		for (int i = 0; i < list.arraySize; i++) {
			if (showButtons) {
				EditorGUILayout.BeginHorizontal();
			}
			if (showElementLabels) {
				EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
			} else {
				EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
			}
			if (showButtons) {
				ShowButtons(list, i);
				EditorGUILayout.EndHorizontal();
			}
		}

		if (showButtons && /*list.arraySize == 0 &&*/ GUILayout.Button(addButtonContent, EditorStyles.miniButton, GUILayout.ExpandWidth(true))) {
			list.arraySize += 1;
		}
	}

	private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);

	private static void ShowButtons(SerializedProperty list, int index)
	{
		if (GUILayout.Button(moveDownButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth)) {
			list.MoveArrayElement(index, index + 1);
		}
		if (GUILayout.Button(moveUpButtonContent, EditorStyles.miniButtonMid, miniButtonWidth)) {
			list.MoveArrayElement(index, index - 1);
		}
		/*if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth)) {
			list.InsertArrayElementAtIndex(index);
		}*/
		if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth)) {
			int oldSize = list.arraySize;
			list.DeleteArrayElementAtIndex(index);
			if (list.arraySize == oldSize) {
				list.DeleteArrayElementAtIndex(index);
			}
		}
	}
}
