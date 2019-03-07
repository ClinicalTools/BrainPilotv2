#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.References
{
	using System.Collections.Generic;
	using Core;

	internal class TreeConjunction
	{
		public readonly List<ReferencesTreeElement> treeElements = new List<ReferencesTreeElement>();
		public AssetInfo referencedAsset;
		public ReferencedAtInfo referencedAtInfo;
	}
}