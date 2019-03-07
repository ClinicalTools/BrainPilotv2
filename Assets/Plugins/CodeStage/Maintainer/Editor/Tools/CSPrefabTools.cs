#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Tools
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	using Object = UnityEngine.Object;

	public class CSPrefabTools
	{
#if UNITY_2018_3_OR_NEWER
		public static PrefabInstanceStatus GetInstanceStatus(Object componentOrGameObject)
		{
			return PrefabUtility.GetPrefabInstanceStatus(componentOrGameObject);
		}
#else
		public static PrefabType GetInstanceStatus(Object componentOrGameObject)
		{
			return PrefabUtility.GetPrefabType(componentOrGameObject);
		}
#endif

#if UNITY_2018_3_OR_NEWER
		public static bool IsProperlyConnectedInstance(PrefabInstanceStatus status)
		{
			return status == PrefabInstanceStatus.Connected;
		}
#else
		public static bool IsProperlyConnectedInstance(PrefabType status)
		{
			return status != PrefabType.None && 
			       status != PrefabType.DisconnectedPrefabInstance &&
			       status != PrefabType.DisconnectedModelPrefabInstance &&
			       status != PrefabType.MissingPrefabInstance;
		}
#endif

#if UNITY_2018_3_OR_NEWER
		public static bool IsInstance(PrefabInstanceStatus status)
		{
			return status != PrefabInstanceStatus.NotAPrefab;
		}
#else
		public static bool IsInstance(PrefabType status)
		{
			return status != PrefabType.None;
		}
#endif

#if UNITY_2018_3_OR_NEWER
		public static bool IsMissingPrefabInstance(GameObject gameObject)
		{
			var status = PrefabUtility.GetPrefabInstanceStatus(gameObject);
			return status == PrefabInstanceStatus.MissingAsset || 
			       status == PrefabInstanceStatus.NotAPrefab && IsInstance(gameObject) && PrefabUtility.GetNearestPrefabInstanceRoot(gameObject) == null;
		}
#else
		public static bool IsMissingPrefabInstance(GameObject gameObject)
		{
			var type = PrefabUtility.GetPrefabType(gameObject);
			return type == PrefabType.MissingPrefabInstance;
		}
#endif

		public static bool IsInstance(Object componentOrGameObject)
		{
#if UNITY_2018_3_OR_NEWER
			return PrefabUtility.IsPartOfPrefabInstance(componentOrGameObject);
#else
			var type = PrefabUtility.GetPrefabType(componentOrGameObject);
			return type == PrefabType.PrefabInstance || type == PrefabType.ModelPrefabInstance || type == PrefabType.MissingPrefabInstance;
#endif
		}

		public static Object GetAssetSource(Object componentOrGameObject)
		{
#if UNITY_2018_3_OR_NEWER
			return PrefabUtility.GetCorrespondingObjectFromOriginalSource(componentOrGameObject);
#elif UNITY_2018_2_OR_NEWER
			return PrefabUtility.GetCorrespondingObjectFromSource(componentOrGameObject);
#else
			return PrefabUtility.GetPrefabParent(componentOrGameObject);
#endif
		}

		public static GameObject GetPrefabAssetRoot(string path)
		{
			return AssetDatabase.LoadMainAssetAtPath(path) as GameObject;
		}

		public static GameObject GetInstanceRoot(GameObject gameObject)
		{
#if UNITY_2018_3_OR_NEWER
			return PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
#else
			return PrefabUtility.FindRootGameObjectWithSameParentPrefab(gameObject);
#endif
		}

		public static bool IsInstanceRoot(GameObject gameObject)
		{
#if UNITY_2018_3_OR_NEWER
			return PrefabUtility.IsOutermostPrefabInstanceRoot(gameObject);
#else
			var instanceRoot = GetInstanceRoot(gameObject);
			return instanceRoot == gameObject;
#endif
		}

		public static bool IsWholeInstanceHasAnyOverrides(GameObject current)
		{
#if UNITY_2018_3_OR_NEWER
			return PrefabUtility.HasPrefabInstanceAnyOverrides(current, false);
#else
			return true;
#endif
		}

		public static void GetOverridenObjectsFromWholePrefabInstance(GameObject target, out int[] dirtyComponents)
		{
			var resultComponents = new HashSet<int>();

#if UNITY_2018_3_OR_NEWER
			var overrides = PrefabUtility.GetObjectOverrides(target, false);
			foreach (var objectOverride in overrides)
			{
				var component = objectOverride.instanceObject as Component;
				if (component != null)
				{
					resultComponents.Add(component.GetInstanceID());
				}
			}

			var addedComponents = PrefabUtility.GetAddedComponents(target);
			foreach (var addedComponent in addedComponents)
			{
				resultComponents.Add(addedComponent.instanceComponent.GetInstanceID());
			}
#else
			var modifications = PrefabUtility.GetPropertyModifications(target);
			if (modifications != null)
			{
				var modifiedComponents = new HashSet<Component>();

				foreach (var modification in modifications)
				{
					var component = modification.target as Component;
					if (component == null) continue;
					if (component is Transform) continue;

					modifiedComponents.Add(component);
				}

				var targetComponents = target.GetComponents<Component>();
				foreach (var targetComponent in targetComponents)
				{
					if (targetComponent == null) continue;
#if UNITY_2018_2_OR_NEWER
					var prefabComponent = PrefabUtility.GetCorrespondingObjectFromSource(targetComponent) as Component;
#else
					var prefabComponent = PrefabUtility.GetPrefabParent(targetComponent) as Component;
#endif
					if (modifiedComponents.Contains(prefabComponent))
					{
						resultComponents.Add(targetComponent.GetInstanceID());
					}
				}

				GetAddedComponentsFromInstanceTree(target, resultComponents);
			}
#endif
			dirtyComponents = resultComponents.Count == 0 ? null : resultComponents.ToArray();
		}

#if !UNITY_2018_3_OR_NEWER
		private static void GetAddedComponentsFromInstanceTree(GameObject parent, HashSet<int> results)
		{
			var prefabAssetSource = GetAssetSource(parent) as GameObject;
			if (prefabAssetSource != null)
			{
				var goComponents = parent.GetComponents<Component>();
				var prefabComponents = prefabAssetSource.GetComponents<Component>();
				var prefabTypes = new List<System.Type>(prefabComponents.Length);

				foreach (var prefabComponent in prefabComponents)
				{
					if (prefabComponent == null) continue;
					prefabTypes.Add(prefabComponent.GetType());
				}

				foreach (var goComponent in goComponents)
				{
					if (goComponent == null) continue;

					if (prefabTypes.IndexOf(goComponent.GetType()) == -1)
					{
						results.Add(goComponent.GetInstanceID());
					}
				}
			}

			var transform = parent.transform;
			var childrenCount = transform.childCount;

			for (var i = 0; i < childrenCount; i++)
			{
				var child = transform.GetChild(i);
				GetAddedComponentsFromInstanceTree(child.gameObject, results);
			}
		}
#endif
	}
}
