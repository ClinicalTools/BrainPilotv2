#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Tools
{
	using System;
	using System.IO;
	using UnityEngine;

	public static class CSAssetTools
	{
		public static bool IsAssetScriptableObjectWithMissingScript(string path)
		{
			var extension = Path.GetExtension(path);
			return string.Equals(extension, ".asset", StringComparison.OrdinalIgnoreCase);
		}

		public static int GetMainAssetInstanceID(string path)
		{
			var mi = CSReflectionTools.GetGetMainAssetInstanceIDMethodInfo();
			if (mi != null)
			{
				return (int)mi.Invoke(null, new object[] { path });
			}

			Debug.LogError(Maintainer.ConstructError("Can't retrieve InstanceID From path via reflection!"));
			return -1;
		}
	}
}