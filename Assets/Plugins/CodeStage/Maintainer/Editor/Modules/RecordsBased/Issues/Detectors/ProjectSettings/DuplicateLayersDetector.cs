#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using System.Collections.Generic;
	using Settings;

	internal class DuplicateLayersDetector : IssueDetectorBase
	{
		private readonly bool enabled = MaintainerSettings.Issues.duplicateLayers;

		public DuplicateLayersDetector(List<IssueRecord> issues) : base(issues) { }

		public void TryDetectIssue()
		{
			if (!enabled) return;

			var issue = SettingsChecker.CheckTagsAndLayers();
			if (issue != null)
			{
				issues.Add(issue);
			}
		}
	}
}