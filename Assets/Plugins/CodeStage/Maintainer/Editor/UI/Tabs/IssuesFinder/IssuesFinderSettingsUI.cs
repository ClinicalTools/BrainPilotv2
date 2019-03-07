#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using System;
	using Filters;
	using Settings;
	using UnityEditor;
	using UnityEngine;

	internal static class IssuesFinderSettingsUI
	{
		public static void Draw(ref Vector2 settingsSectionScrollPosition)
		{
			// ----------------------------------------------------------------------------
			// filtering settings
			// ----------------------------------------------------------------------------

			if (UIHelpers.ImageButton("Manage Filters...", CSIcons.Gear))
			{
				IssuesFiltersWindow.Create();
			}

			GUILayout.Space(5);
			DrawWhereSection(ref settingsSectionScrollPosition);
			GUILayout.Space(5);
			DrawWhatSection(ref settingsSectionScrollPosition);

			if (UIHelpers.ImageButton("Reset", "Resets settings to defaults.", CSIcons.Restore))
			{
				MaintainerSettings.Issues.Reset();
			}
		}

		private static void DrawWhereSection(ref Vector2 settingsSectionScrollPosition)
		{
			// ----------------------------------------------------------------------------
			// where to look
			// ----------------------------------------------------------------------------

			using (new GUILayout.VerticalScope(UIHelpers.panelWithBackground))
			{
				GUILayout.Space(5);

				GUILayout.Label("<b><size=12>Where</size></b>", UIHelpers.richLabel);
				UIHelpers.Separator();
				GUILayout.Space(5);

				using (new GUILayout.HorizontalScope())
				{
					MaintainerSettings.Issues.lookInScenes = EditorGUILayout.ToggleLeft(new GUIContent("Scenes", "Uncheck to exclude all scenes from search or select filtering level:\n\n" +
																												 "All Scenes: all project scenes with respect to configured filters.\n" +
																												 "Included Scenes: scenes included via Manage Filters > Scene Includes.\n" +
																												 "Current Scene: currently opened scene including any additional loaded scenes."), MaintainerSettings.Issues.lookInScenes, GUILayout.Width(70));
					GUI.enabled = MaintainerSettings.Issues.lookInScenes;
					MaintainerSettings.Issues.scenesSelection = (IssuesFinderSettings.ScenesSelection)EditorGUILayout.EnumPopup(MaintainerSettings.Issues.scenesSelection);
					GUI.enabled = true;
				}

				MaintainerSettings.Issues.lookInAssets = EditorGUILayout.ToggleLeft(new GUIContent("File assets", "Uncheck to exclude all file assets like prefabs, ScriptableObjects and such from the search. Check readme for additional details."), MaintainerSettings.Issues.lookInAssets);
				MaintainerSettings.Issues.lookInProjectSettings = EditorGUILayout.ToggleLeft(new GUIContent("Project Settings", "Uncheck to exclude all file assets like prefabs, ScriptableObjects and such from the search. Check readme for additional details."), MaintainerSettings.Issues.lookInProjectSettings);
				UIHelpers.Separator(2);

				var canScanGamObjects = MaintainerSettings.Issues.lookInScenes || MaintainerSettings.Issues.lookInAssets;
				GUI.enabled = canScanGamObjects;
				var scanGameObjects = UIHelpers.ToggleFoldout(ref MaintainerSettings.Issues.scanGameObjects,
					ref MaintainerSettings.Issues.scanGameObjectsFoldout,
					new GUIContent("Game Objects", "Specify if you wish to look for GameObjects issues."));
				GUI.enabled = scanGameObjects && canScanGamObjects;
				if (MaintainerSettings.Issues.scanGameObjectsFoldout)
				{
					UIHelpers.Indent();
					MaintainerSettings.Issues.touchInactiveGameObjects = EditorGUILayout.ToggleLeft(new GUIContent("Inactive Game Objects", "Uncheck to exclude all inactive Game Objects from the search."), MaintainerSettings.Issues.touchInactiveGameObjects);
					MaintainerSettings.Issues.touchDisabledComponents = EditorGUILayout.ToggleLeft(new GUIContent("Disabled Components", "Uncheck to exclude all disabled Components from the search."), MaintainerSettings.Issues.touchDisabledComponents);
					UIHelpers.UnIndent();
				}

				GUI.enabled = true;

				GUILayout.Space(2);
			}
		}

		private static void DrawWhatSection(ref Vector2 settingsSectionScrollPosition)
		{
			// ----------------------------------------------------------------------------
			// what to look for
			// ----------------------------------------------------------------------------

			using (new GUILayout.VerticalScope(UIHelpers.panelWithBackground, GUILayout.ExpandHeight(true)))
			{
				GUILayout.Space(5);
				GUILayout.Label("<b><size=12>What</size></b>", UIHelpers.richLabel);
				UIHelpers.Separator();
				GUILayout.Space(5);

				settingsSectionScrollPosition = GUILayout.BeginScrollView(settingsSectionScrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);

				// ----------------------------------------------------------------------------
				// Common Issues
				// ----------------------------------------------------------------------------

				MaintainerSettings.Issues.missingReferences = EditorGUILayout.ToggleLeft(new GUIContent("Missing references", "Search for any missing references in Components, Project Settings, Scriptable Objects, and so on."), MaintainerSettings.Issues.missingReferences);
				UIHelpers.Separator(5);

				// ----------------------------------------------------------------------------
				// Game Object Issues
				// ----------------------------------------------------------------------------

				UIHelpers.Foldout(ref MaintainerSettings.Issues.gameObjectsFoldout, "<b>Game Object Issues</b>");
				if (MaintainerSettings.Issues.gameObjectsFoldout)
				{
					GUILayout.Space(-2);

					if (DrawSettingsSearchSectionHeader(SettingsSearchSection.Common, ref MaintainerSettings.Issues.commonFoldout))
					{
						MaintainerSettings.Issues.missingComponents = EditorGUILayout.ToggleLeft(new GUIContent("Missing components", "Search for the missing components on the Game Objects."), MaintainerSettings.Issues.missingComponents);
						MaintainerSettings.Issues.missingPrefabs = EditorGUILayout.ToggleLeft(new GUIContent("Missing prefabs", "Search for instances of prefabs which were removed from project."), MaintainerSettings.Issues.missingPrefabs);
						MaintainerSettings.Issues.duplicateComponents = EditorGUILayout.ToggleLeft(new GUIContent("Duplicate components", "Search for the multiple instances of the same component with same values on the same object."), MaintainerSettings.Issues.duplicateComponents);
						MaintainerSettings.Issues.inconsistentTerrainData = EditorGUILayout.ToggleLeft(new GUIContent("Inconsistent Terrain Data", "Search for Game Objects where Terrain and TerrainCollider have different Terrain Data."), MaintainerSettings.Issues.inconsistentTerrainData);
					}

					if (DrawSettingsSearchSectionHeader(SettingsSearchSection.Neatness, ref MaintainerSettings.Issues.neatnessFoldout))
					{
						MaintainerSettings.Issues.unnamedLayers = EditorGUILayout.ToggleLeft(new GUIContent("Objects with unnamed layers", "Search for GameObjects with unnamed layers."), MaintainerSettings.Issues.unnamedLayers);
						MaintainerSettings.Issues.hugePositions = EditorGUILayout.ToggleLeft(new GUIContent("Objects with huge positions", "Search for GameObjects with huge world positions (> |100 000| on any axis)."), MaintainerSettings.Issues.hugePositions);
					}

					GUILayout.Space(5);
				}
				GUI.enabled = true;

				// ----------------------------------------------------------------------------
				// Project Settings Issues
				// ----------------------------------------------------------------------------

				UIHelpers.Foldout(ref MaintainerSettings.Issues.projectSettingsFoldout, "<b>Project Settings Issues</b>");
				if (MaintainerSettings.Issues.projectSettingsFoldout)
				{
					GUILayout.Space(3);
					//UIHelpers.Indent();

					MaintainerSettings.Issues.duplicateLayers = EditorGUILayout.ToggleLeft(new GUIContent("Duplicate Layers", "Search for the duplicate layers and sorting layers at the 'Tags and Layers' Project Settings."), MaintainerSettings.Issues.duplicateLayers);

					//UIHelpers.UnIndent();
				}
				GUI.enabled = true;

				GUILayout.EndScrollView();
				UIHelpers.Separator();

				using (new GUILayout.HorizontalScope())
				{
					if (UIHelpers.ImageButton("Check all", CSIcons.SelectAll))
					{
						MaintainerSettings.Issues.SwitchAll(true);
					}

					if (UIHelpers.ImageButton("Uncheck all", CSIcons.SelectNone))
					{
						MaintainerSettings.Issues.SwitchAll(false);
					}
				}
			}
		}

		private static bool DrawSettingsSearchSectionHeader(SettingsSearchSection section, ref bool foldout)
		{
			GUILayout.Space(5);
			using (new GUILayout.HorizontalScope())
			{
				foldout = EditorGUI.Foldout(EditorGUILayout.GetControlRect(true, GUILayout.Width(165)), foldout, ObjectNames.NicifyVariableName(section.ToString()), true, UIHelpers.richFoldout);

				if (UIHelpers.IconButton(CSIcons.SelectAll))
				{
					SettingsSectionGroupSwitch(section, true);
				}

				if (UIHelpers.IconButton(CSIcons.SelectNone))
				{
					SettingsSectionGroupSwitch(section, false);
				}
			}
			UIHelpers.Separator();

			return foldout;
		}

		private static void SettingsSectionGroupSwitch(SettingsSearchSection section, bool enable)
		{
			switch (section)
			{
				case SettingsSearchSection.Common:
					MaintainerSettings.Issues.SwitchCommon(enable);
					break;
				case SettingsSearchSection.Neatness:
					MaintainerSettings.Issues.SwitchNeatness(enable);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}