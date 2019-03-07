#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core
{
	using System;

	[Serializable]
	public enum FilterKind
	{
		Path,
		Directory,
		FileName,
		Extension,
		Type,

		NotSet = 1000,
	}

	[Serializable]
	public class FilterItem
	{
		public bool ignoreCase;
		public FilterKind kind;
		public string value;

		public static FilterItem Create(string value, FilterKind kind, bool ignoreCase = false)
		{
			var newFilter = new FilterItem
			{
				ignoreCase = ignoreCase,
				kind = kind,
				value = value
			};

			return newFilter;
		}
	}
}