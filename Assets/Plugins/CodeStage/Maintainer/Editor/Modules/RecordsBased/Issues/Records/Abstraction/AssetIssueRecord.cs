#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues
{
	using System;

	[Serializable]
	public abstract class AssetIssueRecord : IssueRecord
	{
		public string Path { get; private set; }

		protected AssetIssueRecord(IssueKind kind, RecordLocation location, string path) : base(kind, location)
		{
			Path = path;
		}
	}
}
