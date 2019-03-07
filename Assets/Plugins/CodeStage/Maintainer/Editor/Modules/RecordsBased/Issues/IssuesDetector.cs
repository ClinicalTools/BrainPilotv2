#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues
{
	using System.Collections.Generic;
	using Core;
	using Detectors;
	using Settings;
	using Tools;
	using UnityEditor;
	using UnityEngine;
	using Object = UnityEngine.Object;

	internal static class IssuesDetector
	{
		internal static MissingReferenceDetector missingReferenceDetector;

		internal static MissingComponentDetector missingComponentDetector;
		internal static DuplicateComponentDetector duplicateComponentDetector;
		internal static MissingPrefabDetector missingPrefabDetector;
		internal static InconsistentTerrainDataDetector inconsistentTerrainDataDetector;

		internal static HugePositionDetector hugePositionDetector;
		internal static EmptyLayerNameDetector emptyLayerNameDetector;

		internal static DuplicateLayersDetector duplicateLayersDetector;

		private static AssetInfo currentAsset;
		private static RecordLocation currentLocation;
		private static GameObject currentGameObject;

		public static void Init(List<IssueRecord> issues)
		{
			missingReferenceDetector = new MissingReferenceDetector(issues);

			missingComponentDetector = new MissingComponentDetector(issues);
			duplicateComponentDetector = new DuplicateComponentDetector(issues);
			inconsistentTerrainDataDetector = new InconsistentTerrainDataDetector(issues);
			missingPrefabDetector = new MissingPrefabDetector(issues);

			hugePositionDetector = new HugePositionDetector(issues);
			emptyLayerNameDetector = new EmptyLayerNameDetector(issues);

			duplicateLayersDetector = new DuplicateLayersDetector(issues);
		}

		/////////////////////////////////////////////////////////////////////////
		// Scenes Processing
		/////////////////////////////////////////////////////////////////////////

		public static void SceneStart(AssetInfo asset)
		{
			currentLocation = RecordLocation.Scene;
			currentAsset = asset;

			missingReferenceDetector.TryDetectIssuesInSceneSettings(currentAsset);
		}

		public static void SceneEnd(AssetInfo asset)
		{
			currentLocation = RecordLocation.Unknown;
			currentAsset = null;
			currentGameObject = null;
		}

		/////////////////////////////////////////////////////////////////////////
		// Prefab Assets Processing
		/////////////////////////////////////////////////////////////////////////

		public static void StartPrefabAsset(AssetInfo asset)
		{
			currentLocation = RecordLocation.Prefab;
			currentAsset = asset;
		}

		public static void EndPrefabAsset(AssetInfo asset)
		{
			currentLocation = RecordLocation.Unknown;
			currentAsset = null;
			currentGameObject = null;
		}

		/////////////////////////////////////////////////////////////////////////
		// Game Objects Processing (both from Scenes and Prefab Assets)
		/////////////////////////////////////////////////////////////////////////

		public static bool StartGameObject(GameObject target, bool inPrefabInstance, out bool skipTree)
		{
			skipTree = false;

			if (!MaintainerSettings.Issues.touchInactiveGameObjects)
			{
				if (currentLocation == RecordLocation.Scene)
				{
					if (!target.activeInHierarchy) return false;
				}
				else
				{
					if (!target.activeSelf) return false;
				}
			}

			if (inPrefabInstance)
			{
				if (missingPrefabDetector.TryDetectIssue(currentLocation, currentAsset.Path, target))
				{
					skipTree = true;
					return false;
				}
			}

			currentGameObject = target;
			missingComponentDetector.StartGameObject();
			duplicateComponentDetector.StartGameObject();

			hugePositionDetector.TryDetectIssue(currentLocation, currentAsset.Path, target);
			emptyLayerNameDetector.TryDetectIssue(currentLocation, currentAsset.Path, target);

			return true;
		}

		public static void EndGameObject(GameObject target)
		{
			missingComponentDetector.TryDetectIssue(currentLocation, currentAsset.Path, target);
			inconsistentTerrainDataDetector.TryDetectIssue(currentLocation, currentAsset.Path, target);
		}

		/////////////////////////////////////////////////////////////////////////
		// Game Object's Components Processing
		/////////////////////////////////////////////////////////////////////////

		public static void ProcessComponent(Component component, int orderIndex)
		{
			if (missingComponentDetector.CheckAndRecordNullComponent(component))
			{
				return;
			}

			if ((component.hideFlags & HideFlags.HideInInspector) != 0)
			{
				return;
			}

			if (!MaintainerSettings.Issues.touchDisabledComponents)
			{
				if (EditorUtility.GetObjectEnabled(component) == 0) return;
			}

			var componentType = component.GetType();
			var componentName = componentType.Name;

			if (MaintainerSettings.Issues.componentIgnoresFilters != null &&
				MaintainerSettings.Issues.componentIgnoresFilters.Length > 0)
			{
				if (CSFilterTools.IsValueMatchesAnyFilterOfKind(componentName, MaintainerSettings.Issues.componentIgnoresFilters, FilterKind.Type)) return;
			}

			duplicateComponentDetector.ProcessComponent(component, componentType);

			var shouldCheckPropertiesForDuplicate = duplicateComponentDetector.IsPropertiesHashCalculationRequired();
			if (shouldCheckPropertiesForDuplicate)
			{
				// skipping duplicate search for non-standard components with invisible properties
				var baseType = componentType.BaseType;
				if (baseType != null)
				{
					if (baseType.Name == "MegaModifier")
					{
						shouldCheckPropertiesForDuplicate = false;
						duplicateComponentDetector.SkipComponent();
					}
				}
			}

			var shouldTraverseProperties = missingReferenceDetector.Enabled || shouldCheckPropertiesForDuplicate;
			if (shouldTraverseProperties)
			{
				var initialInfo = new SerializedObjectTraverseInfo(component);

				CSTraverseTools.TraverseObjectProperties(initialInfo, (info, property) =>
				{
					if (property.type == "UnityEvent")
					{
						missingReferenceDetector.TryDetectUnityEventIssues(currentLocation, currentAsset.Path,
							currentGameObject, componentType, componentName, orderIndex, property);

						info.skipCurrentTree = true;
						return;
					}

					missingReferenceDetector.TryDetectIssue(currentLocation, currentAsset.Path, currentGameObject, componentType, componentName, orderIndex, property);

					if (shouldCheckPropertiesForDuplicate) duplicateComponentDetector.ProcessProperty(property);
				});
			}

			if (shouldCheckPropertiesForDuplicate)
			{
				duplicateComponentDetector.TryDetectIssue(currentLocation, currentAsset.Path, currentGameObject, componentType, componentName, orderIndex);
			}

			if (component is Terrain)
			{
				inconsistentTerrainDataDetector.ProcessTerrainComponent((Terrain)component, componentType, componentName, orderIndex);
			}
			else if (component is TerrainCollider)
			{
				inconsistentTerrainDataDetector.ProcessTerrainColliderComponent((TerrainCollider)component);
			}
			//Debug.Log("ProcessComponent: " + target.name + ":" + component);
		}

		/////////////////////////////////////////////////////////////////////////
		// Scriptable Objects Processing
		/////////////////////////////////////////////////////////////////////////

		public static void ProcessScriptableObject(AssetInfo asset, Object scriptableObject)
		{
			currentLocation = RecordLocation.Asset;

			if (missingComponentDetector.TryDetectScriptableObjectIssue(asset.Path,
				scriptableObject))
			{
				return;
			}

			var shouldTraverseProperties = missingReferenceDetector.Enabled;
			if (shouldTraverseProperties)
			{
				var initialInfo = new SerializedObjectTraverseInfo(scriptableObject);
				CSTraverseTools.TraverseObjectProperties(initialInfo, (info, property) =>
				{
					if (property.type == "UnityEvent")
					{
						missingReferenceDetector.TryDetectScriptableObjectUnityEventIssue(asset.Path,
							info.TraverseTarget.GetType().Name, property);

						info.skipCurrentTree = true;
						return;
					}

					missingReferenceDetector.TryDetectScriptableObjectIssue(asset.Path,
						info.TraverseTarget.GetType().Name, property);
				});
			}

			currentLocation = RecordLocation.Unknown;
		}

		/////////////////////////////////////////////////////////////////////////
		// Settings Assets Processing
		/////////////////////////////////////////////////////////////////////////

		public static void ProcessSettingsAsset(AssetInfo asset)
		{
			currentLocation = RecordLocation.Asset;

			var kind = asset.SettingsKind;

			missingReferenceDetector.TryDetectIssuesInSettingsAsset(asset, kind);

			if (kind == AssetSettingsKind.TagManager)
			{
				duplicateLayersDetector.TryDetectIssue();
			}

			currentLocation = RecordLocation.Unknown;
		}

		/*private static void OLD(ref List<IssueRecord> issues, GameObject go, string assetPath = null)
		{
			// ----------------------------------------------------------------------------
			// checking all components for ignores
			// ----------------------------------------------------------------------------

			var skipEmptyMeshFilter = false;
			var skipEmptyAudioSource = false;

			var allComponents = go.GetComponents<Component>();
			var allComponentsCount = allComponents.Length;

			var components = new List<Component>(allComponentsCount);
			var componentsTypes = new List<Type>(allComponentsCount);
			var componentsNames = new List<string>(allComponentsCount);
			var componentsNamespaces = new List<string>(allComponentsCount);

			var componentsCount = 0;
			var missingComponentsCount = 0;

			for (var i = 0; i < allComponentsCount; i++)
			{
				var component = allComponents[i];

				if (component == null)
				{
					missingComponentsCount++;
					continue;
				}

				var componentType = component.GetType();
				var componentName = componentType.Name;
				var componentFullName = componentType.FullName;
				var componentNamespace = componentType.Namespace;

				if (!componentType.IsSubclassOf(CSReflectionTools.componentType))
				{
					Debug.LogWarning(Maintainer.LogPrefix + "This object is pretend to be a Component, but is not a subclass of the Component:\n" +
									 "Name: " + componentName + "\n" +
									 "Type: " + componentType + "\n" +
									 "Namespace: " + componentNamespace
					);
					continue;
				}

				//
				//  checking object for the components which may affect 
				//  other components and produce false positives 
				//

				// allowing empty mesh filters for the objects with attached TextMeshPro and 2D Toolkit components.
				if (!skipEmptyMeshFilter)
				{
					skipEmptyMeshFilter = (componentFullName == "TMPro.TextMeshPro") || componentName.StartsWith("tk2d");
				}

				// allowing empty AudioSources for the objects with attached standard FirstPersonController.
				if (!skipEmptyAudioSource)
				{
					skipEmptyAudioSource = componentFullName == "UnityStandardAssets.Characters.FirstPerson.FirstPersonController";
				}

				// skipping disabled components
				if (!MaintainerSettings.Issues.touchDisabledComponents)
				{
					if (EditorUtility.GetObjectEnabled(component) == 0) continue;
				}

				// skipping ignored components

				components.Add(component);
				componentsTypes.Add(componentType);
				componentsNames.Add(componentName);
				componentsNamespaces.Add(componentNamespace);
				componentsCount++;
			}

			if (missingComponentsCount > 0 && MaintainerSettings.Issues.missingComponents)
			{
				var record = GameObjectIssueRecord.Create(IssueKind.MissingComponent, currentLocation, assetPath, go, null, null, -1);
				record.headerFormatArgument = missingComponentsCount;
				issues.Add(record);
			}

			// ----------------------------------------------------------------------------
			// looking for component-level issues
			// ----------------------------------------------------------------------------

			for (var i = 0; i < componentsCount; i++)
			{
				var component = components[i];
				var componentType = componentsTypes[i];
				var componentName = componentsNames[i];
				//string componentFullName = componentsFullNames[i];
				var componentNamespace = componentsNamespaces[i];

				if (component is Transform)
				{
					if (MaintainerSettings.Issues.hugePositions)
					{
						
					}
					continue;
				}



				// ----------------------------------------------------------------------------
				// looping through the component's SerializedProperties via SerializedObject
				// ----------------------------------------------------------------------------

				var so = new SerializedObject(component);
				var sp = so.GetIterator();
				var arrayLength = 0;

				while (sp.NextVisible(true))
				{
					var fullPropertyPath = sp.propertyPath;

					if (sp.isArray)
					{
						arrayLength = sp.arraySize;
					}

					var isArrayItem = fullPropertyPath.EndsWith("]", StringComparison.Ordinal);

					if (MaintainerSettings.Issues.missingReferences)
					{
						if (sp.propertyType == SerializedPropertyType.ObjectReference)
						{
							if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0)
							{
								var propertyPath = isArrayItem ? CSObjectTools.RemoveArrayStuffFromProperty(fullPropertyPath) : sp.name;
								var record = GameObjectIssueRecord.Create(IssueKind.MissingReference, currentLocation, assetPath, go, componentType, componentName, i, propertyPath);
								record.propertyPath = sp.propertyPath;
								issues.Add(record);
							}
						}
					}
				}
			}
		}*/
	}
}
