#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using Cleaner;
	using Issues;
	using Tools;
	using References;
	using UnityEditor;

	public class MaintainerMenu
	{
		public const string MenuSection = "⚙ Maintainer";

		public const string ReferencesFinderMenuName = "🔍 Find References in Project";
		public const string ScriptReferencesContextMenuName = "🔍 Maintainer: Find All Script References";
		public const string ScriptReferencesContextMenu = "CONTEXT/MonoBehaviour/" + ScriptReferencesContextMenuName; 
		public const string ProjectBrowserContextStart = "Assets/";
		public const string ProjectBrowserContextReferencesFinderName = MenuSection + "/" + ReferencesFinderMenuName;
		public const string ProjectBrowserContextReferencesFinderNoHotKey = ProjectBrowserContextStart + ProjectBrowserContextReferencesFinderName;
		public const string ProjectBrowserContextReferencesFinder = ProjectBrowserContextReferencesFinderNoHotKey + " %#&s";
		public const string MainMenu = "Tools/Code Stage/" + MenuSection + "/";

		[MenuItem(MainMenu + "Show %#&`", false, 900)]
		private static void ShowWindow()
		{
			MaintainerWindow.Create();
		}

		[MenuItem(MainMenu + "About", false, 901)]
		private static void ShowAbout()
		{
			MaintainerWindow.ShowAbout();
		}

		[MenuItem(MainMenu + "Find Issues %#&f", false, 1000)]
		private static void FindAllIssues()
		{
			IssuesFinder.StartSearch(true);
		}

		[MenuItem(MainMenu + "Find Garbage %#&g", false, 1001)]
		private static void FindAllGarbage()
		{
			ProjectCleaner.StartSearch(true);
		}

		[MenuItem(MainMenu + "Find All References %#&r", false, 1002)]
		private static void FindAllReferences()
		{
			ReferencesFinder.GetReferences();
		}

		[MenuItem(ProjectBrowserContextReferencesFinder, true, 39)]
		public static bool ValidateFindReferences()
		{
			return ReferencesFinder.GetSelectedAssets().Length > 0;
		}

		[MenuItem(ProjectBrowserContextReferencesFinder, false, 39)]
		public static void FindReferences()
		{
			ReferencesFinder.AddSelectedToSelectionAndRun();
		}

		[MenuItem(ScriptReferencesContextMenu, true, 144444)]
		public static bool ValidateFindComponentReferences(MenuCommand command)
		{
			var scriptPath = CSObjectTools.GetScriptPathFromObject(command.context);
			return !string.IsNullOrEmpty(scriptPath);
		}

		[MenuItem(ScriptReferencesContextMenu, false, 144444)]
		public static void FindComponentReferences(MenuCommand command)
		{
			var scriptPath = CSObjectTools.GetScriptPathFromObject(command.context);
			ReferencesFinder.AddToSelectionAndRun(scriptPath);
		}
	}
}