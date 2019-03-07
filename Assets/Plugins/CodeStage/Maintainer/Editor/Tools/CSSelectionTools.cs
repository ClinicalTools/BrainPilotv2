#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Tools
{
	using System;
	using TreeEditor;
	using UnityEditor;
	using UnityEngine;
	using Object = UnityEngine.Object;

#if UNITY_2018_3_OR_NEWER
	using UnityEditor.Experimental.SceneManagement;
	using UnityEditor.SceneManagement;
#endif

	public static class CSSelectionTools
	{
		public static bool RevealAndSelectFileAsset(string assetPath)
		{
			var instanceId = CSAssetTools.GetMainAssetInstanceID(assetPath);
			if (AssetDatabase.Contains(instanceId))
			{
				Selection.activeInstanceID = instanceId;
				return true;
			}

			return false;
		}

		public static bool RevealAndSelectSubAsset(string assetPath, string name, long objectId)
		{
			var targetAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
			if (targetAssets == null || targetAssets.Length == 0) return false;

			foreach (var targetAsset in targetAssets)
			{
				if (!AssetDatabase.IsSubAsset(targetAsset)) continue;
				if (targetAsset is GameObject || targetAsset is Component) continue;
				if (!string.Equals(targetAsset.name, name, StringComparison.OrdinalIgnoreCase)) continue;

				var assetId = CSObjectTools.GetUniqueObjectId(targetAsset);
				if (assetId != objectId) continue;

				Selection.activeInstanceID = targetAsset.GetInstanceID();
				return true;
			}

			return false;
		}

		public static bool RevealAndSelectGameObject(string enclosingAssetPath, string transformPath, long objectId, long componentId)
		{
			var enclosingAssetType = AssetDatabase.GetMainAssetTypeAtPath(enclosingAssetPath);
			return enclosingAssetType == typeof(SceneAsset) ? 
				RevealAndSelectGameObjectInScene(enclosingAssetPath, transformPath, objectId, componentId) : 
				RevealAndSelectGameObjectInPrefab(enclosingAssetPath, transformPath, objectId, componentId);
		}

		public static CSSceneTools.OpenSceneResult OpenSceneForReveal(string path)
		{
			var result = new CSSceneTools.OpenSceneResult();

			if (CSEditorTools.lastRevealSceneOpenResult != null)
			{
				if (CSEditorTools.lastRevealSceneOpenResult.scene.isDirty)
				{
					if (!CSSceneTools.SaveDirtyScenesWithPrompt(
						new[] { CSEditorTools.lastRevealSceneOpenResult.scene }))
					{
						return result;
					}
				}
			}

			result = CSSceneTools.OpenScene(path);
			if (result.success)
			{
				CSSceneTools.CloseUntitledSceneIfNotDirty();

				if (CSEditorTools.lastRevealSceneOpenResult != null)
				{
					CSSceneTools.CloseOpenedSceneIfNeeded(CSEditorTools.lastRevealSceneOpenResult, path, true);
				}

				CSEditorTools.lastRevealSceneOpenResult = result;
			}

			return result;
		}

		public static bool TryFoldAllComponentsExceptId(long componentId)
		{
			var tracker = CSEditorTools.GetActiveEditorTrackerForSelectedObject();
			if (tracker == null)
			{
				Debug.LogError(Maintainer.ConstructError("Can't get active tracker."));
				return false;
			}

			tracker.RebuildIfNecessary();

			var editors = tracker.activeEditors;
			if (editors.Length > 1)
			{
				var targetFound = false;
				var skipCount = 0;

				for (var i = 0; i < editors.Length; i++)
				{
					var editor = editors[i];
					var editorTargetType = editor.target.GetType();
					if (editorTargetType == CSReflectionTools.assetImporterType ||
					    editorTargetType == CSReflectionTools.gameObjectType)
					{
						skipCount++;
						continue;
					}

					if (i - skipCount == componentId)
					{
						targetFound = true;

						/* known corner cases when editor can't be set to visible via tracker */

						if (editor.serializedObject.targetObject is ParticleSystemRenderer)
						{
							var renderer = (ParticleSystemRenderer)editor.serializedObject.targetObject;
							var ps = renderer.GetComponent<ParticleSystem>();
							componentId = CSObjectTools.GetComponentIndex(ps);
						}

						break;
					}
				}

				if (!targetFound)
				{
					return false;
				}

#if UNITY_2018_3_OR_NEWER
				for (var i = 1; i < editors.Length; i++)
#else
				for (var i = 0; i < editors.Length; i++)
#endif
				{
					tracker.SetVisible(i, i - skipCount != componentId ? 0 : 1);
				}

				var inspectorWindow2 = CSEditorTools.GetInspectorWindow();
				if (inspectorWindow2 != null)
				{
					inspectorWindow2.Repaint();
				}

				// workaround for bug when tracker selection gets reset after scene open
				// (e.g. revealing TMP component in new scene)
				EditorApplication.delayCall += () =>
				{
					EditorApplication.delayCall += () =>
					{
						try
						{
							for (var i = 0; i < editors.Length; i++)
							{
								tracker.SetVisible(i, i - skipCount != componentId ? 0 : 1);
							}

							var inspectorWindow = CSEditorTools.GetInspectorWindow();
							if (inspectorWindow != null)
							{
								inspectorWindow.Repaint();
							}
						}
						catch (Exception)
						{
							// ignored
						}
					};
				};
			}

			return true;
		}

		private static bool RevealAndSelectGameObjectInScene(string enclosingAssetPath, string transformPath, long objectId, long componentId)
		{
			var openResult = OpenSceneForReveal(enclosingAssetPath);
			if (!openResult.success) return false;

			var target = CSObjectTools.FindGameObjectInScene(openResult.scene, objectId, transformPath);

			if (target == null)
			{
				Debug.LogError(Maintainer.ConstructError("Couldn't find target Game Object " + transformPath + " at " + enclosingAssetPath + " with ObjectID " + objectId + "!"));
				return false;
			}

			// workaround for a bug when Unity doesn't expand hierarchy in scene
			EditorApplication.delayCall += () =>
			{
				EditorGUIUtility.PingObject(target);
			};

			CSObjectTools.SelectGameObject(target, true);

			var enclosingAssetInstanceId = CSAssetTools.GetMainAssetInstanceID(enclosingAssetPath);
			EditorApplication.delayCall += () =>
			{
				EditorGUIUtility.PingObject(enclosingAssetInstanceId);
			};

			if (componentId != -1)
			{
				return TryFoldAllComponentsExceptId(componentId);
			}

			return true;
		}

#if UNITY_2018_3_OR_NEWER
		private static bool RevealAndSelectGameObjectInPrefab(string enclosingAssetPath, string transformPath, long objectId, long componentId)
		{
			/*Debug.Log("LOOKING FOR objectId " + objectId);
			Debug.Log("enclosingAssetPath " + enclosingAssetPath);*/

			var targetAsset = AssetDatabase.LoadMainAssetAtPath(enclosingAssetPath) as GameObject;
			var prefabType = PrefabUtility.GetPrefabAssetType(targetAsset);

			GameObject target;

			if (prefabType == PrefabAssetType.Model)
			{
				target = targetAsset;
			}
			else
			{
				if (!AssetDatabase.OpenAsset(targetAsset))
				{
					Debug.LogError(Maintainer.ConstructError("Couldn't open prefab at " + enclosingAssetPath + "!"));
					return false;
				}

				var stage = PrefabStageUtility.GetCurrentPrefabStage();
				if (stage == null)
				{
					Debug.LogError(Maintainer.ConstructError("Couldn't get prefab stage for prefab at " + enclosingAssetPath + "!"));
					return false;
				}

				target = stage.prefabContentsRoot;
			}

			if (target == null)
			{
				Debug.LogError(Maintainer.ConstructError("Couldn't find target Game Object " + transformPath + " at " + enclosingAssetPath + " with ObjectID " + objectId + "!"));
				return false;
			}

			target = CSObjectTools.FindChildGameObjectRecursive(target.transform, objectId, target.transform.name, transformPath);

			EditorApplication.delayCall += () =>
			{
				CSObjectTools.SelectGameObject(target, false);
				EditorGUIUtility.PingObject(targetAsset);

				if (componentId != -1)
				{
					EditorApplication.delayCall += () =>
					{
						TryFoldAllComponentsExceptId(componentId);
					};
				}
			};

			return true;
		}
#else
		private static bool RevealAndSelectGameObjectInPrefab(string enclosingAssetPath, string transformPath, long objectId, long componentId)
		{
			var targetAsset = AssetDatabase.LoadMainAssetAtPath(enclosingAssetPath) as GameObject;
			if (targetAsset == null) return false;

			Object target = CSObjectTools.FindChildGameObjectRecursive(targetAsset.transform, objectId, targetAsset.transform.name, transformPath);

			// in some cases, prefabs can have nested non-GameObject items
			if (target == null)
			{
				var allObjectsInPrefab = AssetDatabase.LoadAllAssetsAtPath(enclosingAssetPath);

				foreach (var objectOnPrefab in allObjectsInPrefab)
				{
					if (objectOnPrefab is BillboardAsset || objectOnPrefab is TreeData)
					{
						var objectOnPrefabId = CSObjectTools.GetUniqueObjectId(objectOnPrefab);
						if (objectOnPrefabId == objectId)
						{
							target = objectOnPrefab;
						}
					}
				}
			}

			if (target == null)
			{
				Debug.LogError(Maintainer.ConstructError("Couldn't find target Game Object " + transformPath + " at " + enclosingAssetPath + " with ObjectID " + objectId + "!"));
				return false;
			}

			if (target is GameObject)
			{
				CSObjectTools.SelectGameObject((GameObject)target, false);
			}
			else
			{
				Selection.activeObject = target;
			}

			if (transformPath.Split('/').Length > 2)
			{
				EditorApplication.delayCall += () =>
				{
					EditorGUIUtility.PingObject(targetAsset);
				};
			}
			
			if (componentId != -1)
			{
				return TryFoldAllComponentsExceptId(componentId);
			}

			return true;
		}
#endif
	}
}