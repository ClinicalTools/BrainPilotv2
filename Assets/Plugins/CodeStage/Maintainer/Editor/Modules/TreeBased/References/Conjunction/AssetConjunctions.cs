#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.References
{
	using System.Collections.Generic;
	using Core;

	internal class AssetConjunctions
	{
		public AssetInfo asset;
		public readonly List<TreeConjunction> conjunctions = new List<TreeConjunction>();
	}
}