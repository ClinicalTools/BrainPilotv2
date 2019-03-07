#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization.Formatters.Binary;
	
	using UnityEditor;
	using UnityEngine;

	using Settings;
	using Tools;

	[Serializable]
	public class AssetsMap
	{
		private const string MapPath = "Library/MaintainerMap.dat";

		private static AssetsMap cachedMap;

		public readonly List<AssetInfo> assets = new List<AssetInfo>();
		public string version;

		public static AssetsMap CreateNew()
		{
			Delete();
			return GetUpdated();
		}

		public static void Delete()
		{
			cachedMap = null;
			CSFileTools.DeleteFile(MapPath);
		}

		public static AssetsMap GetUpdated()
		{
			if (cachedMap == null)
			{
				cachedMap = LoadMap(MapPath);
			}

			if (cachedMap == null)
			{
				cachedMap = new AssetsMap {version = Maintainer.Version};
			}

			try
			{
				if (UpdateMap(cachedMap))
				{
					SaveMap(MapPath, cachedMap);
				}
				else
				{
					cachedMap.assets.Clear();
					cachedMap = null;
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}

			EditorUtility.ClearProgressBar();

			return cachedMap;
		}

		public static void Save()
		{
			if (cachedMap != null)
			{
				SaveMap(MapPath, cachedMap);
			}
			else
			{
				Debug.LogError(Maintainer.ConstructError("Can't save AssetsMap, no cache found!"));
			}
		}

		public static int GetAssetInfoIndex(AssetInfo assetInfo)
		{
			var items = GetUpdated().assets;
			return items.IndexOf(assetInfo);
		}

		public static AssetInfo GetAssetInfoAtIndex(int index)
		{
			var items = GetUpdated().assets;
			return index > items.Count - 1 ? null : items[index];
		}

		public static AssetInfo GetAssetInfoWithPath(string path)
		{
			var items = GetUpdated().assets;
			return items.FirstOrDefault(item => item.Path == path);
		}

		private static bool UpdateMap(AssetsMap map)
		{
			// ----------------------------------------
			// getting all valid assets within project 
			// ----------------------------------------
			if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 1 of 4", "Getting all valid assets...", 0))
			{
				Debug.LogError(Maintainer.LogPrefix + "Assets Map update was canceled by user.");
				return false;
			}

			var allAssetPaths = AssetDatabase.GetAllAssetPaths();
			var validNewAssets = new List<RawAssetInfo>(allAssetPaths.Length);

			foreach (var assetPath in allAssetPaths)
			{
				/*if (assetPath.Contains(@"ScriptableObjectScriptWithMissingScript"))
				{
					Debug.Log(assetPath);
				}*/

				var kind = CSEditorTools.GetAssetKind(assetPath);
				if (kind == AssetKind.Unsupported) continue;

				if (!File.Exists(assetPath)) continue;
				if (AssetDatabase.IsValidFolder(assetPath)) continue;

				var guid = AssetDatabase.AssetPathToGUID(assetPath);
				var rawInfo = new RawAssetInfo
				{
					path = CSPathTools.EnforceSlashes(assetPath),
					guid = guid,
					kind = kind,
				};

				validNewAssets.Add(rawInfo);
			}

			// -----------------------------
			// checking existing map assets 
			// -----------------------------

			if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 2 of 4", "Checking existing assets in map...", 0))
			{
				Debug.LogError(Maintainer.LogPrefix + "Assets Map update was canceled by user.");
				return false;
			}

			var count = map.assets.Count;
			var updateStep = Math.Max(count / MaintainerSettings.UpdateProgressStep, 1);
			for (var i = count - 1; i > -1; i--)
			{
				if (i % updateStep == 0 && i != 0)
				{
					var index = count - i;
					if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 2 of 4", "Checking existing assets in map..." + index + "/" + count, (float) index / count))
					{
						EditorUtility.ClearProgressBar();
						Debug.LogError(Maintainer.LogPrefix + "Assets Map update was canceled by user.");
						return false;
					}
				}

				var assetInMap = map.assets[i];
				if (assetInMap.Exists())
				{
					validNewAssets.RemoveAll(a => a.guid == assetInMap.GUID);
					assetInMap.UpdateIfNeeded();
				}
				else
				{
					assetInMap.Clean();
					map.assets.RemoveAt(i);
				}
			}

			// ------------------------
			// dealing with new assets
			// ------------------------

			if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 3 of 4", "Looking for new assets...", 0))
			{
				Debug.LogError(Maintainer.LogPrefix + "Assets Map update was canceled by user.");
				return false;
			}

			count = validNewAssets.Count;
			updateStep = Math.Max(count / MaintainerSettings.UpdateProgressStep, 1);

			for (var i = 0; i < count; i++)
			{
				if (i % updateStep == 0 && i != 0)
				{
					if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 3 of 4",
						"Looking for new assets..." + (i + 1) + "/" + count, (float)i / count))
					{
						Debug.LogError(Maintainer.LogPrefix + "Assets Map update was canceled by user.");
						return false;
					}
				}

				var rawAssetInfo = validNewAssets[i];
				var rawAssetInfoPath = rawAssetInfo.path;

				var type = AssetDatabase.GetMainAssetTypeAtPath(rawAssetInfoPath);
				if (type == null)
				{
					var loadedAsset = AssetDatabase.LoadMainAssetAtPath(rawAssetInfoPath);
					if (loadedAsset == null)
					{
						if (rawAssetInfo.kind != AssetKind.FromPackage)
						{
							if (!CSAssetTools.IsAssetScriptableObjectWithMissingScript(rawAssetInfoPath))
							{
								Debug.LogWarning(Maintainer.LogPrefix + "Can't retrieve type of the asset:\n" +
								                 rawAssetInfoPath);
								continue;
							}
						}
						else
						{
							continue;
						}
					}
					else
					{
						type = loadedAsset.GetType();
					}
				}

				var settingsKind = rawAssetInfo.kind == AssetKind.Settings ? GetSettingsKind(rawAssetInfoPath) : AssetSettingsKind.NotSettings;

				var asset = AssetInfo.Create(rawAssetInfo, type, settingsKind);
				map.assets.Add(asset);
			}

			if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 4 of 4", "Generating links...", 0))
			{
				Debug.LogError(Maintainer.LogPrefix + "Assets Map update was canceled by user.");
				return false;
			}

			count = map.assets.Count;
			updateStep = Math.Max(count / MaintainerSettings.UpdateProgressStep, 1);

			for (var i = 0; i < count; i++)
			{
				if (i % updateStep == 0 && i != 0)
				{
					if (EditorUtility.DisplayCancelableProgressBar("Updating Assets Map, phase 4 of 4", "Generating links..." + (i + 1) + "/" + count, (float)i / count))
					{
						Debug.LogError(Maintainer.LogPrefix + "Assets Map update was canceled by user.");
						return false;
					}
				}

				var asset = map.assets[i];

				if (!asset.needToRebuildReferences) continue;

				var dependencies = asset.dependenciesGUIDs;
				var referenceInfos = new List<ReferenceInfo>(asset.referencesInfo);

				foreach (var mapAsset in map.assets)
				{
					var referencedAtInfos = new List<ReferencedAtInfo>(mapAsset.referencedAtInfoList);

					foreach (var dependency in dependencies)
					{
						if (mapAsset.GUID != dependency) continue;
						if (mapAsset.Type == asset.Type && asset.Type == CSReflectionTools.fontType) continue;

						var referencedAtInfo = new ReferencedAtInfo()
						{
							assetInfo = asset
						};
						referencedAtInfos.Add(referencedAtInfo);

						var referenceInfo = new ReferenceInfo()
						{
							assetInfo = mapAsset
						};
						referenceInfos.Add(referenceInfo);
					}

					mapAsset.referencedAtInfoList = referencedAtInfos.ToArray();
				}

				asset.referencesInfo = referenceInfos.ToArray();
				asset.needToRebuildReferences = false;
			}

			/*Debug.Log("Total assets in map: " + map.assets.Count);
			foreach (var mapAsset in map.assets)
			{
				//if (!(mapAsset.path.Contains("frag_ab") || mapAsset.path.Contains("frag_ac"))) continue;
				if (!mapAsset.Path.Contains("NewAssembly")) continue;

				Debug.Log("==================================================\n" + mapAsset.Path + "\n" + mapAsset.Path);
				Debug.Log("[REFERENCED BY]");
				foreach (var reference in mapAsset.referencedAtInfoList)
				{
					Debug.Log(reference.assetInfo.Path);
				}

				Debug.Log("[REFERENCES]");
				foreach (var reference in mapAsset.referencesInfo)
				{
					Debug.Log(reference.assetInfo.Path);
				} 
			}*/

			return true;
		}

		private static AssetsMap LoadMap(string path)
		{
			if (!File.Exists(path)) return null;

			var fileSize = new FileInfo(path).Length;

			if (fileSize > 500000)
			{
				EditorUtility.DisplayProgressBar("Loading Assets Map", "Please wait...", 0);
			}

			AssetsMap result = null;
			var bf = new BinaryFormatter();
			var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			try
			{
				result = bf.Deserialize(stream) as AssetsMap;

				if (result != null && result.version != Maintainer.Version)
				{
					result = null;
				}
			}
			catch (Exception)
			{
				Debug.Log(Maintainer.LogPrefix + "Couldn't read assets map (more likely you've updated Maintainer recently).\nThis message is harmless unless repeating on every Maintainer run."); //"\n" + ex);
			}
			finally
			{
				stream.Close();
			}

			EditorUtility.ClearProgressBar();

			return result;
		}

		private static void SaveMap(string path, AssetsMap map)
		{
			if (map.assets.Count > 10000)
			{
				EditorUtility.DisplayProgressBar("Saving Assets Map", "Please wait...", 0);
			}

			var bf = new BinaryFormatter();
			var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
			bf.Serialize(stream, map);
			stream.Close();

			EditorUtility.ClearProgressBar();
		}
		
		private static AssetSettingsKind GetSettingsKind(string assetPath)
		{
			var result = AssetSettingsKind.Unknown;

			var fileName = Path.GetFileNameWithoutExtension(assetPath);
			if (!string.IsNullOrEmpty(fileName))
			{
				try
				{
					result = (AssetSettingsKind)Enum.Parse(CSReflectionTools.assetSettingsKindType, fileName);
				}
				catch (Exception)
				{
					// ignored
				}
			}

			return result;
		}
	}
}