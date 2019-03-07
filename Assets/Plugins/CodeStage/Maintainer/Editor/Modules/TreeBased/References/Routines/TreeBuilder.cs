#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.References.Routines
{
	using System.Collections.Generic;
	using System.Linq;
	using Core;
	using Settings;
	using Tools;

	internal class TreeBuilder
	{
		public static ReferencesTreeElement BuildTreeBranchRecursive(AssetInfo referencedAsset, int depth, int id, List<ReferencesTreeElement> results)
		{
			if (AssetIsFilteredOut(referencedAsset, depth)) return null;
			
			var assetPath = referencedAsset.Path;
			var assetType = referencedAsset.Type;
			var assetTypeName = referencedAsset.SettingsKind == AssetSettingsKind.NotSettings ? assetType.Name : "Settings Asset";

			var element = new ReferencesTreeElement
			{
				id = id + results.Count,
				name = CSPathTools.NicifyAssetPath(referencedAsset.Path, referencedAsset.Kind),
				assetPath = assetPath,
				assetTypeName = assetTypeName,
				assetSize = referencedAsset.Size,
				assetSizeFormatted = CSEditorTools.FormatBytes(referencedAsset.Size),
				assetIsTexture = assetType.BaseType == CSReflectionTools.textureType,
				assetSettingsKind = referencedAsset.SettingsKind,
				depth = depth
			};
			results.Add(element);

			var recursionId = CheckParentsForRecursion(element, results);

			if (recursionId > -1)
			{
				element.name += " [RECURSION]";
				element.recursionId = recursionId;
				return element;
			}

			if (referencedAsset.referencedAtInfoList.Length > 0)
			{
				foreach (var referencedAtInfo in referencedAsset.referencedAtInfoList)
				{
					//if (CSFilterTools.IsValueMatchesAnyFilter(referencedAtInfo.assetInfo.Path, MaintainerSettings.References.pathIgnoresFilters)) continue;
					if (referencedAtInfo.assetInfo.Kind == AssetKind.FromPackage) continue;

					var childElement = BuildTreeBranchRecursive(referencedAtInfo.assetInfo, depth + 1, id, results);
					if (childElement == null) continue;

					var referencedAtType = referencedAtInfo.assetInfo.Type;

					if (referencedAtType == CSReflectionTools.gameObjectType || 
					    referencedAtType == CSReflectionTools.sceneAssetType ||
					    referencedAtType == CSReflectionTools.monoScriptType ||
						referencedAtType == CSReflectionTools.monoBehaviourType ||
						referencedAtType.BaseType == CSReflectionTools.scriptableObjectType)
					{
						if (referencedAtInfo.entries != null)
						{
							childElement.referencingEntries = referencedAtInfo.entries;
						}
						else
						{
							var collectedData = ReferencesFinder.conjunctionInfoList.FirstOrDefault(d => d.asset == referencedAtInfo.assetInfo);

							if (collectedData == null)
							{
								collectedData = new AssetConjunctions();
								ReferencesFinder.conjunctionInfoList.Add(collectedData);
								collectedData.asset = referencedAtInfo.assetInfo;
							}

							var tc = collectedData.conjunctions.FirstOrDefault(c => c.referencedAsset == referencedAsset);

							if (tc == null)
							{
								tc = new TreeConjunction
								{
									referencedAsset = referencedAsset,
									referencedAtInfo = referencedAtInfo
								};

								collectedData.conjunctions.Add(tc);
							}
							tc.treeElements.Add(childElement);
						}
					}
				}
			}

			return element;
		}

		private static bool AssetIsFilteredOut(AssetInfo referencedAsset, int depth)
		{
			if (MaintainerPersonalSettings.References.showAssetsWithoutReferences || depth != 0) return false;
			if (referencedAsset.referencedAtInfoList.Length == 0) return true;

			var allIgnored = true;
			foreach (var referencedAtInfo in referencedAsset.referencedAtInfoList)
			{
				if (CSFilterTools.IsValueMatchesAnyFilter(referencedAtInfo.assetInfo.Path, MaintainerSettings.References.pathIgnoresFilters)) continue;
				if (referencedAtInfo.assetInfo.Kind == AssetKind.FromPackage) continue;

				allIgnored = false;
				break;
			}

			return allIgnored;
		}

		private static int CheckParentsForRecursion(ReferencesTreeElement item, List<ReferencesTreeElement> items)
		{
			var result = -1;

			var lastDepth = item.depth;
			for (var i = items.Count - 1; i >= 0; i--)
			{
				var previousItem = items[i];
				if (previousItem.depth >= lastDepth) continue;

				lastDepth = previousItem.depth;
				if (item.assetPath != previousItem.assetPath) continue;

				result = previousItem.id;
				break;
			}

			return result;
		}
	}
}