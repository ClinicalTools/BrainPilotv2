using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomUtilities
{
	public static class LoadAsset
	{
		public static void Load<T>(out T obj, string folder = "Assets") where T : Object
		{
			string[] assets = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T), new[] { folder });
			if (assets.Length > 0) {
				string path = UnityEditor.AssetDatabase.GUIDToAssetPath(assets[0]);
				obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
			} else {
				obj = null;
			}
		}
	}
}