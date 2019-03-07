#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.References
{
	using System;
	using Core;

	[Serializable]
	public class ReferencesTreeElement : TreeElement
	{
		public string assetPath;
		public string assetTypeName;
		public long assetSize;
		public string assetSizeFormatted;
		public bool assetIsTexture;
		public AssetSettingsKind assetSettingsKind;

		public ReferencingEntryData[] referencingEntries;

		/*public bool exactReferencesChecked = true;
		public bool exactReferencesExpanded = true;*/

		protected override bool Search(string searchString)
		{
			return assetPath.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1;
		}
	}
}