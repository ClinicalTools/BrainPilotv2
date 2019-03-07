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

	[Serializable]
	public class SettingsIssueRecord : AssetIssueRecord, IShowableRecord
	{
		public string PropertyPath { get; private set; }
		public AssetSettingsKind SettingsKind { get; private set; }

		public void Show()
		{
			CSEditorTools.RevealInSettings(SettingsKind);
		}

		internal static SettingsIssueRecord Create(AssetSettingsKind settingsKind, IssueKind type, string body)
		{
			return new SettingsIssueRecord(settingsKind, type, body);
		}

		internal static SettingsIssueRecord Create(AssetSettingsKind settingsKind, IssueKind type, string path, string propertyPath)
		{
			return new SettingsIssueRecord(settingsKind, type, path, propertyPath);
		}

		internal override bool MatchesFilter(FilterItem newFilter)
		{
			return false;
		}

		internal override bool CanBeFixed()
		{
			return Kind == IssueKind.MissingReference;
		}

		protected SettingsIssueRecord(AssetSettingsKind settingsKind, IssueKind kind, string body):base(kind, RecordLocation.Asset, null)
		{
			SettingsKind = settingsKind;
			bodyExtra = body;
		}

		protected SettingsIssueRecord(AssetSettingsKind settingsKind, IssueKind kind, string path, string propertyPath) : base(kind, RecordLocation.Asset, path)
		{
			SettingsKind = settingsKind;
			PropertyPath = propertyPath;
		}

		protected override void ConstructBody(StringBuilder text)
		{
			text.Append("<b>Settings: </b>" + SettingsKind);
			if (!string.IsNullOrEmpty(PropertyPath))
			{
				var propertyName = CSObjectTools.GetNicePropertyPath(PropertyPath);
				text.Append("\n<b>Property:</b> ").Append(propertyName);
			}
		}

		protected override bool PerformFix(bool batchMode)
		{
			var assetObject = AssetDatabase.LoadMainAssetAtPath(Path);

			// workaround for Unity 5.6 issue: LoadMainAssetAtPath returns null for settings assets
			if (assetObject == null)
			{
				var allObjects = AssetDatabase.LoadAllAssetsAtPath(Path);
				if (allObjects != null && allObjects.Length > 0)
				{
					assetObject = allObjects[0];
				}
			}

			if (assetObject == null)
			{
				if (batchMode)
				{
					Debug.LogWarning(Maintainer.LogPrefix + "Couldn't find settings asset for issue:\n" + this);
				}
				else
				{
					MaintainerWindow.ShowNotification("Couldn't find settings asset at " + Path);
				}
				return false;
			}

			var fixResult = IssuesFixer.FixMissingReference(assetObject, PropertyPath, RecordLocation.Asset);
			return fixResult;
		}
	}
}