#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.IO;

	using UnityEditor;
	using UnityEngine;

#if UNITY_2017_3_OR_NEWER
	using UnityEditor.Compilation;
#endif

	using Tools;

	[Serializable]
	public enum AssetKind
	{
		Regular = 0,
		Settings = 10,
		FromPackage = 20,
		Unsupported = 100
	}

	[Serializable]
	public enum AssetSettingsKind
	{
		NotSettings = 0,
		AudioManager = 100,
		ClusterInputManager = 200,
		DynamicsManager = 300,
		EditorBuildSettings = 400,
		EditorSettings = 500,
		GraphicsSettings = 600,
		InputManager = 700,
		NavMeshAreas = 800,
		NavMeshLayers = 900,
		NavMeshProjectSettings = 1000,
		NetworkManager = 1100,
		Physics2DSettings = 1200,
		ProjectSettings = 1300,
		PresetManager = 1400,
		QualitySettings = 1500,
		TagManager = 1600,
		TimeManager = 1700,
		UnityAdsSettings = 1800,
		UnityConnectSettings = 1900,
		VFXManager = 2000,
		Unknown = 100000
	}

	public class RawAssetInfo
	{
		public string path;
		public string guid;
		public AssetKind kind;
	}

	[Serializable]
	public class AssetInfo
	{
		public string GUID { get; private set; }
		public string Path { get; private set; }
		public AssetKind Kind { get; private set; }
		public AssetSettingsKind SettingsKind { get; private set; }
		public Type Type { get; private set; }
		public long Size { get; private set; }

		public string[] dependenciesGUIDs = new string[0];
		public ReferenceInfo[] referencesInfo = new ReferenceInfo[0];
		public ReferencedAtInfo[] referencedAtInfoList = new ReferencedAtInfo[0];

		public bool needToRebuildReferences = true;

		private long lastTimestamp;
		private long lastSize;
		private FileInfo fileInfo;

		[NonSerialized]
		private int[] allAssetObjects;

		public static AssetInfo Create(RawAssetInfo rawAssetInfo, Type type, AssetSettingsKind settingsKind)
		{
			if (string.IsNullOrEmpty(rawAssetInfo.guid))
			{
				Debug.LogError("Can't create AssetInfo since guid is invalid!");
				return null;
			}

			var newAsset = new AssetInfo
			{
				GUID = rawAssetInfo.guid,
				Path = rawAssetInfo.path,
				Kind = rawAssetInfo.kind,
				Type = type,
				SettingsKind = settingsKind,
				fileInfo = new FileInfo(rawAssetInfo.path)
			};

			newAsset.UpdateIfNeeded();

			return newAsset;
		}

		private AssetInfo() { }

		public bool Exists()
		{
			ActualizePath();
			fileInfo.Refresh();
			return fileInfo.Exists;
		}

		public void UpdateIfNeeded()
		{
			if (string.IsNullOrEmpty(Path))
			{
				Debug.LogWarning(Maintainer.LogPrefix + "Can't update Asset since path is not set!");
				return;
			}

			/*if (Path.Contains("Button.prefab"))
			{
				Debug.Log(Path);
			}*/

			fileInfo.Refresh();

			if (!fileInfo.Exists)
			{
				Debug.LogWarning(Maintainer.LogPrefix + "Can't update asset since file at path is not found:\n" + fileInfo.FullName + "\nAsset Path: " + Path);
				return;
			}

			var currentTimestamp = fileInfo.LastWriteTimeUtc.Ticks;
			var currentSize = fileInfo.Length;

			if (lastTimestamp == currentTimestamp && lastSize == currentSize)
			{
				for (var i = dependenciesGUIDs.Length - 1; i > -1; i--)
				{
					var guid = dependenciesGUIDs[i];
					var path = AssetDatabase.GUIDToAssetPath(guid);
					path = CSPathTools.EnforceSlashes(path);
					if (!string.IsNullOrEmpty(path) && File.Exists(path)) continue;

					ArrayUtility.RemoveAt(ref dependenciesGUIDs, i);
					foreach (var referenceInfo in referencesInfo)
					{
						if (referenceInfo.assetInfo.GUID != guid) continue;

						ArrayUtility.Remove(ref referencesInfo, referenceInfo);
						break;
					}
				}

				if (!needToRebuildReferences) return;
			}

			foreach (var referenceInfo in referencesInfo)
			{
				foreach (var info in referenceInfo.assetInfo.referencedAtInfoList)
				{
					if (info.assetInfo != this) continue;

					ArrayUtility.Remove(ref referenceInfo.assetInfo.referencedAtInfoList, info);
					break;
				}
			}

			lastTimestamp = currentTimestamp;
			lastSize = currentSize;
			needToRebuildReferences = true;
			Size = fileInfo.Length;

			referencesInfo = new ReferenceInfo[0];
			dependenciesGUIDs = new string[0];

			var dependencies = new List<string>();

			if (SettingsKind == AssetSettingsKind.NotSettings)
			{
				var getRegularDependencies = true;

				/* pre-regular dependencies additions */

#if UNITY_2017_3_OR_NEWER
				if (Type == CSReflectionTools.assemblyDefinitionAssetType)
				{
					if (Kind == AssetKind.Regular)
					{
						//TODO: check if bug 1020737 is fixed and this can be removed
						dependencies.AddRange(GetAssetsReferencedFromAssemblyDefinition(Path));
						getRegularDependencies = false;
					}
				}
#endif
				/* regular dependencies additions */

				if (getRegularDependencies) dependencies.AddRange(AssetDatabase.GetDependencies(Path, false));

				/* post-regular dependencies additions */

#if UNITY_2017_1_OR_NEWER
				if (Type == CSReflectionTools.spriteAtlasType)
				{
					CSArrayTools.TryAddIfNotExists(ref dependencies, GetGetAssetsInFoldersReferencedFromSpriteAtlas(Path));
				}
#endif
			}
			else
			{
				dependencies.AddRange(GetAssetsReferencedInPlayerSettingsAsset(Path, SettingsKind));
			}

			// kept for debugging purposes
			/*if (Path.Contains("1.unity"))
			{
				Debug.Log("1.unity non-recursive dependencies:");
				foreach (var reference in references)
				{
					Debug.Log(reference);
				}
			}*/

			if (Type == CSReflectionTools.shaderType)
			{
				// below is a workaround for the shader fallbacks issue 902729
				// we just manually check shader for fallbacks recursively to make sure all fallback shaders are included to the references list
				ScanShaderForNestedFallbacks(dependencies, Path);

				// below is an another workaround for dependencies not include #include-ed files, like *.cginc
				ScanFileForIncludes(dependencies, Path);
			}

			if (Type == CSReflectionTools.textAssetType && Path.EndsWith(".cginc"))
			{
				// below is an another workaround for dependencies not include #include-ed files, like *.cginc
				ScanFileForIncludes(dependencies, Path);
			}

			var guids = new string[dependencies.Count];

			for (var i = 0; i < dependencies.Count; i++)
			{
				guids[i] = AssetDatabase.AssetPathToGUID(dependencies[i]);
			}

			dependenciesGUIDs = guids;
		}

		public List<AssetInfo> GetReferencesRecursive()
		{
			var result = new List<AssetInfo>();

			WalkReferencesRecursive(result, referencesInfo);

			return result;
		}

		public List<AssetInfo> GetReferencedAtRecursive()
		{
			var result = new List<AssetInfo>();

			WalkReferencedAtRecursive(result, referencedAtInfoList);

			return result;
		}

		public void Clean()
		{
			foreach (var referenceInfo in referencesInfo)
			{
				foreach (var info in referenceInfo.assetInfo.referencedAtInfoList)
				{
					if (info.assetInfo != this) continue;
					ArrayUtility.Remove(ref referenceInfo.assetInfo.referencedAtInfoList, info);
					break;
				}
			}

			foreach (var referencedAtInfo in referencedAtInfoList)
			{
				foreach (var info in referencedAtInfo.assetInfo.referencesInfo)
				{
					if (info.assetInfo != this) continue;
					ArrayUtility.Remove(ref referencedAtInfo.assetInfo.referencesInfo, info);
					referencedAtInfo.assetInfo.needToRebuildReferences = true;
					break;
				}
			}
		}

		public int[] GetAllAssetObjects()
		{
			if (allAssetObjects != null) return allAssetObjects;

			var assetType = Type;
			var assetTypeName = assetType.Name;

			if ((assetType == CSReflectionTools.fontType ||
				assetType == CSReflectionTools.texture2DType ||
#if !UNITY_2018_1_OR_NEWER
				assetType == CSReflectionTools.substanceArchiveType ||
#endif
				assetType == CSReflectionTools.gameObjectType ||
				assetType == CSReflectionTools.defaultAssetType && Path.EndsWith(".dll") ||
				assetTypeName == "AudioMixerController" ||
				Path.EndsWith("LightingData.asset")) &&
				assetType != CSReflectionTools.lightingDataAsset)
			{
				var loadedObjects = AssetDatabase.LoadAllAssetsAtPath(Path);
				var referencedObjectsCandidatesList = new List<int>(loadedObjects.Length);
				for (var i = 0; i < loadedObjects.Length; i++)
				{
					var loadedObject = loadedObjects[i];
					if (loadedObject == null) continue;
					var instance = loadedObject.GetInstanceID();
					if (assetType == CSReflectionTools.gameObjectType)
					{
						if (!AssetDatabase.IsSubAsset(instance) && !AssetDatabase.IsMainAsset(instance)) continue;
					}

					referencedObjectsCandidatesList.Add(instance);
				}

				allAssetObjects = referencedObjectsCandidatesList.ToArray();
			}
			else
			{
				allAssetObjects = new[] { AssetDatabase.LoadMainAssetAtPath(Path).GetInstanceID() };
			}

			return allAssetObjects;
		}

		public static void ScanShaderForNestedFallbacks(List<string> referencePaths, string shaderPath)
		{
			var shaderCode = new StringBuilder();

			using (var sr = File.OpenText(shaderPath))
			{
				string s;
				while ((s = sr.ReadLine()) != null)
				{
					shaderCode.AppendLine(s);
				}
			}

			var shaderCodeString = shaderCode.ToString();

			var lastIndex = 0;

			while (lastIndex != -1)
			{
				lastIndex = shaderCodeString.IndexOf("fallback", lastIndex + 1, StringComparison.CurrentCultureIgnoreCase);
				if (lastIndex != -1)
				{
					var firstQuoteIndex = shaderCodeString.IndexOf('"', lastIndex);
					if (firstQuoteIndex == -1) continue;

					var whiteSpace = shaderCodeString.Substring(lastIndex + 8, firstQuoteIndex - (lastIndex + 8));
					whiteSpace = whiteSpace.Trim();
					if (whiteSpace.Length != 0) continue;

					var lastQuoteIndex = shaderCodeString.IndexOf('"', firstQuoteIndex + 1);
					if (lastQuoteIndex == -1) continue;

					var fallbackName = shaderCodeString.Substring(firstQuoteIndex + 1, lastQuoteIndex - firstQuoteIndex - 1);
					var fallbackShader = Shader.Find(fallbackName);
					var fallbackShaderPath = CSPathTools.EnforceSlashes(AssetDatabase.GetAssetPath(fallbackShader));

					if (!fallbackShaderPath.StartsWith("Assets", StringComparison.Ordinal)) continue;

					if (referencePaths.IndexOf(fallbackShaderPath) == -1)
					{
						referencePaths.Add(fallbackShaderPath);
						ScanFileForIncludes(referencePaths, fallbackShaderPath);
					}

					ScanShaderForNestedFallbacks(referencePaths, fallbackShaderPath);
				}
			}
		}

		private static void ScanFileForIncludes(List<string> referencePaths, string filePath)
		{
			var shaderLines = File.ReadAllLines(filePath);
			foreach (var line in shaderLines)
			{
				var includeIndex = line.IndexOf("include", StringComparison.Ordinal);
				if (includeIndex == -1) continue;

				var noSharp = line.IndexOf('#', 0, includeIndex) == -1;
				if (noSharp) continue;

				var indexOfFirstQuote = line.IndexOf('"', includeIndex);
				if (indexOfFirstQuote == -1) continue;

				var indexOfLastQuote = line.IndexOf('"', indexOfFirstQuote + 1);
				if (indexOfLastQuote == -1) continue;

				var path = line.Substring(indexOfFirstQuote + 1, indexOfLastQuote - indexOfFirstQuote - 1);
				path = CSPathTools.EnforceSlashes(path);

				string assetPath;

				if (path.StartsWith("Assets/"))
				{
					assetPath = path;
				}
				else if (path.IndexOf('/') != -1)
				{
					var folder = System.IO.Path.GetDirectoryName(filePath);
					if (folder == null) continue;

					var combinedPath = System.IO.Path.Combine(folder, path);
					var fullPath = CSPathTools.EnforceSlashes(System.IO.Path.GetFullPath(combinedPath));
					var assetsIndex = fullPath.IndexOf("Assets/", StringComparison.Ordinal);
					if (assetsIndex == -1) continue;

					assetPath = fullPath.Substring(assetsIndex, fullPath.Length - assetsIndex);
				}
				else
				{
					var folder = System.IO.Path.GetDirectoryName(filePath);
					if (folder == null) continue;

					assetPath = CSPathTools.EnforceSlashes(System.IO.Path.Combine(folder, path));
				}

				if (!File.Exists(assetPath)) continue;

				if (referencePaths.IndexOf(assetPath) != -1) continue;
				{
					referencePaths.Add(assetPath);
				}
			}
		}

		private void WalkReferencesRecursive(List<AssetInfo> result, ReferenceInfo[] referenceInfos)
		{
			foreach (var referenceInfo in referenceInfos)
			{
				if (result.IndexOf(referenceInfo.assetInfo) == -1)
				{
					result.Add(referenceInfo.assetInfo);
					WalkReferencesRecursive(result, referenceInfo.assetInfo.referencesInfo);
				}
			}
		}

		private void WalkReferencedAtRecursive(List<AssetInfo> result, ReferencedAtInfo[] referencedAtInfos)
		{
			foreach (var referencedAtInfo in referencedAtInfos)
			{
				if (result.IndexOf(referencedAtInfo.assetInfo) == -1)
				{
					result.Add(referencedAtInfo.assetInfo);
					WalkReferencedAtRecursive(result, referencedAtInfo.assetInfo.referencedAtInfoList);
				}
			}
		}

		private static string[] GetAssetsReferencedInPlayerSettingsAsset(string assetPath, AssetSettingsKind settingsKind)
		{
			var referencedAssets = new List<string>();

			if (settingsKind == AssetSettingsKind.EditorBuildSettings)
			{
				referencedAssets.AddRange(CSSceneTools.GetScenesInBuild(true));
			}
			else
			{
				var settingsAsset = AssetDatabase.LoadAllAssetsAtPath(assetPath);
				if (settingsAsset != null && settingsAsset.Length > 0)
				{
					var settingsAssetSerialized = new SerializedObject(settingsAsset[0]);

					var sp = settingsAssetSerialized.GetIterator();
					while (sp.Next(true))
					{
						if (sp.propertyType == SerializedPropertyType.ObjectReference)
						{
							var instanceId = sp.objectReferenceInstanceIDValue;
							if (instanceId != 0)
							{
								var path = CSPathTools.EnforceSlashes(AssetDatabase.GetAssetPath(instanceId));
								if (!string.IsNullOrEmpty(path) && path.StartsWith("Assets"))
								{
									if (referencedAssets.IndexOf(path) == -1)
										referencedAssets.Add(path);
								}
							}
						}
					}
				}
			}

			return referencedAssets.ToArray();
		}

		private void ActualizePath()
		{
			if (Kind == AssetKind.FromPackage) return;

			var actualPath = CSPathTools.EnforceSlashes(AssetDatabase.GUIDToAssetPath(GUID));
			if (!string.IsNullOrEmpty(actualPath) && actualPath != Path)
			{
				fileInfo = new FileInfo(actualPath);
				Path = actualPath;
			}
		}

#if UNITY_2017_1_OR_NEWER
		private List<string> GetGetAssetsInFoldersReferencedFromSpriteAtlas(string assetPath)
		{
			var result = new List<string>();

			var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.U2D.SpriteAtlas>(assetPath);
			var so = new SerializedObject(asset);

			// source: SpriteAtlasInspector
			var packablesProperty = so.FindProperty("m_EditorData.packables");
			if (packablesProperty == null || !packablesProperty.isArray)
			{
				Debug.LogError(Maintainer.LogPrefix + "Can't parse UnityEngine.U2D.SpriteAtlas, please report to " + Maintainer.SupportEmail);
			}
			else
			{
				var count = packablesProperty.arraySize;
				for (var i = 0; i < count; i++)
				{
					var packable = packablesProperty.GetArrayElementAtIndex(i);
					var objectReferenceValue = packable.objectReferenceValue;
					if (objectReferenceValue != null)
					{
						var path = AssetDatabase.GetAssetOrScenePath(objectReferenceValue);
						if (AssetDatabase.IsValidFolder(path))
						{
							var packablePaths = CSPathTools.GetAllPackableAssetsPathsRecursive(path);
							result.AddRange(packablePaths);
						}
					}
				}
			}

			return result;
		}
#endif

#if UNITY_2017_3_OR_NEWER
		private List<string> GetAssetsReferencedFromAssemblyDefinition(string assetPath)
		{
			var result = new List<string>();

			var asset = AssetDatabase.LoadAssetAtPath<UnityEditorInternal.AssemblyDefinitionAsset>(assetPath);
			var data = JsonUtility.FromJson<AssemblyDefinitionData>(asset.text);

			if (data.references != null && data.references.Length > 0)
			{
				foreach (var reference in data.references)
				{
					var assemblyDefinitionFilePathFromAssemblyName = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(reference);
					if (!string.IsNullOrEmpty(assemblyDefinitionFilePathFromAssemblyName))
					{
						assemblyDefinitionFilePathFromAssemblyName = CSPathTools.EnforceSlashes(assemblyDefinitionFilePathFromAssemblyName);
						result.Add(assemblyDefinitionFilePathFromAssemblyName);
					}
				}
			}

			data.references = null;

			return result;
		}

		private class AssemblyDefinitionData
		{
			public string[] references;
		}
#endif
	}
}