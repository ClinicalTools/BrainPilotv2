#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Tools
{
	using UnityEditor.AI;
	using UnityEngine;

	public static class CSSettingsTools
	{
		public static Object GetInSceneLightmapSettings()
		{
			var mi = CSReflectionTools.GetGetLightmapSettingsMethodInfo();
			if (mi != null)
			{
				return (Object)mi.Invoke(null, null);
			}

			Debug.LogError(Maintainer.ConstructError("Can't retrieve LightmapSettings object via reflection!"));
			return null;
		}

		public static Object GetInSceneRenderSettings()
		{
			var mi = CSReflectionTools.GetGetRenderSettingsMethodInfo();
			if (mi != null)
			{
				return (Object)mi.Invoke(null, null);
			}

			Debug.LogError(Maintainer.ConstructError("Can't retrieve RenderSettings object via reflection!"));
			return null;
		}

		public static Object GetInSceneNavigationSettings()
		{
			return NavMeshBuilder.navMeshSettingsObject;
		}
	}
}