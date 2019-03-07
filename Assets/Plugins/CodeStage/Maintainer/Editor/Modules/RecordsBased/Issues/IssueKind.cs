#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues
{
	public enum IssueKind
	{
		/* object issues */

		MissingComponent = 0,
		DuplicateComponent = 50,
		MissingReference = 100,
		MissingPrefab = 300,
		UnnamedLayer = 800,
		HugePosition = 900,
		InconsistentTerrainData = 1100,

		/* project settings issues */
		 
		DuplicateLayers = 3010,
		Error = 5000,
		Other = 100000
	}
}