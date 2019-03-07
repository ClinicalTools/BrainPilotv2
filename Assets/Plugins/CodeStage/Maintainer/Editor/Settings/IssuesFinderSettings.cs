#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Settings
{
	using System;

	using Core;

	[Serializable]
	public class IssuesFinderSettings
	{
		[Serializable]
		public enum ScenesSelection
		{
			AllScenes,
			IncludedScenes,
			OpenedScenesOnly
		}

		// ----------------------------------------------------------------------------
		// filtering
		// ----------------------------------------------------------------------------

		public bool includeScenesInBuild = true;
		public bool includeOnlyEnabledScenesInBuild = true;

		public string[] sceneIncludes = new string[0];
		public string[] pathIgnores = new string[0];
		public string[] pathIncludes = new string[0];
		public string[] componentIgnores = new string[0];

		public FilterItem[] sceneIncludesFilters = new FilterItem[0];
		public FilterItem[] pathIgnoresFilters = new FilterItem[0];
		public FilterItem[] pathIncludesFilters = new FilterItem[0];
		public FilterItem[] componentIgnoresFilters = new FilterItem[0];

		// ----------------------------------------------------------------------------
		// where to look
		// ----------------------------------------------------------------------------

		public bool lookInScenes;
		public bool lookInAssets;
		public bool lookInProjectSettings;

		public ScenesSelection scenesSelection;

		public bool scanGameObjects;
		public bool touchInactiveGameObjects;
		public bool touchDisabledComponents;
		public bool scanGameObjectsFoldout;

		// ----------------------------------------------------------------------------
		// what to look for
		// ----------------------------------------------------------------------------

		public bool gameObjectsFoldout;
		public bool commonFoldout;
		public bool neatnessFoldout;
		public bool projectSettingsFoldout;

		/* project-wide  */

		public bool missingReferences;

		/* game objects common  */

		public bool missingComponents;
		public bool missingPrefabs;

		public bool duplicateComponents;
		public bool inconsistentTerrainData;

		/* game objects neatness */

		public bool unnamedLayers;
		public bool hugePositions;

		/* project settings */

		public bool duplicateLayers;

		public IssuesFinderSettings()
		{
            Reset();
		}

		internal void SwitchAll(bool enable)
		{
			missingReferences = enable;

			SwitchCommon(enable);
			SwitchNeatness(enable);
			SwitchProjectSettings(enable);
		}

		internal void SwitchCommon(bool enable)
		{
			missingComponents = enable;
			missingPrefabs = enable;
			duplicateComponents = enable;
			inconsistentTerrainData = enable;
		}

		internal void SwitchNeatness(bool enable)
		{
			unnamedLayers = enable;
			hugePositions = enable;
		}

		internal void SwitchProjectSettings(bool enable)
		{
			duplicateLayers = enable;
		}

		internal void Reset()
		{
			scanGameObjects = true;
			gameObjectsFoldout = true;
			commonFoldout = false;
			neatnessFoldout = false;
			lookInProjectSettings = true;
			projectSettingsFoldout = true;
			lookInScenes = true;
			scenesSelection = ScenesSelection.AllScenes;
			lookInAssets = true;
			touchInactiveGameObjects = true;
			touchDisabledComponents = true;
			missingComponents = true;
			duplicateComponents = true;
			missingReferences = true;
			inconsistentTerrainData = true;
			missingPrefabs = true;
			unnamedLayers = true;
			hugePositions = true;
			duplicateLayers = true;
		}
	}
}