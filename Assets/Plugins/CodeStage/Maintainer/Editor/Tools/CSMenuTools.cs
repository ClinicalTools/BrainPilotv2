#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Tools
{
	using UnityEditor;
	using UnityEngine;

	public class CSMenuTools
	{

		public static bool ShowEditorSettings()
		{
#if UNITY_2018_3_OR_NEWER
			return CallUnifiedSettings("Editor");
#else
			return EditorApplication.ExecuteMenuItem("Edit/Project Settings/Editor");
#endif
		}

		public static bool ShowProjectSettingsGraphics()
		{
#if UNITY_2018_3_OR_NEWER
			return CallUnifiedSettings("Graphics");
#else
			return EditorApplication.ExecuteMenuItem("Edit/Project Settings/Graphics");
#endif
		}

		public static bool ShowProjectSettingsPhysics()
		{
#if UNITY_2018_3_OR_NEWER
			return CallUnifiedSettings("Physics");
#else
			return EditorApplication.ExecuteMenuItem("Edit/Project Settings/Physics");
#endif
		}

		public static bool ShowProjectSettingsPhysics2D()
		{
#if UNITY_2018_3_OR_NEWER
			return CallUnifiedSettings("Physics 2D");
#else
			return EditorApplication.ExecuteMenuItem("Edit/Project Settings/Physics 2D");
#endif
		}

		public static bool ShowProjectSettingsPresetManager()
		{
#if UNITY_2018_3_OR_NEWER
			return CallUnifiedSettings("Preset Manager");
#else
			return EditorApplication.ExecuteMenuItem("Edit/Project Settings/Preset Manager");
#endif
		}

		public static bool ShowProjectSettingsPlayer()
		{
#if UNITY_2018_3_OR_NEWER
			return CallUnifiedSettings("Player");
#else
			return EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
#endif
		}

		public static bool ShowProjectSettingsTagsAndLayers()
		{
#if UNITY_2018_3_OR_NEWER
			return CallUnifiedSettings("Tags and Layers");
#else
			return EditorApplication.ExecuteMenuItem("Edit/Project Settings/Tags and Layers");
#endif
		}

		public static bool ShowProjectSettingsVFX()
		{
#if UNITY_2018_3_OR_NEWER
			return CallUnifiedSettings("VFX");
#else
			return false;
#endif
		}

		public static bool ShowSceneSettingsLighting()
		{
#if UNITY_2018_2_OR_NEWER
			return EditorApplication.ExecuteMenuItem("Window/Rendering/Lighting Settings");
#else
			return EditorApplication.ExecuteMenuItem("Window/Lighting/Settings");
#endif
		}

		public static bool ShowSceneSettingsNavigation()
		{
#if UNITY_2018_2_OR_NEWER
			return EditorApplication.ExecuteMenuItem("Window/AI/Navigation");
#else
			return EditorApplication.ExecuteMenuItem("Window/Navigation");
#endif
		}

#if UNITY_2018_3_OR_NEWER
		private static bool CallUnifiedSettings(string providerName)
		{
			SettingsService.OpenProjectSettings("Project/" + providerName);
			return true;
		}
#endif
		public static bool ShowEditorBuildSettings()
		{
			return (EditorWindow.GetWindow(CSReflectionTools.buildPlayerWindowType, true) != null);
		}
	}
}