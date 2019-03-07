#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Cleaner
{
	using System;
	using System.Text;
	using Core;

	[Serializable]
	public class CleanerErrorRecord : CleanerRecord
	{
		public string errorText;

		protected CleanerErrorRecord(string errorText) : base(RecordType.Error, RecordLocation.Unknown)
		{
			this.errorText = errorText;
		}

		internal static CleanerErrorRecord Create(string text)
		{
			return new CleanerErrorRecord(text);
		}

		internal override bool MatchesFilter(FilterItem newFilter)
		{
			return false;
		}

		protected override void ConstructCompactLine(StringBuilder text)
		{
			text.Append(errorText);
		}

		protected override void ConstructBody(StringBuilder text)
		{
			text.Append(errorText);
		}

		protected override bool PerformClean()
		{
			return false;
		}
	}
}