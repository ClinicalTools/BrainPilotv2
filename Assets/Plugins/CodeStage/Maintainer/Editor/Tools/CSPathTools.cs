#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Tools
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	
	using UnityEngine;

	using Core;
	using UnityEditor;

	public class CSPathTools
	{
		public static readonly int assetsFolderIndex = EnforceSlashes(Application.dataPath).IndexOf("/Assets", StringComparison.Ordinal);

		public static string GetPathRelativeToProject(string path)
		{
			return !Path.IsPathRooted(path) ? path : path.Substring(assetsFolderIndex + 1);
		}

		public static string NicifyAssetPath(string path, bool trimExtension = false)
		{
			return NicifyAssetPath(path, AssetKind.Regular, trimExtension);
		}

		public static string NicifyAssetPath(string path, AssetKind kind, bool trimExtension = false)
		{
			var nicePath = path;

			switch (kind)
			{
				case AssetKind.Regular:
					if (path.Length <= 7) return path;
					nicePath = nicePath.Remove(0, 7);
					break;
				case AssetKind.Settings:
					break;
				case AssetKind.FromPackage:
					break;
				case AssetKind.Unsupported:
					break;
				default:
					throw new ArgumentOutOfRangeException("kind", kind, null);
			}

			if (trimExtension)
			{
				var lastSlash = nicePath.LastIndexOf('/');
				var lastDot = nicePath.LastIndexOf('.');

				// making sure we'll not trim path like Test/My.Test/linux_file
				if (lastDot > lastSlash)
				{
					nicePath = nicePath.Remove(lastDot, nicePath.Length - lastDot);
				}
			}

			return nicePath;
		}

		public static string EnforceSlashes(string path)
		{
			return string.IsNullOrEmpty(path) ? path : path.Replace('\\', '/');
		}

		// source: UnityEditor.U2D.SpriteAtlasInspector.IsPackable(Object) : bool
		public static string[] GetAllPackableAssetsPathsRecursive(string folder)
		{
			var results = AssetDatabase.FindAssets("t:Sprite t:Texture2D", new[] {folder});

			for (var i = 0; i < results.Length; i++)
			{
				results[i] = AssetDatabase.GUIDToAssetPath(results[i]);
			}
			
			return results;
		}
	}
}