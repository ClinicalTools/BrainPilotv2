#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using System.Collections.Generic;
	using Settings;
	using Tools;
	using UnityEngine;

	internal class MissingPrefabDetector : IssueDetectorBase
	{
		private readonly bool enabled = MaintainerSettings.Issues.missingPrefabs;

		public MissingPrefabDetector(List<IssueRecord> issues) : base(issues) { }

		public bool TryDetectIssue(RecordLocation location, string assetPath, GameObject target)
		{
			if (!CSPrefabTools.IsMissingPrefabInstance(target)) return false;

			if (enabled) issues.Add(GameObjectIssueRecord.Create(IssueKind.MissingPrefab, location, assetPath, target));

			return true;
		}
	}
}