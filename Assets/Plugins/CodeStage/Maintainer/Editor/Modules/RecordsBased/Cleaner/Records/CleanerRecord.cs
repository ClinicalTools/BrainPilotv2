#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Cleaner
{
	using System;
	using System.Text;

	[Serializable]
	public abstract class CleanerRecord : RecordBase
	{
		public RecordType type;
		public bool cleaned;

		internal bool Clean()
		{
			cleaned = PerformClean();
			return cleaned; 
		}

		// ----------------------------------------------------------------------------
		// base constructors
		// ----------------------------------------------------------------------------

		protected CleanerRecord(RecordType type, RecordLocation location):base(location)
		{
			this.type = type;
		}

		// ----------------------------------------------------------------------------
		// header generation
		// ----------------------------------------------------------------------------

		protected override void ConstructHeader(StringBuilder header)
		{
			switch (type)
			{
				case RecordType.EmptyFolder:
					header.Append("Empty Folder");
					break;
				case RecordType.UnreferencedAsset:
					header.Append("Unused ");
					break;
				case RecordType.Error:
					header.Append("Error!");
					break;
				case RecordType.Other:
					header.Append("Other");
					break;
				default:
					header.Append("Unknown issue!");
					break;
			}
		}

		protected abstract bool PerformClean();
	}
}