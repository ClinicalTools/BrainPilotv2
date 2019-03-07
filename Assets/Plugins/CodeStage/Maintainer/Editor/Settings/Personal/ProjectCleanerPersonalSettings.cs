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
	public class ProjectCleanerPersonalSettings
	{
		public RecordsTabState tabState;

		public int filtersTabIndex = 0;

		public bool firstTime = true;
		public bool trashBinWarningShown = false;
		public bool deletionPromptShown = false;

		/* sorting */

		public CleanerSortingType sortingType = CleanerSortingType.BySize;
		public SortingDirection sortingDirection = SortingDirection.Ascending;
	}
}