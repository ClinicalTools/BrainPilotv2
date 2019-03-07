#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using Core;
	using References;
	using Settings;
	using UnityEditor;
	using UnityEditor.IMGUI.Controls;
	using UnityEngine;

	public class ReferencesTreePanel
	{
		private ReferencesTreeElement[] treeElements;
		private TreeModel<ReferencesTreeElement> treeModel;
		private ReferencesTreeView<ReferencesTreeElement> treeView;
		private SearchField searchField;

		public void Refresh(bool newData)
		{
			if (newData)
			{
				MaintainerPersonalSettings.References.referencesTreeViewState = new TreeViewState();
				treeModel = null;
			}

			if (treeModel == null)
			{
				UpdateTreeModel();
			}
		}

		public void SelectItemWithPath(string path)
		{
			treeView.SelectRowWithPath(path);
		}

		private void UpdateTreeModel()
		{
			var firstInit = MaintainerPersonalSettings.References.referencesTreeHeaderState == null || MaintainerPersonalSettings.References.referencesTreeHeaderState.columns == null || MaintainerPersonalSettings.References.referencesTreeHeaderState.columns.Length == 0;
			var headerState = ReferencesTreeView<ReferencesTreeElement>.CreateDefaultMultiColumnHeaderState();
			if (MultiColumnHeaderState.CanOverwriteSerializedFields(MaintainerPersonalSettings.References.referencesTreeHeaderState, headerState))
				MultiColumnHeaderState.OverwriteSerializedFields(MaintainerPersonalSettings.References.referencesTreeHeaderState, headerState);
			MaintainerPersonalSettings.References.referencesTreeHeaderState = headerState;

			var multiColumnHeader = new MaintainerMultiColumnHeader(headerState);

			if (firstInit)
			{
				multiColumnHeader.ResizeToFit();
				MaintainerPersonalSettings.References.referencesTreeViewState = new TreeViewState();
			}

			treeElements = LoadLastTreeElements();
			treeModel = new TreeModel<ReferencesTreeElement>(treeElements);
			treeView = new ReferencesTreeView<ReferencesTreeElement>(MaintainerPersonalSettings.References.referencesTreeViewState, multiColumnHeader, treeModel);
			treeView.SetSearchString(MaintainerPersonalSettings.References.searchString);
			treeView.Reload();

			searchField = new SearchField();
			searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;
		}

		public void Draw()
		{
			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(5);
				using (new GUILayout.VerticalScope())
				{
					EditorGUI.BeginChangeCheck();
					var searchString = searchField.OnGUI(GUILayoutUtility.GetRect(0, 0, 20, 20, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)), MaintainerPersonalSettings.References.searchString);
					if (EditorGUI.EndChangeCheck())
					{
						MaintainerPersonalSettings.References.searchString = searchString;
						treeView.SetSearchString(searchString);
						treeView.Reload();
					}
					treeView.OnGUI(GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)));
				}
				GUILayout.Space(5);
			}
		}

		public void CollapseAll()
		{
			treeView.CollapseAll();
		}

		public void ExpandAll()
		{
			treeView.ExpandAll();
		}

		private ReferencesTreeElement[] LoadLastTreeElements()
		{
			var loaded = SearchResultsStorage.ReferencesSearchResults;
			if (loaded == null || loaded.Length == 0)
			{
				loaded = new ReferencesTreeElement[1];
				loaded[0] = new ReferencesTreeElement { id = 0, depth = -1, name = "root" };
			}
			return loaded;
		}
	}
}