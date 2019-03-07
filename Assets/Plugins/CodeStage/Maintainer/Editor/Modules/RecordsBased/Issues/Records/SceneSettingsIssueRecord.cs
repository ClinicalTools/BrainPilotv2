#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues
{
	using System;
	using System.Text;
	using Core;
	using Tools;
	using UI;
	using UnityEditor;
	using UnityEngine;
	using Object = UnityEngine.Object;

	public enum SceneSettingsKind
	{
		LightmapSettings = 5,
		RenderSettings = 7
	}

	[Serializable]
	public class SceneSettingsIssueRecord : AssetIssueRecord, IShowableRecord
	{
		public string PropertyPath { get; private set; }
		public SceneSettingsKind SettingsKind { get; private set; }

		public void Show()
		{
			var openResult = CSSelectionTools.OpenSceneForReveal(Path);
			if (openResult.success)
			{
				var instanceId = CSAssetTools.GetMainAssetInstanceID(Path);
				EditorGUIUtility.PingObject(instanceId);
			}

			switch (SettingsKind)
			{
				case SceneSettingsKind.RenderSettings:
				case SceneSettingsKind.LightmapSettings:
					CSMenuTools.ShowSceneSettingsLighting();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		internal static SceneSettingsIssueRecord Create(SceneSettingsKind settingsKind, IssueKind issueKind, string path, string propertyPath)
		{
			return new SceneSettingsIssueRecord(settingsKind, issueKind, path, propertyPath);
		}

		internal override bool MatchesFilter(FilterItem newFilter)
		{
			return false;
		}

		internal override bool CanBeFixed()
		{
			return Kind == IssueKind.MissingReference;
		}

		protected SceneSettingsIssueRecord(SceneSettingsKind settingsKind, IssueKind issueKind, string path, string propertyPath) : base(issueKind, RecordLocation.Scene, path)
		{
			SettingsKind = settingsKind;
			PropertyPath = propertyPath;
		}

		protected override void ConstructBody(StringBuilder text)
		{
			text.AppendLine("<b>Scene: </b>" + CSPathTools.NicifyAssetPath(Path, true));
			text.Append("<b>Settings: </b>" + SettingsKind);
			if (!string.IsNullOrEmpty(PropertyPath))
			{
				var propertyName = CSObjectTools.GetNicePropertyPath(PropertyPath);
				text.Append("\n<b>Property:</b> ").Append(propertyName);
			}
		}

		protected override bool PerformFix(bool batchMode)
		{
			CSSceneTools.OpenSceneResult openSceneResult = null;

			if (!batchMode)
			{
				openSceneResult = CSSceneTools.OpenScene(Path);
				if (!openSceneResult.success)
				{
					return false;
				}
			}

			var settingsObject = GetSettingsObjectWithThisIssue();
			if (settingsObject == null)
			{
				if (batchMode)
				{
					Debug.LogWarning(Maintainer.LogPrefix + "Couldn't find " + SettingsKind + " object for issue:\n" + this);
				}
				else
				{
					MaintainerWindow.ShowNotification("Couldn't find " + SettingsKind + " object at " + Path);
				}
				return false;
			}

			var fixResult = IssuesFixer.FixMissingReference(settingsObject, PropertyPath, RecordLocation.Scene);

			if (!batchMode)
			{
				CSSceneTools.SaveScene(openSceneResult.scene);
				CSSceneTools.CloseOpenedSceneIfNeeded(openSceneResult);
			}

			return fixResult;
		}

		private Object GetSettingsObjectWithThisIssue()
		{
			Object result;

			switch (SettingsKind)
			{
				case SceneSettingsKind.LightmapSettings:
					result = CSSettingsTools.GetInSceneLightmapSettings();
					break;
				case SceneSettingsKind.RenderSettings:
					result = CSSettingsTools.GetInSceneRenderSettings();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return result;
		}
	}
}