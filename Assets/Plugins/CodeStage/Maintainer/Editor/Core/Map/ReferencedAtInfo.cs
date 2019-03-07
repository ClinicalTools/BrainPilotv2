#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core
{
	using System;

	using UnityEditor;

	[Serializable]
	public class ReferencedAtInfo : ReferencingInfo
	{
		public ReferencingEntryData[] entries;

		public void AddNewEntry(ReferencingEntryData newEntry)
		{
			if (entries == null)
			{
				entries = new[] {newEntry};
			}
			else
			{
				ArrayUtility.Add(ref entries, newEntry);
			}
		}
	}
}