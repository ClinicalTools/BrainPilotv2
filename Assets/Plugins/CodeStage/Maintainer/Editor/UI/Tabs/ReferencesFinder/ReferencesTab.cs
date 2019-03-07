#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using References;

	using UnityEngine;

	using System;
	
	using Core;
	using Settings;
	using Filters;

	using UnityEditor.IMGUI.Controls;
	using UnityEditor;

	internal class ReferencesTab
	{
		public static string AutoSelectPath { get; set; }
		public static bool AutoShowExistsNotification { get; set; }

		private readonly MaintainerWindow window;
		private readonly ReferencesTreePanel treePanel;

		private GUIContent caption;

		public ReferencesTab(MaintainerWindow window)
		{
			this.window = window;
			treePanel = new ReferencesTreePanel();
		}

		public GUIContent Caption
		{
			get
			{
				if (caption == null)
				{
					caption = new GUIContent(ReferencesFinder.ModuleName, CSIcons.Find);
				}
				return caption;
			}
		}

		public void Refresh(bool newData)
		{
			treePanel.Refresh(newData);

			if (!string.IsNullOrEmpty(AutoSelectPath))
			{
				EditorApplication.delayCall += () =>
				{
					treePanel.SelectItemWithPath(AutoSelectPath);
					AutoSelectPath = null;
				};
			}

			if (AutoShowExistsNotification)
			{
				window.ShowNotification(new GUIContent("Such item(s) already present in the list!"));
				AutoShowExistsNotification = false;
			}
		}

		public virtual void Draw()
		{
			using (new GUILayout.VerticalScope(UIHelpers.panelWithBackground, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true)))
			{
				GUILayout.Space(5);

				DrawHeader();

				GUILayout.Space(5);

				treePanel.Draw();

				GUILayout.Space(5);

				DrawFooter();
			}
		}

		private void DrawHeader()
		{
			using (new GUILayout.HorizontalScope())
			{
				using (new GUILayout.VerticalScope())
				{
					GUILayout.Label("<size=13>Here you may check any project assets for all project references.</size>", UIHelpers.richWordWrapLabel);

					if (UIHelpers.ImageButton("Find all assets references",
						"Traverses whole project to find where all assets are referenced.", CSIcons.Find))
					{
						if (Event.current.control && Event.current.shift)
						{
							ReferencesFinder.debugMode = true;
							AssetsMap.Delete();
							Event.current.Use();
						}
						else
						{
							ReferencesFinder.debugMode = false;
						}
						EditorApplication.delayCall += StartProjectReferencesScan;
					}

					if (ReferencesFinder.GetSelectedAssets().Length == 0)
					{
						GUI.enabled = false;
					}
					if (UIHelpers.ImageButton("Find selected assets references",
						"Adds selected Project View assets to the current search results.", CSIcons.Find))
					{
						EditorApplication.delayCall += () => ReferencesFinder.AddSelectedToSelectionAndRun();
					}
					GUI.enabled = true;
				}

				GUILayout.Space(30);

				using (new GUILayout.VerticalScope(UIHelpers.panelWithBackground, GUILayout.Width(250)))
				{
					GUILayout.Space(5);
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Space(3);
						if (UIHelpers.ImageButton("Manage Filters... (" + MaintainerSettings.References.pathIgnoresFilters.Length + ")",
							CSIcons.Gear, GUILayout.ExpandWidth(false)))
						{
							ReferencesFiltersWindow.Create();
						}
						GUILayout.FlexibleSpace();
						if (UIHelpers.ImageButton(null, "Show some extra info and notes about " + ReferencesFinder.ModuleName + ".", CSIcons.HelpOutline, GUILayout.ExpandWidth(false)))
						{
							EditorUtility.DisplayDialog(ReferencesFinder.ModuleName + " Extra Info",
								"Except buttons on this tab, you may use these commands to search for references:\n\n" +
								"Search for asset references from the Project Browser context menu:\n" +
								MaintainerMenu.ProjectBrowserContextReferencesFinderName + "\n\n" +
								"Look for the MonoBehaviour and ScriptableObject references from the Components' or ScriptableObjects' context menus\n" +
								MaintainerMenu.ScriptReferencesContextMenuName + "\n\n" +
								"Or just drag && drop items from Project Browser or Inspector to the list." + "\n\n" +
								"Note #1: you'll see only those references which Maintainer was able to figure out. " +
								"Some kinds of references can't be statically found or not supported yet." + "\n\n" +
								"Note #2: not referenced assets still may be used at runtime or from Editor scripting.", "OK");
						}

						GUILayout.Space(3);
					}

					MaintainerPersonalSettings.References.showAssetsWithoutReferences = GUILayout.Toggle(
						MaintainerPersonalSettings.References.showAssetsWithoutReferences,
						new GUIContent("Add assets without found references", "Check to see all scanned assets in the list even if there was no any references to the asset found in project."), GUILayout.ExpandWidth(false));

					MaintainerPersonalSettings.References.selectedFindClearsResults = GUILayout.Toggle(
						MaintainerPersonalSettings.References.selectedFindClearsResults,
						new GUIContent(@"Clear results on selected assets search", "Check to automatically clear last results on selected assets find both from context menu and main window.\nUncheck to add new results to the last results."), GUILayout.ExpandWidth(false));

					GUILayout.Space(3);
				}
			}
		}

		private void DrawFooter()
		{
			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(5);

				if (SearchResultsStorage.ReferencesSearchSelection.Length == 0)
				{
					GUI.enabled = false;
				}
				if (UIHelpers.ImageButton("Refresh", "Restarts references search for the previous results.", CSIcons.Repeat))
				{
					if (Event.current.control && Event.current.shift)
					{
						ReferencesFinder.debugMode = true;
						AssetsMap.Delete();
						Event.current.Use();
					}
					else
					{
						ReferencesFinder.debugMode = false;
					}

					EditorApplication.delayCall += () =>
					{
						ReferencesFinder.GetReferences(SearchResultsStorage.ReferencesSearchSelection);
					};
				}
				GUI.enabled = true;

				if (UIHelpers.ImageButton("Collapse all", "Collapses all tree items.", CSIcons.Collapse))
				{
					treePanel.CollapseAll();
				}

				if (UIHelpers.ImageButton("Expand all", "Expands all tree items.", CSIcons.Expand))
				{
					treePanel.ExpandAll();
				}

				if (UIHelpers.ImageButton("Clear results", "Clears results tree and empties cache.", CSIcons.Clear))
				{
					SearchResultsStorage.ReferencesSearchResults = null;
					SearchResultsStorage.ReferencesSearchSelection = null;
					Refresh(true);
				}
				GUILayout.Space(5);
			}
		}

		private void StartProjectReferencesScan()
		{
			window.RemoveNotification();
			ReferencesFinder.GetReferences();
			window.Focus();
		}
	}
}
