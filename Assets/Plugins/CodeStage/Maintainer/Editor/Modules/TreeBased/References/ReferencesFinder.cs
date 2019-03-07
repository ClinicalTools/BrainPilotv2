#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.References
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using UnityEditor;

	using Core;
	using Entry;
	using Routines;
	using Settings;
	using Tools;
	using UI;
	using Debug = UnityEngine.Debug;
	using Object = UnityEngine.Object;

	/// <summary>
	/// Allows to find references of specific objects in your project (where objects are referenced).
	/// </summary>
	public class ReferencesFinder
	{
		internal const string ModuleName = "References Finder";

		internal const string ProgressCaption = ModuleName + ": phase {0} of {1}";
		internal const string ProgressText = "{0}: asset {1} of {2}";
		internal const int PhasesCount = 2;

		public static bool debugMode;

		internal static readonly List<AssetConjunctions> conjunctionInfoList = new List<AssetConjunctions>();

		/// <summary>
		/// Gets current Project View selection and calls GetReferences() method with respect to all settings regarding selections.
		/// </summary>
		/// <param name="showResults">Shows results in %Maintainer window if true.</param>
		/// <returns>Array of ReferencesTreeElement for the TreeView buildup or manual parsing.</returns>
		public static ReferencesTreeElement[] AddSelectedToSelectionAndRun(bool showResults = true)
		{
			var selection = GetSelectedAssets();
			return AddToSelectionAndRun(selection);
		}

		/// <summary>
		/// Adds new assets to the last selection if it existed and calls a GetReferences() with extended selection;
		/// </summary>
		/// <param name="selectedAsset">Additionally selected asset.</param>
		/// <param name="showResults">Shows results in %Maintainer window if true.</param>
		/// <returns>Array of ReferencesTreeElement for the TreeView buildup or manual parsing.</returns>
		public static ReferencesTreeElement[] AddToSelectionAndRun(string selectedAsset, bool showResults = true)
		{
			return AddToSelectionAndRun(new []{ selectedAsset }, showResults);
		}

		/// <summary>
		/// Adds new assets to the last selection if it existed and calls a GetReferences() with extended selection;
		/// </summary>
		/// <param name="selectedAssets">Additionally selected assets.</param>
		/// <param name="showResults">Shows results in %Maintainer window if true.</param>
		/// <returns>Array of ReferencesTreeElement for the TreeView buildup or manual parsing.</returns>
		public static ReferencesTreeElement[] AddToSelectionAndRun(string[] selectedAssets, bool showResults = true)
		{
			var additiveSelection = new FilterItem[selectedAssets.Length];
			for (var i = 0; i < selectedAssets.Length; i++)
			{
				additiveSelection[i] = FilterItem.Create(selectedAssets[i], FilterKind.Path);
			}

			return AddToSelectionAndRun(additiveSelection, showResults);
		}

		/// <summary>
		/// Adds new assets to the last selection if it existed and calls a GetReferences() with extended selection;
		/// </summary>
		/// <param name="selectedAssets">Additionally selected assets.</param>
		/// <param name="showResults">Shows results in %Maintainer window if true.</param>
		/// <returns>Array of ReferencesTreeElement for the TreeView buildup or manual parsing.</returns>
		public static ReferencesTreeElement[] AddToSelectionAndRun(FilterItem[] selectedAssets, bool showResults = true)
		{
			if (MaintainerPersonalSettings.References.selectedFindClearsResults)
			{
				SearchResultsStorage.ReferencesSearchSelection = new FilterItem[0];
				SearchResultsStorage.ReferencesSearchResults = new ReferencesTreeElement[0];
			}

			var currentSelection = SearchResultsStorage.ReferencesSearchSelection;

			var newItem = false;

			foreach (var selectedAsset in selectedAssets)
			{
				newItem |= CSFilterTools.TryAddNewItemToFilters(ref currentSelection, selectedAsset);
			}

			if (selectedAssets.Length == 1)
			{
				ReferencesTab.AutoSelectPath = selectedAssets[0].value;
			}

			if (newItem)
			{
				return GetReferences(currentSelection, showResults);
			}

			ReferencesTab.AutoShowExistsNotification = true;
			MaintainerWindow.ShowReferences();

			return SearchResultsStorage.ReferencesSearchResults;
		}

		/// <summary>
		/// Returns references of all assets at the project, e.g. where each asset is referenced.
		/// </summary>
		/// <param name="showResults">Shows results in %Maintainer window if true.</param>
		/// <returns>Array of ReferencesTreeElement for the TreeView buildup or manual parsing.</returns>
		public static ReferencesTreeElement[] GetReferences(bool showResults = true)
		{
			if (!MaintainerPersonalSettings.References.fullProjectScanWarningShown)
			{
				if (!EditorUtility.DisplayDialog(ModuleName,
					"Full project scan may take significant amount of time if your project is very big.\nAre you sure you wish to make a full project scan?\nThis message shows only before first full scan.",
					"Yes", "Nope"))
				{
					return null;
				}

				MaintainerPersonalSettings.References.fullProjectScanWarningShown = true;
				MaintainerSettings.Save();
			}

			return GetReferences(null, showResults);
		}

		/// <summary>
		/// Returns references of selectedAssets or all assets at the project (if selectedAssets is null), e.g. where each asset is referenced, with additional filtration of the results.
		/// </summary>
		/// <param name="selectedAssets">Assets you wish to show references for. Pass null to process all assets in the project.</param>
		/// <param name="showResults">Shows results in %Maintainer window if true.</param>
		/// <returns>Array of ReferencesTreeElement for the TreeView buildup or manual parsing.</returns>
		public static ReferencesTreeElement[] GetReferences(FilterItem[] selectedAssets, bool showResults = true)
		{
			var results = new List<ReferencesTreeElement>();

			conjunctionInfoList.Clear();

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

			try
			{
				var sw = Stopwatch.StartNew();

				CSEditorTools.lastRevealSceneOpenResult = null;

				var searchCanceled = LookForReferences(selectedAssets, results);
				sw.Stop();

				EditorUtility.ClearProgressBar();

				if (!searchCanceled)
				{
					var resultsCount = results.Count;
					if (resultsCount <= 1)
					{
						MaintainerWindow.ShowNotification("Nothing found!");
					}

					Debug.Log(Maintainer.LogPrefix + ModuleName + " results: " + (resultsCount - 1) +
					          " items found in " + sw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture) +
					          " seconds.");
				}
				else
				{
					Debug.Log(Maintainer.LogPrefix + ModuleName + "Search canceled by user!");
				}
			}
			catch (Exception e)
			{
				Debug.LogError(Maintainer.LogPrefix + ModuleName + ": " + e);
				EditorUtility.ClearProgressBar();
			}

			BuildSelectedAssetsFromResults(results);

			SearchResultsStorage.ReferencesSearchResults = results.ToArray();

			if (showResults)
			{
				MaintainerWindow.ShowReferences();
			}

			return results.ToArray();
		}

		internal static string[] GetSelectedAssets()
		{
			var selectedIDs = Selection.instanceIDs;
			return GetSelectedAssets(Selection.instanceIDs);
		}

		internal static string[] GetSelectedAssets(Object[] objects)
		{
			var selectedIDs = new int[objects.Length];
			for (var i = 0; i < objects.Length; i++)
			{
				selectedIDs[i] = objects[i].GetInstanceID();
			}

			return GetSelectedAssets(selectedIDs);
		}

		internal static string[] GetSelectedAssets(int[] instanceIDs)
		{
			var paths = new List<string>(instanceIDs.Length);

			foreach (var id in instanceIDs)
			{
				if (AssetDatabase.IsSubAsset(id)) continue;
				var path = AssetDatabase.GetAssetPath(id);
				path = CSPathTools.EnforceSlashes(path);
				if (!File.Exists(path) && !Directory.Exists(path)) continue;
				paths.Add(path);
			}

			return paths.ToArray();
		}

		private static void BuildSelectedAssetsFromResults(List<ReferencesTreeElement> results)
		{
			var resultsCount = results.Count;
			var showProgress = resultsCount > 500000;

			if (showProgress) EditorUtility.DisplayProgressBar(ModuleName, "Parsing results...", 0);

			var rootItems = new List<FilterItem>(resultsCount);
			var updateStep = Math.Max(resultsCount / MaintainerSettings.UpdateProgressStep, 1);
			for (var i = 0; i < resultsCount; i++)
			{
				if (showProgress && i % updateStep == 0) EditorUtility.DisplayProgressBar(ModuleName, "Parsing results...", (float)i / resultsCount);

				var result = results[i];
				if (result.depth != 0) continue;
				rootItems.Add(FilterItem.Create(result.assetPath, FilterKind.Path));
			}

			SearchResultsStorage.ReferencesSearchSelection = rootItems.ToArray();
		}

		private static bool LookForReferences(FilterItem[] selectedAssets, List<ReferencesTreeElement> results)
		{
			var canceled = !CSSceneTools.SaveCurrentModifiedScenes(false);

			if (!canceled)
			{
				var map = AssetsMap.GetUpdated();
				if (map == null) return true;

				var count = map.assets.Count;
				var updateStep = Math.Max(count / MaintainerSettings.UpdateProgressStep, 1);

				var root = new ReferencesTreeElement
				{
					id = results.Count,
					name = "root",
					depth = -1
				};
				results.Add(root);

				for (var i = 0; i < count; i++)
				{
					if (i % updateStep == 0 && EditorUtility.DisplayCancelableProgressBar(
						    string.Format(ProgressCaption, 1, PhasesCount),
						    string.Format(ProgressText, "Building references tree", i + 1, count),
						    (float) i / count))
					{
						canceled = true;
						break;
					}

					var assetInfo = map.assets[i];

					// excludes settings assets from the list depth 0 items
					if (assetInfo.Kind == AssetKind.Settings) continue;

					if (selectedAssets != null)
					{
						if (!CSFilterTools.IsValueMatchesAnyFilter(assetInfo.Path, selectedAssets)) continue;
					}

					if (CSFilterTools.IsValueMatchesAnyFilter(assetInfo.Path, MaintainerSettings.References.pathIgnoresFilters)) continue;

					var branchElements = new List<ReferencesTreeElement>();
					TreeBuilder.BuildTreeBranchRecursive(assetInfo, 0, results.Count, branchElements);
					results.AddRange(branchElements);
				}
			}

			if (!canceled)
			{
				canceled = ReferenceEntryFinder.FillReferenceEntries();
			}

			if (!canceled)
			{
				AssetsMap.Save();
			}

			if (canceled)
			{
				ReferencesTab.AutoShowExistsNotification = false;
				ReferencesTab.AutoSelectPath = null;
			}

			return canceled;
		}
	}
}