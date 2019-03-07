#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using System;
	using System.IO;
	using System.Linq;
	using Cleaner;
	using Core;
	using Issues;
	using Settings;
	using Tools;
	using Filters;

	using UnityEditor;
	using UnityEngine;

	internal class IssuesTab : RecordsTab<IssueRecord>
	{
		private GUIContent caption;

		public IssuesTab(MaintainerWindow maintainerWindow) : base(maintainerWindow)
		{
		}

		internal GUIContent Caption
		{
			get
			{
				if (caption == null)
				{
					caption = new GUIContent(IssuesFinder.ModuleName, CSIcons.Issue);
				}
				return caption;
			}
		}

		protected override IssueRecord[] LoadLastRecords()
		{
			var loadedRecords = SearchResultsStorage.IssuesSearchResults;

			if (loadedRecords == null)
			{
				loadedRecords = new IssueRecord[0];
			}

			return loadedRecords;
		}

		protected override RecordsTabState GetState()
		{
			return MaintainerPersonalSettings.Issues.tabState;
		}

		protected override void ApplySorting()
		{
			base.ApplySorting();

			switch (MaintainerPersonalSettings.Issues.sortingType)
			{
				case IssuesSortingType.Unsorted:
					break;
				case IssuesSortingType.ByIssueType:
					filteredRecords = MaintainerPersonalSettings.Issues.sortingDirection == SortingDirection.Ascending ?
						filteredRecords.OrderBy(RecordsSortings.issueRecordByType).ThenBy(RecordsSortings.issueRecordByPath).ToArray() :
						filteredRecords.OrderByDescending(RecordsSortings.issueRecordByType).ThenBy(RecordsSortings.issueRecordByPath).ToArray();
					break;
				case IssuesSortingType.BySeverity:
					filteredRecords = MaintainerPersonalSettings.Issues.sortingDirection == SortingDirection.Ascending ?
						filteredRecords.OrderByDescending(RecordsSortings.issueRecordBySeverity).ThenBy(RecordsSortings.issueRecordByPath).ToArray() :
						filteredRecords.OrderBy(RecordsSortings.issueRecordBySeverity).ThenBy(RecordsSortings.issueRecordByPath).ToArray();
					break;
				case IssuesSortingType.ByPath:
					filteredRecords = MaintainerPersonalSettings.Issues.sortingDirection == SortingDirection.Ascending ?
						filteredRecords.OrderBy(RecordsSortings.issueRecordByPath).ToArray() :
						filteredRecords.OrderByDescending(RecordsSortings.issueRecordByPath).ToArray();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override void SaveSearchResults()
		{
			SearchResultsStorage.IssuesSearchResults = GetRecords();
		}

		protected override string GetModuleName()
		{
			return IssuesFinder.ModuleName;
		}

		protected override void DrawSettingsBody()
		{
			IssuesFinderSettingsUI.Draw(ref settingsSectionScrollPosition);
		}

		protected override void DrawRightPanelTop()
		{
			if (UIHelpers.ImageButton("1. Find issues!", CSIcons.Find))
			{
				EditorApplication.delayCall += StartSearch;
			}

			if (UIHelpers.ImageButton("2. Automatically fix selected issues if possible", CSIcons.AutoFix))
			{
				EditorApplication.delayCall += StartFix;
			}
		}

		protected override void DrawPagesRightHeader()
		{
			base.DrawPagesRightHeader();

			GUILayout.Label("Sorting:", GUILayout.ExpandWidth(false));

			EditorGUI.BeginChangeCheck();
			MaintainerPersonalSettings.Issues.sortingType = (IssuesSortingType)EditorGUILayout.EnumPopup(MaintainerPersonalSettings.Issues.sortingType, GUILayout.Width(100));
			if (EditorGUI.EndChangeCheck())
			{
				ApplySorting();
			}

			EditorGUI.BeginChangeCheck();
			MaintainerPersonalSettings.Issues.sortingDirection = (SortingDirection)EditorGUILayout.EnumPopup(MaintainerPersonalSettings.Issues.sortingDirection, GUILayout.Width(80));
			if (EditorGUI.EndChangeCheck())
			{
				ApplySorting();
			}
		}

		protected override void DrawRecord(IssueRecord record, int recordIndex)
		{
			// hide fixed records 
			if (record.@fixed) return;

			using (new GUILayout.VerticalScope())
			{
				if (recordIndex > 0 && recordIndex < filteredRecords.Length) UIHelpers.Separator();

				using (new GUILayout.HorizontalScope())
				{
					DrawRecordCheckbox(record);
					DrawExpandCollapseButton(record);
					DrawSeverityIcon(record);
					
					if (record.compactMode)
					{
						DrawRecordButtons(record, recordIndex);
						GUILayout.Label(record.GetCompactLine(), UIHelpers.richLabel);
					}
					else
					{
						GUILayout.Space(5);
						GUILayout.Label(record.GetHeader(), UIHelpers.richLabel);
					}

					if (record.Location == RecordLocation.Prefab)
					{
						GUILayout.Space(3);
						UIHelpers.Icon(CSEditorIcons.PrefabIcon, "Issue found in the Prefab.");
					}
				}

				if (!record.compactMode)
				{
					UIHelpers.Separator();
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Space(5);
						GUILayout.Label(record.GetBody(), UIHelpers.richLabel);
					}
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Space(5);
						DrawRecordButtons(record, recordIndex);
					}
					GUILayout.Space(3);
				}
			}
		}

		protected override string GetReportFileNamePart()
		{
			return "Issues";
		}

		protected override void AfterClearRecords()
		{
			SearchResultsStorage.IssuesSearchResults = null;
		}

		private void StartSearch()
		{
			window.RemoveNotification();
			IssuesFinder.StartSearch(true);
			window.Focus();
		}

		private void StartFix()
		{
			window.RemoveNotification();
			IssuesFinder.StartFix();
			window.Focus();
		}

		private void DrawRecordButtons(IssueRecord record, int recordIndex)
		{
			DrawShowButtonIfPossible(record);
			DrawFixButton(record, recordIndex);

			if (!record.compactMode)
			{
				DrawCopyButton(record);
				DrawHideButton(record, recordIndex);
			}

			var objectIssue = record as GameObjectIssueRecord;
			if (objectIssue != null)
			{
				DrawMoreButton(objectIssue);
			}
		}

		private void DrawFixButton(IssueRecord record, int recordIndex)
		{
			GUI.enabled = record.CanBeFixed();

			var label = "Fix";
			var hint = "Automatically fixes issue (not available for this issue yet).";

			if (record.Kind == IssueKind.MissingComponent)
			{
				label = "Remove";
				hint = "Removes missing component.";
			}
			else if (record.Kind == IssueKind.MissingReference)
			{
				label = "Reset";
				hint = "Resets missing reference to default None value.";
			}

			if (UIHelpers.RecordButton(record, label, hint, CSIcons.AutoFix))
			{
				if (record.Fix(false))
				{
					DeleteRecords(new[] { recordIndex });

					var notificationExtra = "";

					if (record.Location == RecordLocation.Prefab || record.Location == RecordLocation.Asset)
					{
						AssetDatabase.SaveAssets();
					}

					MaintainerWindow.ShowNotification("Issue successfully fixed!" + notificationExtra);
				}
				else
				{
					MaintainerWindow.ShowNotification("Could not fix the issue!");
				}
			}

			GUI.enabled = true;
		}

		private void DrawHideButton(IssueRecord record, int recordIndex)
		{
			if (UIHelpers.RecordButton(record, "Hide", "Hides this issue from the results list.\nUseful when you fixed issue and wish to hide it away.", CSIcons.Hide))
			{
				DeleteRecords(new []{ recordIndex });
			}
		}

		private void DrawMoreButton(GameObjectIssueRecord record)
		{
			if (!UIHelpers.RecordButton(record, "Shows menu with additional actions for this record.", CSIcons.More)) return;

			var menu = new GenericMenu();
			if (!string.IsNullOrEmpty(record.Path))
			{
				menu.AddItem(new GUIContent("Ignore/Full Path"), false, () =>
				{
					if (!CSFilterTools.IsValueMatchesAnyFilter(record.Path, MaintainerSettings.Issues.pathIgnoresFilters))
					{
						var newFilter = FilterItem.Create(record.Path, FilterKind.Path);
						ArrayUtility.Add(ref MaintainerSettings.Issues.pathIgnoresFilters, newFilter);

						ApplyNewIgnoreFilter(newFilter);

						MaintainerWindow.ShowNotification("Ignore added: " + record.Path);
						CleanerFiltersWindow.Refresh();
					}
					else
					{
						MaintainerWindow.ShowNotification("Already added to the ignores!");
					}
				});

				var dir = Directory.GetParent(record.Path);
				if (dir.Name != "Assets")
				{
					menu.AddItem(new GUIContent("Ignore/Parent Folder"), false, () =>
					{
						var dirPath = CSPathTools.EnforceSlashes(dir.ToString());

						if (!CSFilterTools.IsValueMatchesAnyFilter(dirPath, MaintainerSettings.Issues.pathIgnoresFilters))
						{
							var newFilter = FilterItem.Create(dirPath, FilterKind.Directory);
							ArrayUtility.Add(ref MaintainerSettings.Issues.pathIgnoresFilters, newFilter);

							ApplyNewIgnoreFilter(newFilter);

							MaintainerWindow.ShowNotification("Ignore added: " + dirPath);
							CleanerFiltersWindow.Refresh();
						}
						else
						{
							MaintainerWindow.ShowNotification("Already added to the ignores!");
						}
					});
				}
			}

			if (!string.IsNullOrEmpty(record.componentName))
			{
				menu.AddItem(new GUIContent("Ignore/\"" + record.componentName + "\" Component" ), false, () =>
				{
					if (!CSFilterTools.IsValueMatchesAnyFilter(record.componentName, MaintainerSettings.Issues.componentIgnoresFilters))
					{
						var newFilter = FilterItem.Create(record.componentName, FilterKind.Type);
						ArrayUtility.Add(ref MaintainerSettings.Issues.componentIgnoresFilters, newFilter);

						ApplyNewIgnoreFilter(newFilter);

						MaintainerWindow.ShowNotification("Ignore added: " + record.componentName);
						CleanerFiltersWindow.Refresh();
					}
					else
					{
						MaintainerWindow.ShowNotification("Already added to the ignores!");
					}
				});
			}
			menu.ShowAsContext();
		}

		private void DrawSeverityIcon(IssueRecord record)
		{
			Texture icon;

			if (record == null) return;

			switch (record.Severity)
			{
				case RecordSeverity.Error:
					icon = CSEditorIcons.ErrorSmallIcon;
					break;
				case RecordSeverity.Warning:
					icon = CSEditorIcons.WarnSmallIcon;
					break;
				case RecordSeverity.Info:
					icon = CSEditorIcons.InfoSmallIcon;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var iconArea = EditorGUILayout.GetControlRect(false, 16, GUILayout.Width(16));
			var iconRect = new Rect(iconArea);

			GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleAndCrop);
		}
	}

	internal enum SettingsSearchSection : byte
	{
		Common,
		Neatness,
	}
}