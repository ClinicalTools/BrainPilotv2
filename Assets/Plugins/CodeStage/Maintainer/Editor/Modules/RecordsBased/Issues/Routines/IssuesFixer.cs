#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues
{
	using System;
	using System.Linq;
	using Detectors;
	using Settings;
	using UnityEditor;
	using UnityEngine;

	using Tools;
	using Object = UnityEngine.Object;

	internal class IssuesFixer
	{
		public static void FixRecords(IssueRecord[] results, bool showProgress = true)
		{
			var i = 0;
			var count = results.Length;

			var sortedRecords = results.OrderBy(RecordsSortings.issueRecordByPath).ToArray();
			var updateStep = Math.Max(count / MaintainerSettings.UpdateProgressStep, 1);

			for (var k = 0; k < count; k++)
			{
				var item = sortedRecords[k];

				if (showProgress)
				{
					if (k % updateStep == 0)
					{
						if (IssuesFinder.ShowProgressBar(1, 1, i, count, "Resolving selected issues..."))
						{
							IssuesFinder.operationCanceled = true;
							break;
						}
					}
				}

				if (item.selected && item.CanBeFixed())
				{
					if (item.Location == RecordLocation.Scene)
					{
						var assetIssue = item as AssetIssueRecord;
						if (assetIssue != null)
						{
							var newOpenSceneResult = CSSceneTools.OpenScene(assetIssue.Path);
							if (!newOpenSceneResult.success)
							{
								continue;
							}

							if (newOpenSceneResult.sceneWasLoaded)
							{
								if (IssuesFinder.lastOpenSceneResult != null)
								{
									CSSceneTools.SaveScene(IssuesFinder.lastOpenSceneResult.scene);
									CSSceneTools.CloseOpenedSceneIfNeeded(IssuesFinder.lastOpenSceneResult);
								}
							}

							if (IssuesFinder.lastOpenSceneResult == null || IssuesFinder.lastOpenSceneResult.scene != newOpenSceneResult.scene)
							{
								IssuesFinder.lastOpenSceneResult = newOpenSceneResult;
							}
						}
					}

					item.Fix(true);
					i++;
				}
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public static bool FixObjectIssue(GameObjectIssueRecord issue, Object obj, Component component, IssueKind type)
		{
			var result = false;

			if (type == IssueKind.MissingComponent)
			{
				var hasIssue = GameObjectHasMissingComponent(obj as GameObject);

				if (hasIssue)
				{
					FixMissingComponents(issue, obj as GameObject, false);
					result = !GameObjectHasMissingComponent(obj as GameObject);
				}
				else
				{
					result = true;
				}
			}
			else if (type == IssueKind.MissingReference)
			{
				if (component != null)
				{
					result = FixMissingReference(component, issue.propertyPath, issue.Location);
				}
				else
				{
					result = FixMissingReference(obj, issue.propertyPath, issue.Location);
				}
			}

			return result;
		}

		#region missing component

		// ----------------------------------------------------------------------------
		// fix missing component
		// ----------------------------------------------------------------------------

		private static bool FixMissingComponents(GameObjectIssueRecord issue, GameObject go, bool alternative)
		{
			if (!alternative)
			{
				CSObjectTools.SelectGameObject(go, issue.Location == RecordLocation.Scene);
			}

			var tracker = CSEditorTools.GetActiveEditorTrackerForSelectedObject();
			if (tracker == null)
			{
				Debug.LogError(Maintainer.ConstructError("Can't get active tracker."));
				return false;
			}
			tracker.RebuildIfNecessary();

			var touched = false;
			var activeEditors = tracker.activeEditors;
			for (var i = activeEditors.Length - 1; i >= 0; i--)
			{
				var editor = activeEditors[i];
				if (editor.serializedObject.targetObject == null)
				{
					Object.DestroyImmediate(editor.target, true);
					touched = true;
				}
			}

			if (alternative)
			{
				return touched;
			}

			if (!touched)
			{
				// missing script could be hidden with hide flags, so let's try select it directly and repeat
				var serializedObject = new SerializedObject(go);
				var componentsArray = serializedObject.FindProperty("m_Component");
				if (componentsArray != null)
				{
					for (var i = componentsArray.arraySize - 1; i >= 0; i--)
					{
						var componentPair = componentsArray.GetArrayElementAtIndex(i);
						var nestedComponent = componentPair.FindPropertyRelative("component");
						if (nestedComponent != null)
						{
							if (MissingReferenceDetector.IsPropertyHasMissingReference(nestedComponent))
							{
								var instanceId = nestedComponent.objectReferenceInstanceIDValue;
								if (instanceId == 0)
								{
									var fileId = nestedComponent.FindPropertyRelative("m_FileID");
									if (fileId != null)
									{
										instanceId = fileId.intValue;
									}
								}

								Selection.instanceIDs = new []{ instanceId };
								touched |= FixMissingComponents(issue, go, true);
							} 
						}
						else
						{
							Debug.LogError(Maintainer.LogPrefix + "Couldn't find component in component pair!");
							break;
						}
					}

					if (touched)
					{
						CSObjectTools.SelectGameObject(go, issue.Location == RecordLocation.Scene);
					}
				}
				else
				{
					Debug.LogError(Maintainer.LogPrefix + "Couldn't find components array!");
				}
			}

			if (touched)
			{
				if (issue.Location == RecordLocation.Scene)
				{
					CSSceneTools.MarkSceneDirty();
				}
				else
				{
					EditorUtility.SetDirty(go);
				}
			}

			return touched;
		}

		private static bool GameObjectHasMissingComponent(GameObject go)
		{
			var hasMissingComponent = false;
			var components = go.GetComponents<Component>();
			foreach (var c in components)
			{
				if (c == null)
				{
					hasMissingComponent = true;
					break;
				}
			}
			return hasMissingComponent;
		}
#endregion

		#region missing reference
		// ----------------------------------------------------------------------------
		// fix missing reference
		// ----------------------------------------------------------------------------

		public static bool FixMissingReference(Object unityObject, string propertyPath, RecordLocation location)
		{
			var so = new SerializedObject(unityObject);
			var sp = so.FindProperty(propertyPath);

			if (MissingReferenceDetector.IsPropertyHasMissingReference(sp))
			{
				sp.objectReferenceInstanceIDValue = 0;

				var fileId = sp.FindPropertyRelative("m_FileID");
				if (fileId != null)
				{
					fileId.intValue = 0;
				}

				// fixes dirty scene flag after batch issues fix
				// due to the additional undo action
				so.ApplyModifiedPropertiesWithoutUndo();

				if (location == RecordLocation.Scene)
				{
					CSSceneTools.MarkSceneDirty();
				}
				else
				{
					if (unityObject != null) EditorUtility.SetDirty(unityObject);
				}
			}

			return true;
		}
		#endregion
	}
}