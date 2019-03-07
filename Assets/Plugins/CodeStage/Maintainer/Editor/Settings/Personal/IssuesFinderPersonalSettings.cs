#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Settings
{
	using System;

	using Cleaner;
	using UI;

	[Serializable]
	public class IssuesFinderPersonalSettings
	{
		public RecordsTabState tabState = new RecordsTabState();
		public int filtersTabIndex = 0;

		/* sorting */

		public IssuesSortingType sortingType = IssuesSortingType.BySeverity;
		public SortingDirection sortingDirection = SortingDirection.Ascending;
	}
}