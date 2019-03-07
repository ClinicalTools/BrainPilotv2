#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Tools
{
	using System;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.SceneManagement;
	using Object = UnityEngine.Object;

	internal class TraverseStats
	{
		public long gameObjectsTraversed;
		public long componentsTraversed;
		public long propertiesTraversed;

		public void Clear()
		{
			gameObjectsTraversed = componentsTraversed = propertiesTraversed = 0;
		}
	}

	internal class ObjectTraverseInfo
	{
		public bool SkipCleanPrefabInstances { get; private set; }
		public int TotalRoots { get; private set; }

		public GameObject current;
		public bool inPrefabInstance;
		public bool inMissingPrefabInstance;
		public int[] dirtyComponents;
		public int rootIndex;

		public bool skipCurrentTree;

		public ObjectTraverseInfo(bool skipCleanPrefabInstances, int totalRoots)
		{
			SkipCleanPrefabInstances = skipCleanPrefabInstances;
			TotalRoots = totalRoots;
		}
	}

	internal class SerializedObjectTraverseInfo
	{
		public Object TraverseTarget { get; private set; }
		public bool OnlyVisibleProperties { get; private set; }

		public bool skipCurrentTree;

		public SerializedObjectTraverseInfo(Object traverseTarget, bool onlyVisibleProperties = true)
		{
			TraverseTarget = traverseTarget;
			OnlyVisibleProperties = onlyVisibleProperties;
		}
	}

	internal class CSTraverseTools
	{


		internal delegate bool GameObjectTraverseCallback(ObjectTraverseInfo traverseInfo);
		internal delegate void ComponentTraverseCallback(ObjectTraverseInfo traverseInfo, Component component, int orderIndex);
		internal delegate void SerializableObjectTraverseCallback(SerializedObjectTraverseInfo traverseInfo, SerializedProperty currentProperty);

		private static readonly TraverseStats stats = new TraverseStats();

		public static void ClearStats()
		{
			stats.Clear();
		}

		public static TraverseStats GetStats()
		{
			return stats;
		}

		public static bool TraverseSceneGameObjects(Scene scene, bool skipCleanPrefabInstances, GameObjectTraverseCallback callback)
		{
			var rootObjects = scene.GetRootGameObjects();
			var rootObjectsCount = rootObjects.Length;

			var objectTraverseInfo = new ObjectTraverseInfo(skipCleanPrefabInstances, rootObjectsCount);

			for (var i = 0; i < rootObjectsCount; i++)
			{
				var rootObject = rootObjects[i];

				objectTraverseInfo.current = rootObject;
				objectTraverseInfo.rootIndex = i;

				if (!TraverseGameObjectTree(objectTraverseInfo, callback))
				{
					return false;
				}
			}

			return true;
		}

		public static bool TraversePrefabGameObjects(GameObject prefabAsset, bool enablePrefabChecks, GameObjectTraverseCallback callback)
		{
			var objectTraverseInfo = new ObjectTraverseInfo(enablePrefabChecks, 1)
			{
				current = prefabAsset,
				inPrefabInstance = false,
				dirtyComponents = null,
				rootIndex = 0,
			};

			return TraverseGameObjectTree(objectTraverseInfo, callback);
		}

		public static bool TraverseGameObjectTree(ObjectTraverseInfo traverseInfo, GameObjectTraverseCallback callback)
		{
			stats.gameObjectsTraversed++;

			var prefabInstanceRoot = false;
			if (!traverseInfo.inPrefabInstance)
			{
				
				//Debug.Log("IsPartOfPrefabInstance " + PrefabUtility.IsPartOfPrefabInstance(componentOrGameObject));
				//Debug.Log(traverseInfo.current.name);
				traverseInfo.inPrefabInstance = CSPrefabTools.IsInstance(traverseInfo.current);
				if (traverseInfo.inPrefabInstance)
				{
					if (!CSPrefabTools.IsMissingPrefabInstance(traverseInfo.current))
					{
						if (traverseInfo.SkipCleanPrefabInstances)
						{
							if (CSPrefabTools.IsWholeInstanceHasAnyOverrides(traverseInfo.current))
							{
								CSPrefabTools.GetOverridenObjectsFromWholePrefabInstance(traverseInfo.current, out traverseInfo.dirtyComponents);
							}
						}
					}
					else
					{
						traverseInfo.inMissingPrefabInstance = true;
					}

					prefabInstanceRoot = true;
				}
			}

			if (!callback.Invoke(traverseInfo))
			{
				return false;
			}

			if (traverseInfo.skipCurrentTree)
			{
				if (prefabInstanceRoot)
				{
					traverseInfo.dirtyComponents = null;
					traverseInfo.inPrefabInstance = false;
					traverseInfo.inMissingPrefabInstance = false;
					traverseInfo.skipCurrentTree = false;
				}

				return true;
			}

			var transform = traverseInfo.current.transform;
			var childrenCount = transform.childCount;

			for (var i = 0; i < childrenCount; i++)
			{
				var child = transform.GetChild(i);
				traverseInfo.current = child.gameObject;
				if (!TraverseGameObjectTree(traverseInfo, callback))
				{
					return false;
				}
			}

			if (prefabInstanceRoot)
			{
				traverseInfo.dirtyComponents = null;
				traverseInfo.inPrefabInstance = false;
				traverseInfo.inMissingPrefabInstance = false;
				traverseInfo.skipCurrentTree = false;
			}

			return true;
		}

		public static void TraverseGameObjectComponents(ObjectTraverseInfo traverseInfo, ComponentTraverseCallback callback)
		{
			var components = traverseInfo.current.GetComponents<Component>();
			var checkingPrefabInstanceObject = false;
			var checkingAddedToInstanceObject = false;

			if (traverseInfo.SkipCleanPrefabInstances)
			{
#if UNITY_2018_3_OR_NEWER
				checkingPrefabInstanceObject = traverseInfo.inPrefabInstance && !traverseInfo.inMissingPrefabInstance;
				if (checkingPrefabInstanceObject)
				{
					checkingAddedToInstanceObject = PrefabUtility.IsAddedGameObjectOverride(traverseInfo.current);
				}
#else
				checkingPrefabInstanceObject = CSPrefabTools.IsInstance(traverseInfo.current);
#endif
			}

			var hasDirtyComponents = traverseInfo.dirtyComponents != null;

			for (var i = 0; i < components.Length; i++)
			{
				var component = components[i];
				if (component == null)
				{
					// to register missing script
					callback(traverseInfo, null, i);
					continue;
				}

				// transforms are checked at the GameObject level
				if (component is Transform) continue;

				if (!checkingPrefabInstanceObject)
				{
					stats.componentsTraversed++;
					callback(traverseInfo, component, i);
				}
				else
				{
					var hasOverridenProperties = checkingAddedToInstanceObject;
					if (!hasOverridenProperties && hasDirtyComponents)
					{
						if (Array.IndexOf(traverseInfo.dirtyComponents, component.GetInstanceID()) != -1)
						{
							hasOverridenProperties = true;
						}
					}

					if (hasOverridenProperties)
					{
						stats.componentsTraversed++;
						callback(traverseInfo, component, i);
					}
				}
			}
		}

		public static void TraverseObjectProperties(SerializedObjectTraverseInfo traverseInfo, SerializableObjectTraverseCallback callback)
		{
			var so = new SerializedObject(traverseInfo.TraverseTarget);
			var iterator = so.GetIterator();

			if (traverseInfo.OnlyVisibleProperties)
			{
				while (iterator.NextVisible(!traverseInfo.skipCurrentTree))
				{
					if (traverseInfo.skipCurrentTree) traverseInfo.skipCurrentTree = false;
					stats.propertiesTraversed++;
					callback(traverseInfo, iterator);
				}
			}
			else
			{
				while (iterator.Next(!traverseInfo.skipCurrentTree))
				{
					if (!traverseInfo.skipCurrentTree) traverseInfo.skipCurrentTree = false;

					stats.propertiesTraversed++;
					callback(traverseInfo, iterator);
				}
			}
		}
	}
}