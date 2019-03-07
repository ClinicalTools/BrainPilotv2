#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Tools
{
	using System.IO;

	public static class CSFileTools
	{
		public static void DeleteFile(string path)
		{
			if (!File.Exists(path)) return;
			RemoveReadOnlyAttribute(path);
			File.Delete(path);
		}

		private static void RemoveReadOnlyAttribute(string filePath)
		{
			var attributes = File.GetAttributes(filePath);
			if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				File.SetAttributes(filePath, attributes & ~FileAttributes.ReadOnly);
		}
	}
}