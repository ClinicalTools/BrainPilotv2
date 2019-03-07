#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.References.Entry
{
	using System;
	using System.Collections.Generic;
	using Core;
	using Settings;
	using Tools;
	using UnityEditor;
	using UnityEngine;
	using Object = UnityEngine.Object;

#if UNITY_2018_2_OR_NEWER
	using UnityEngine.Tilemaps;
#endif

	internal static class ReferenceEntryFinder
	{
		private class CachedObjectData
		{
			public long objectId;
			public string transformPath;
		}

		private static AssetConjunctions assetConjunctions;
		private static CachedObjectData currentObjectCache;

		private static Location currentLocation;

		public static bool FillReferenceEntries()
		{
			var canceled = false;

			var count = ReferencesFinder.conjunctionInfoList.Count;
			var updateStep = Math.Max(count / MaintainerSettings.UpdateProgressStep, 1);

			for (var i = 0; i < count; i++)
			{
				if ((i < 10 || i % updateStep == 0) && EditorUtility.DisplayCancelableProgressBar(
						string.Format(ReferencesFinder.ProgressCaption, 2, ReferencesFinder.PhasesCount), string.Format(ReferencesFinder.ProgressText, "Filling reference details", i + 1, count),
						(float)i / count))
				{
					canceled = true;
					break;
				}

				assetConjunctions = ReferencesFinder.conjunctionInfoList[i];

				if (assetConjunctions.asset.Type == CSReflectionTools.gameObjectType)
				{
					ProcessPrefab();
				}
				else if (assetConjunctions.asset.Type == CSReflectionTools.sceneAssetType)
				{
					ProcessScene();
				}
				else if (assetConjunctions.asset.Type == CSReflectionTools.monoScriptType)
				{
					ProcessScriptAsset();
				}
				else if (assetConjunctions.asset.Type.BaseType == CSReflectionTools.scriptableObjectType || 
				         assetConjunctions.asset.Type == CSReflectionTools.monoBehaviourType)
				{
					ProcessScriptableObjectAsset();
				}

				foreach (var conjunction in assetConjunctions.conjunctions)
				{
					var referencedAtInfo = conjunction.referencedAtInfo;

					if (referencedAtInfo.entries == null || referencedAtInfo.entries.Length == 0)
					{
						var newEntry = new ReferencingEntryData
						{
							location = Location.NotFound,
							prefixLabel = "No exact reference place found."
						};

						if (referencedAtInfo.assetInfo.Type == CSReflectionTools.sceneAssetType)
						{
							var sceneSpecificEntry = new ReferencingEntryData
							{
								location = Location.NotFound,
								prefixLabel = "Please try to remove all missing prefabs/scripts (if any) and re-save scene, it may cleanup junky dependencies."
							};

							referencedAtInfo.entries = new[] { newEntry, sceneSpecificEntry };
						}
						else if (referencedAtInfo.assetInfo.Type == CSReflectionTools.gameObjectType)
						{
							var prefabSpecificEntry = new ReferencingEntryData
							{
								location = Location.NotFound,
								prefixLabel = "Please try to re-Apply prefab explicitly, this may clean up junky dependencies."
							};

							referencedAtInfo.entries = new[] { newEntry, prefabSpecificEntry };
						}
						else
						{
							referencedAtInfo.entries = new[] { newEntry };
						}

						if (ReferencesFinder.debugMode)
						{
							Debug.LogWarning(Maintainer.ConstructWarning("Couldn't determine where exactly this asset is referenced: " + conjunction.referencedAsset.Path, ReferencesFinder.ModuleName));
						}
					}

					foreach (var targetTreeElement in conjunction.treeElements)
					{
						targetTreeElement.referencingEntries = referencedAtInfo.entries;
					}
				}
			}

			return canceled;
		}

		private static void ProcessPrefab()
		{
			var path = assetConjunctions.asset.Path;
			var assetObject = AssetDatabase.LoadMainAssetAtPath(path);
			if (assetObject == null) return;

			var prefabRootGameObject = assetObject as GameObject;
			if (prefabRootGameObject == null) return;

			currentLocation = Location.PrefabAssetGameObject;

			CSTraverseTools.TraversePrefabGameObjects(prefabRootGameObject, true, OnGameObjectTraverse);

			// specific cases handling for main asset -----------------------------------------------------

			/*var importSettings = AssetImporter.GetAtPath(path) as ModelImporter;
			if (importSettings == null) return;

			var settings = new EntryAddSettings { suffix = "| Model Importer: RIG > Source" };
			TryAddEntryToMatchedConjunctions(assetConjunctions.conjunctions, prefabRootGameObject, importSettings.sourceAvatar, settings);

			for (var i = 0; i < importSettings.clipAnimations.Length; i++)
			{
				var clipAnimation = importSettings.clipAnimations[i];
				settings.suffix = "| Model Importer: Animations [" + clipAnimation.name + "] > Mask";
				TryAddEntryToMatchedConjunctions(assetConjunctions.conjunctions, prefabRootGameObject, clipAnimation.maskSource, settings);
			}*/

			
			var allObjectsInPrefab = AssetDatabase.LoadAllAssetsAtPath(path);

			foreach (var objectOnPrefab in allObjectsInPrefab)
			{
				if (objectOnPrefab == null) continue;
				currentObjectCache = null;

				if (objectOnPrefab is GameObject || objectOnPrefab is Component) continue;

				currentObjectCache = null;
				currentLocation = Location.PrefabAssetObject;

				var addSettings = new EntryAddSettings
				{
					componentIndex = -1,
				};

				TraverseObjectProperties(objectOnPrefab, objectOnPrefab, addSettings);

				/*if (AssetDatabase.IsMainAsset(objectOnPrefab))
				{

				}
				else*/
				{
					// specific cases handling ------------------------------------------------------------------------
					/*if (objectOnPrefab is BillboardAsset)
					{
						var billboardAsset = objectOnPrefab as BillboardAsset;
						var settings = new EntryAddSettings { suffix = "| BillboardAsset: Material" };
						TryAddEntryToMatchedConjunctions(assetConjunctions.conjunctions, billboardAsset, billboardAsset.material, settings);
					}
					else if (objectOnPrefab is TreeData)
					{
						CachedObjectData objectInAssetCachedData = null;
						InspectComponent(assetConjunctions.conjunctions, objectOnPrefab, objectOnPrefab, -1, true, ref objectInAssetCachedData);
					}*/
				}
			}
		}

		private static void ProcessScene()
		{
			var path = assetConjunctions.asset.Path;
			var openSceneResult = CSSceneTools.OpenScene(path);
			if (!openSceneResult.success)
			{
				Debug.LogWarning(Maintainer.ConstructWarning("Can't open scene " + path));
				return;
			}

			SceneSettingsProcessor.Process(assetConjunctions.conjunctions);

			currentLocation = Location.SceneGameObject;
			CSTraverseTools.TraverseSceneGameObjects(openSceneResult.scene, true, OnGameObjectTraverse);

			CSSceneTools.CloseOpenedSceneIfNeeded(openSceneResult);
		}

		private static void ProcessScriptAsset()
		{
			var path = assetConjunctions.asset.Path;
			var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
			if (mainAsset == null) return;

			currentObjectCache = null;
			currentLocation = Location.ScriptAsset;

			var addSettings = new EntryAddSettings
			{
				componentIndex = -1,
			};

			TraverseObjectProperties(mainAsset, mainAsset, addSettings);
		}

		private static void ProcessScriptableObjectAsset()
		{
			var path = assetConjunctions.asset.Path;
			var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
			if (mainAsset == null) return;

			currentObjectCache = null;
			currentLocation = Location.ScriptableObjectAsset;

			var addSettings = new EntryAddSettings
			{
				componentIndex = -1,
			};

			TraverseObjectProperties(mainAsset, mainAsset, addSettings);
		}

		private static bool OnGameObjectTraverse(ObjectTraverseInfo traverseInfo)
		{
			var target = traverseInfo.current;
			currentObjectCache = null;

			//Debug.Log("OnGameObjectTraverse " + target);

			if (traverseInfo.inPrefabInstance)
			{
				//Debug.Log("traverseInfo.dirtyComponents " + traverseInfo.dirtyComponents);
				var prefabAssetSource = CSPrefabTools.GetAssetSource(target);
				if (prefabAssetSource != null)
				{
					var instanceId = prefabAssetSource.GetInstanceID();
					TryAddEntryToMatchedConjunctions(target, instanceId, null);

					if (traverseInfo.dirtyComponents == null)
					{
						traverseInfo.skipCurrentTree = true;
						return true;
					}
				}
			}

			var thumbnail = AssetPreview.GetMiniThumbnail(target);
			if (thumbnail != null && (thumbnail.hideFlags & HideFlags.HideAndDontSave) == 0)
			{
				var addSettings = new EntryAddSettings
				{
					prefix = "[Object Icon]",
				};
				TryAddEntryToMatchedConjunctions(target, thumbnail.GetInstanceID(), addSettings);
			}

			CSTraverseTools.TraverseGameObjectComponents(traverseInfo, OnGameObjectComponentTraverse);

			return true;
		}

		private static void OnGameObjectComponentTraverse(ObjectTraverseInfo traverseInfo, Component component, int orderIndex)
		{
			if (component == null) return;

			var target = traverseInfo.current;
			var componentName = component.GetType().Name;

			if ((component.hideFlags & HideFlags.HideInInspector) != 0)
			{
				componentName += " (HideInInspector)";
				orderIndex = -1;
			}

			var addSettings = new EntryAddSettings
			{
				componentName = componentName,
				componentIndex = orderIndex,
			};

			TraverseObjectProperties(target, component, addSettings);
		}

		private static void TraverseObjectProperties(Object inspectedUnityObject, Object target, EntryAddSettings addSettings)
		{
#if UNITY_2018_2_OR_NEWER
			if (target is Tilemap)
			{
				ManualComponentProcessor.ProcessTilemap(inspectedUnityObject, (Tilemap)target, addSettings);
				return;
			}
#endif
			GenericObjectProcessor.ProcessObject(currentLocation, inspectedUnityObject, target, addSettings);
		}

		internal static void TryAddEntryToMatchedConjunctions(Object lookAt, int lookForInstanceId, EntryAddSettings settings)
		{
			var lookAtGameObject = lookAt as GameObject;

			for (var i = 0; i < assetConjunctions.conjunctions.Count; i++)
			{
				var conjunction = assetConjunctions.conjunctions[i];
				var referencedAssetObjects = conjunction.referencedAsset.GetAllAssetObjects();

				var match = false;
				for (var j = 0; j < referencedAssetObjects.Length; j++)
				{
					if (referencedAssetObjects[j] != lookForInstanceId) continue;

					match = true;
					break;
				}

				if (!match) continue;

				if (currentObjectCache == null)
				{
					currentObjectCache = new CachedObjectData();
					currentObjectCache.objectId = CSObjectTools.GetUniqueObjectId(lookAt);

					if (currentLocation == Location.SceneGameObject || currentLocation == Location.PrefabAssetGameObject)
					{
						if (lookAtGameObject != null)
						{
							var transform = lookAtGameObject.transform;
							currentObjectCache.transformPath = CSEditorTools.GetFullTransformPath(transform);
						}
						else
						{
							currentObjectCache.transformPath = lookAt.name;
						}
					}
					else if (currentLocation == Location.PrefabAssetObject)
					{
						currentObjectCache.transformPath = lookAt.name;
					}
					else
					{
						currentObjectCache.transformPath = string.Empty;
					}
				}

				var newEntry = new ReferencingEntryData
				{
					location = currentLocation,
					objectId = currentObjectCache.objectId,
					transformPath = currentObjectCache.transformPath
				};

				if (settings != null)
				{
					newEntry.componentName = settings.componentName;
					newEntry.componentId = settings.componentIndex;
					newEntry.prefixLabel = settings.prefix;
					newEntry.suffixLabel = settings.suffix;
					newEntry.propertyPath = settings.propertyPath;
				}

				conjunction.referencedAtInfo.AddNewEntry(newEntry);
			}
		}
	}
}