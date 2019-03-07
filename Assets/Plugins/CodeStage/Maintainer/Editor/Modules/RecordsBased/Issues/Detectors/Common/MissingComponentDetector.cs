#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using System.Collections.Generic;
	using Settings;
	using UnityEngine;

	internal class MissingComponentDetector : IssueDetectorBase
	{
		private readonly bool enabled = MaintainerSettings.Issues.missingComponents;

		private int missingComponentsCount;

		public MissingComponentDetector(List<IssueRecord> issues) : base(issues) { }

		public void StartGameObject()
		{
			missingComponentsCount = 0;
		}

		public void TryDetectIssue(RecordLocation location, string assetPath, GameObject target)
		{
			if (missingComponentsCount <= 0 || !enabled) return;

			var record = GameObjectIssueRecord.Create(IssueKind.MissingComponent, location, assetPath, target, null, null, -1);
			record.headerFormatArgument = missingComponentsCount;
			issues.Add(record);
		}

		public bool TryDetectScriptableObjectIssue(string assetPath, Object target)
		{
			if (target != null) return false;

			if (enabled)
			{
				var record = ScriptableObjectIssueRecord.Create(IssueKind.MissingComponent, assetPath);
				issues.Add(record);
			}
			
			return true;
		}

		public bool CheckAndRecordNullComponent(Component component)
		{
			if (component != null) return false;

			missingComponentsCount++;
			return true;
		}
	}
}