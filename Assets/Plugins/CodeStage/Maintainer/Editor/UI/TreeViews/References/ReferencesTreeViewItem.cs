#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using Core;
	using References;
	using UnityEditor;
	using UnityEngine;

	internal class ReferencesTreeViewItem<T> : MaintainerTreeViewItem<T> where T : ReferencesTreeElement
	{
		public ReferencesTreeViewItem(int id, int depth, string displayName, T data) : base(id, depth, displayName, data) { }

		protected override void Initialize()
		{
			if (depth == -1) return;

			if (data.recursionId == -1)
			{
				if (data.assetIsTexture)
				{
					icon = AssetPreview.GetMiniTypeThumbnail(typeof(Texture));
				}
				else
				{
					if (data.assetSettingsKind == AssetSettingsKind.NotSettings)
					{
						icon = (Texture2D)AssetDatabase.GetCachedIcon(data.assetPath);
					}
					else
					{
						icon = (Texture2D)CSIcons.Gear;
					}
				}

				if (icon == null)
				{
					icon = (Texture2D)CSEditorIcons.WarnSmallIcon;
				}
			}
			else
			{
				icon = (Texture2D)CSIcons.Repeat;
			}
		}
	}
}