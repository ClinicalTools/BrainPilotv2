#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Settings
{
	using System;

	using Core;

	[Serializable]
	public class ReferencesFinderSettings
	{
		public FilterItem[] pathIgnoresFilters = new FilterItem[0];
	}
}