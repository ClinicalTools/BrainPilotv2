#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core
{
	using System;
	using System.Text;
	using Tools;

	public enum Location
	{
		NotFound = 0,
		Invisible = 10,
		SceneGameObject = 20,
		PrefabAssetGameObject = 30,
		PrefabAssetObject = 35,
		ScriptAsset = 40,
		ScriptableObjectAsset = 50,
		SceneLightingSettings = 60,
		SceneNavigationSettings = 70,
		TileMap = 80,
	}

	[Serializable]
	public class ReferencingEntryData
	{
		public Location location;

		public string prefixLabel;
		public string transformPath;
		public string componentName;
		public string propertyPath;
		public string suffixLabel;

		public long objectId = -1L;
		public long componentId = -1L;

		[NonSerialized]
		private StringBuilder labelStringBuilder;

		[NonSerialized]
		private string label;

		public string GetLabel()
		{
			if (label != null) return label; 

			labelStringBuilder = new StringBuilder();
			
			var needsSpace = false;

			if (!string.IsNullOrEmpty(prefixLabel))
			{
				labelStringBuilder.Append(prefixLabel);
				needsSpace = true;
			}

			if (!string.IsNullOrEmpty(transformPath))
			{
				if (needsSpace) labelStringBuilder.Append(' ');

				string labelTransformPath = null;

				if (location == Location.PrefabAssetGameObject)
				{
					var transformPathSplitterIndex = transformPath.IndexOf('/'); 
					if (transformPathSplitterIndex != -1)
					{
						labelTransformPath = transformPath.Remove(0, transformPathSplitterIndex + 1);
					}
					else
					{
						labelTransformPath = "[Prefab Root]";
					}
				}
				else
				{
					labelTransformPath = transformPath;
				}

				if (labelTransformPath != null)
				{
					labelStringBuilder.Append(labelTransformPath);
					needsSpace = true;
				}
			}

			if (!string.IsNullOrEmpty(componentName))
			{
				if (needsSpace) labelStringBuilder.Append(" | ");
				labelStringBuilder.Append(componentName);
				needsSpace = true;
			}

			if (!string.IsNullOrEmpty(propertyPath))
			{
				if (needsSpace) labelStringBuilder.Append(": ");
				var nicePropertyPath = CSObjectTools.GetNicePropertyPath(propertyPath);
				labelStringBuilder.Append(nicePropertyPath);
				needsSpace = true;
			}

			if (!string.IsNullOrEmpty(suffixLabel))
			{
				if (needsSpace) labelStringBuilder.Append(' ');
				labelStringBuilder.Append(suffixLabel);
			}

			label = labelStringBuilder.ToString();

			labelStringBuilder.Length = 0;

			return label;
		}
	}
}