#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Settings
{
	using System;
	using System.IO;
	using Tools;
	using UI;
	using UnityEngine;

	[Serializable]
	public class MaintainerPersonalSettings : ScriptableObject
	{
		private const string Directory = "Library";
		private const string Path = Directory + "/MaintainerPersonalSettings.asset";
		private static MaintainerPersonalSettings instance;

		public IssuesFinderPersonalSettings issuesFinderSettings;
		public ProjectCleanerPersonalSettings projectCleanerSettings;
		public ReferencesFinderPersonalSettings referencesFinderSettings;

		public MaintainerWindow.MaintainerTab selectedTab = MaintainerWindow.MaintainerTab.Issues;

		public string version = Maintainer.Version;

		public static MaintainerPersonalSettings Instance
		{
			get
			{
				if (instance != null) return instance;
				instance = LoadOrCreate();
				return instance;
			}
		}

		public static IssuesFinderPersonalSettings Issues
		{
			get
			{
				if (Instance.issuesFinderSettings == null)
				{
					Instance.issuesFinderSettings = new IssuesFinderPersonalSettings();
				}
				return Instance.issuesFinderSettings;
			}
		}

		public static ProjectCleanerPersonalSettings Cleaner
		{
			get
			{
				if (Instance.projectCleanerSettings == null)
				{
					Instance.projectCleanerSettings = new ProjectCleanerPersonalSettings();
				}
				return Instance.projectCleanerSettings;
			}
		}

		public static ReferencesFinderPersonalSettings References
		{
			get
			{
				if (Instance.referencesFinderSettings == null)
				{
					Instance.referencesFinderSettings = new ReferencesFinderPersonalSettings();
				}
				return Instance.referencesFinderSettings;
			}
		}

		public static void Delete()
		{
			instance = null;
			CSFileTools.DeleteFile(Path);
		}

		public static void Save()
		{
			SaveInstance(Instance);
		}

		private static MaintainerPersonalSettings LoadOrCreate()
		{
			MaintainerPersonalSettings settings;

			if (!File.Exists(Path))
			{
				settings = CreateNewSettingsFile();
			}
			else
			{
				settings = LoadInstance();

				if (settings == null)
				{
					CSFileTools.DeleteFile(Path);
					settings = CreateNewSettingsFile();
				}
			}

			settings.version = Maintainer.Version;

			return settings;
		}

		private static MaintainerPersonalSettings CreateNewSettingsFile()
		{
			var settingsInstance = CreateInstance();

			SaveInstance(settingsInstance);

			return settingsInstance;
		}

		private static void SaveInstance(MaintainerPersonalSettings settingsInstance)
		{
			if (!System.IO.Directory.Exists(Directory)) System.IO.Directory.CreateDirectory(Directory);

			try
			{
				UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new[] { settingsInstance }, Path, true);
			}
			catch (Exception ex)
			{
				Debug.LogError(Maintainer.ConstructError("Can't save personal settings!\n" + ex));
			}
		}

		private static MaintainerPersonalSettings LoadInstance()
		{
			MaintainerPersonalSettings settingsInstance;

			try
			{
				settingsInstance = (MaintainerPersonalSettings)UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(Path)[0];
			}
			catch (Exception ex)
			{
				Debug.Log(Maintainer.LogPrefix + "Can't read personal settings, resetting them to defaults.\nThis is a harmless message in most cases and can be ignored.\n" + ex);
				settingsInstance = null;
			}

			return settingsInstance;
		}

		private static MaintainerPersonalSettings CreateInstance()
		{
			var newInstance = CreateInstance<MaintainerPersonalSettings>();
			newInstance.issuesFinderSettings = new IssuesFinderPersonalSettings();
			newInstance.projectCleanerSettings = new ProjectCleanerPersonalSettings();
			newInstance.referencesFinderSettings = new ReferencesFinderPersonalSettings();
			return newInstance;
		}
	}
}