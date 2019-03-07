#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Routines
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core;
	using Settings;
	using Tools;
	using UnityEditor;
	using UnityEngine;

	internal class TargetCollector
	{
		internal static AssetInfo[] CollectTargetAssets()
		{
			var map = AssetsMap.GetUpdated();
			if (map == null)
			{
				Debug.LogError(Maintainer.LogPrefix + "Can't get updated assets map!");
				return null;
			}

			EditorUtility.DisplayProgressBar(IssuesFinder.ModuleName, "Collecting input data...", 0);

			var supportedKinds = new List<AssetKind> {AssetKind.Regular, AssetKind.Settings};
			var assets = CSFilterTools.GetAssetInfosWithKinds(map.assets, supportedKinds);

			var result = new HashSet<AssetInfo>();

			try
			{
				var targetAssetTypes = new List<TypeFilter>();

				if (MaintainerSettings.Issues.lookInScenes)
				{
					switch (MaintainerSettings.Issues.scenesSelection)
					{
						case IssuesFinderSettings.ScenesSelection.AllScenes:
						{
							targetAssetTypes.Add(new TypeFilter(CSReflectionTools.sceneAssetType));

							break;
						}
						case IssuesFinderSettings.ScenesSelection.IncludedScenes:
						{
							if (MaintainerSettings.Issues.includeScenesInBuild)
							{
								var paths = CSSceneTools.GetScenesInBuild(!MaintainerSettings.Issues.includeOnlyEnabledScenesInBuild);
								result.UnionWith(CSFilterTools.GetAssetInfosWithPaths(assets, paths));
							}

							var assetInfos = CSFilterTools.FilterAssetInfos(assets, MaintainerSettings.Issues.sceneIncludesFilters);
							result.UnionWith(assetInfos);

							break;
						}
						case IssuesFinderSettings.ScenesSelection.OpenedScenesOnly:
						{
							var paths = CSSceneTools.GetScenesSetup().Select(s => s.path).ToArray();
							result.UnionWith(CSFilterTools.GetAssetInfosWithPaths(assets, paths));

							break;
						}
						default:
							throw new ArgumentOutOfRangeException();
					}
				}

				if (MaintainerSettings.Issues.lookInAssets)
				{
					targetAssetTypes.Add(new TypeFilter(CSReflectionTools.scriptableObjectType, true));
					targetAssetTypes.Add(new TypeFilter(null));

					if (MaintainerSettings.Issues.scanGameObjects)
					{
						targetAssetTypes.Add(new TypeFilter(CSReflectionTools.gameObjectType));
					}
				}

				var filtered = CSFilterTools.FilterAssetInfos(
					assets,
					targetAssetTypes,
					MaintainerSettings.Issues.pathIncludesFilters,
					MaintainerSettings.Issues.pathIgnoresFilters
				);

				result.UnionWith(filtered);

				if (MaintainerSettings.Issues.lookInProjectSettings)
				{
					result.UnionWith(CSFilterTools.GetAssetInfosWithKind(assets, AssetKind.Settings));
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

			var resultArray = new AssetInfo[result.Count];
			result.CopyTo(resultArray);
			return resultArray;
		}
	}
}