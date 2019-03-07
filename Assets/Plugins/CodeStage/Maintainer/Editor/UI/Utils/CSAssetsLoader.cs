#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;

	internal class CSAssetsLoader
	{
		private static readonly Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();

		public static Texture2D GetTexture(string fileName)
		{
			return GetTexture(fileName, false, false);
		}

		public static Texture2D GetIconTexture(string fileName, bool fromEditor = false)
		{
			return GetTexture(fileName, true, fromEditor);
		}

		private static Texture2D GetTexture(string fileName, bool icon, bool fromEditor)
		{
			Texture2D result;
			var isDark = EditorGUIUtility.isProSkin;

			var path = fileName;

			if (!fromEditor)
			{
				path = Path.Combine(Maintainer.Directory, "Textures/For" + (isDark ? "Dark/" : "Bright/") + (icon ? "Icons/" : "") + fileName);
			}

			if (cachedTextures.ContainsKey(path))
			{
				result = cachedTextures[path];
			}
			else
			{
				if (!fromEditor)
				{
					result = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
				}
				else
				{
					result = EditorGUIUtility.FindTexture(path);
				}

				if (result == null)
				{
					Debug.LogError(Maintainer.LogPrefix + "Some error occurred while looking for image\n" + path);
				}
				else
				{
					cachedTextures[path] = result;
				}
			}
			return result;
		}
	}

	internal class CSIcons
	{
		public static Texture About { get { return CSAssetsLoader.GetIconTexture("About.png"); } }
		public static Texture HelpOutline { get { return CSAssetsLoader.GetIconTexture("HelpOutline.png"); } }
		public static Texture ArrowLeft { get { return CSAssetsLoader.GetIconTexture("ArrowLeft.png"); } }
		public static Texture ArrowRight { get { return CSAssetsLoader.GetIconTexture("ArrowRight.png"); } }
		public static Texture AssetStore { get { return CSAssetsLoader.GetIconTexture("UAS.png"); } }
		public static Texture AutoFix { get { return CSAssetsLoader.GetIconTexture("AutoFix.png"); } }
		public static Texture Clean { get { return CSAssetsLoader.GetIconTexture("Clean.png"); } }
		public static Texture Clear { get { return CSAssetsLoader.GetIconTexture("Clear.png"); } }
		public static Texture Collapse { get { return CSAssetsLoader.GetIconTexture("Collapse.png"); } }
		public static Texture Copy { get { return CSAssetsLoader.GetIconTexture("Copy.png"); } }
		public static Texture Delete { get { return CSAssetsLoader.GetIconTexture("Delete.png"); } }
		public static Texture DoubleArrowLeft { get { return CSAssetsLoader.GetIconTexture("DoubleArrowLeft.png"); } }
		public static Texture DoubleArrowRight { get { return CSAssetsLoader.GetIconTexture("DoubleArrowRight.png"); } }
		public static Texture Expand { get { return CSAssetsLoader.GetIconTexture("Expand.png"); } }
		public static Texture Export { get { return CSAssetsLoader.GetIconTexture("Export.png"); } }
		public static Texture Find { get { return CSAssetsLoader.GetIconTexture("Find.png"); } }
		public static Texture Gear { get { return CSAssetsLoader.GetIconTexture("Gear.png"); } }
		public static Texture Hide { get { return CSAssetsLoader.GetIconTexture("Hide.png"); } }
		public static Texture Home { get { return CSAssetsLoader.GetIconTexture("Home.png"); } }
		public static Texture Issue { get { return CSAssetsLoader.GetIconTexture("Issue.png"); } }
		public static Texture Log { get { return CSAssetsLoader.GetIconTexture("Log.png"); } }
		public static Texture Maintainer { get { return CSAssetsLoader.GetIconTexture("Maintainer.png"); } }
		public static Texture Minus { get { return CSAssetsLoader.GetIconTexture("Minus.png"); } }
		public static Texture More { get { return CSAssetsLoader.GetIconTexture("More.png"); } }
		public static Texture Plus { get { return CSAssetsLoader.GetIconTexture("Plus.png"); } }
		public static Texture Publisher { get { return CSAssetsLoader.GetIconTexture("Publisher.png"); } }
		public static Texture Restore { get { return CSAssetsLoader.GetIconTexture("Restore.png"); } }
		public static Texture Reveal { get { return CSAssetsLoader.GetIconTexture("Reveal.png"); } }
		public static Texture SelectAll { get { return CSAssetsLoader.GetIconTexture("SelectAll.png"); } }
		public static Texture SelectNone { get { return CSAssetsLoader.GetIconTexture("SelectNone.png"); } }
		public static Texture Show { get { return CSAssetsLoader.GetIconTexture("Show.png"); } }
		public static Texture Support { get { return CSAssetsLoader.GetIconTexture("Support.png"); } }
		public static Texture Repeat { get { return CSAssetsLoader.GetIconTexture("Repeat.png"); } }
	}

	internal class CSEditorIcons
	{
		public static Texture ErrorSmallIcon { get { return CSAssetsLoader.GetIconTexture("console.erroricon.sml", true); } }
		public static Texture ErrorIcon { get { return CSAssetsLoader.GetIconTexture("console.erroricon", true); } }
		public static Texture FolderIcon { get { return CSAssetsLoader.GetIconTexture("Folder Icon", true); } }
		public static Texture InfoSmallIcon { get { return CSAssetsLoader.GetIconTexture("console.infoicon.sml", true); } }
		public static Texture InfoIcon { get { return CSAssetsLoader.GetIconTexture("console.infoicon", true); } }
#if UNITY_2018_1_OR_NEWER
		public static Texture PrefabIcon { get { return UnityEditorInternal.InternalEditorUtility.FindIconForFile("dummy.prefab"); } }
		public static Texture SceneIcon { get { return UnityEditorInternal.InternalEditorUtility.FindIconForFile("dummy.unity"); } }
		public static Texture ScriptIcon { get { return UnityEditorInternal.InternalEditorUtility.FindIconForFile("dummy.cs"); } }
#else
		public static Texture PrefabIcon { get { return UnityEditorInternal.InternalEditorUtility.GetIconForFile("dummy.prefab"); } }
		public static Texture SceneIcon { get { return UnityEditorInternal.InternalEditorUtility.GetIconForFile("dummy.unity"); } }
		public static Texture ScriptIcon { get { return UnityEditorInternal.InternalEditorUtility.GetIconForFile("dummy.cs"); } }
#endif

		public static Texture WarnSmallIcon { get { return CSAssetsLoader.GetIconTexture("console.warnicon.sml", true); } }
		public static Texture WarnIcon { get { return CSAssetsLoader.GetIconTexture("console.warnicon", true); } }
		public static Texture FilterByType { get { return CSAssetsLoader.GetIconTexture("FilterByType", true); } }
	}

	internal class CSImages
	{
		public static Texture Logo { get { return CSAssetsLoader.GetTexture("Logo.png"); } }
	}

	internal class CSColors
	{
		public static Color labelDimmedColor = ChangeColorAlpha(GUI.skin.label.normal.textColor , 150);
		public static Color labelGreenColor = LerpColorToGreen(GUI.skin.label.normal.textColor, 0.3f);
		public static Color labelRedColor = LerpColorToRed(GUI.skin.label.normal.textColor, 0.3f);

		public static Color backgroundGreenTint = EditorGUIUtility.isProSkin ? new Color32(0, 255, 0, 150) : new Color32(0, 255, 0, 30);
		public static Color backgroundRedTint = EditorGUIUtility.isProSkin ? new Color32(255, 0, 0, 150) : new Color32(255, 0, 0, 30);

		private static Color32 LerpColorToRed(Color32 inValue, float greenAmountPercent)
		{
			return Color.Lerp(inValue, new Color32(255, 0, 0, 255), greenAmountPercent);
		}

		private static Color32 LerpColorToGreen(Color32 inValue, float greenAmountPercent)
		{
			return Color.Lerp(inValue, new Color32(0, 255, 0, 255), greenAmountPercent);
		}

		private static Color32 ChangeColorAlpha(Color32 inValue, byte alphaValue)
		{
			inValue.a = alphaValue;
			return inValue;
		}
	}
}