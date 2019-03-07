#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.References.Entry
{
	using UnityEngine;

#if UNITY_2018_2_OR_NEWER
	using UnityEngine.Tilemaps;
#endif

	internal static class ManualComponentProcessor
	{
#if UNITY_2018_2_OR_NEWER
		public static void ProcessTilemap(Object inspectedUnityObject, Tilemap target, EntryAddSettings addSettings)
		{
			var tilesCount = target.GetUsedTilesCount();
			if (tilesCount == 0) return;

			var usedTiles = new TileBase[tilesCount];
			target.GetUsedTilesNonAlloc(usedTiles);

			foreach (var usedTile in usedTiles)
			{
				ReferenceEntryFinder.TryAddEntryToMatchedConjunctions(inspectedUnityObject, usedTile.GetInstanceID(), addSettings);

				var tile = usedTile as Tile;
				if (tile == null) continue;

				if (tile.sprite != null)
				{
					ReferenceEntryFinder.TryAddEntryToMatchedConjunctions(inspectedUnityObject, tile.sprite.GetInstanceID(),
						addSettings);
				}
			}
		}
#endif
	}
}