#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Settings
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using UnityEditor;
	using UnityEngine;

	using Core;
	using Tools;
	using Object = UnityEngine.Object;

	[Serializable]
	public class MaintainerSettings : ScriptableObject
	{
		internal const int UpdateProgressStep = 10;

		private const string Directory = "ProjectSettings";
		private const string Path = Directory + "/MaintainerSettings.asset";
		private static MaintainerSettings instance;

		public IssuesFinderSettings issuesFinderSettings;
		public ProjectCleanerSettings projectCleanerSettings;
		public ReferencesFinderSettings referencesFinderSettings;

		public string version = Maintainer.Version;

		public static MaintainerSettings Instance
		{
			get
			{
				if (instance != null) return instance;
				instance = LoadOrCreate();
				return instance;
			}
		}

		public static IssuesFinderSettings Issues
		{
			get
			{
				if (Instance.issuesFinderSettings == null)
				{
					Instance.issuesFinderSettings = new IssuesFinderSettings();
				}
				return Instance.issuesFinderSettings;
			}
		}

		public static ProjectCleanerSettings Cleaner
		{
			get
			{
				if (Instance.projectCleanerSettings == null)
				{
					Instance.projectCleanerSettings = new ProjectCleanerSettings();
				}

				return Instance.projectCleanerSettings;
			}
		}

		public static ReferencesFinderSettings References
		{
			get
			{
				if (Instance.referencesFinderSettings == null)
				{
					Instance.referencesFinderSettings = new ReferencesFinderSettings();
				}
				return Instance.referencesFinderSettings;
			}
		}

		public static void Delete()
		{
			instance = null;
			CSFileTools.DeleteFile(Path);
			MaintainerPersonalSettings.Delete();
		}

		public static void Save()
		{
			SaveInstance(Instance);
			MaintainerPersonalSettings.Save();
		}

		private static MaintainerSettings LoadOrCreate()
		{
			MaintainerSettings settings;

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

				if (settings.version != Maintainer.Version)
				{
					if (string.IsNullOrEmpty(settings.version))
					{
						MigrateAllIgnores(settings.issuesFinderSettings.pathIgnores, ref settings.issuesFinderSettings.pathIgnoresFilters, FilterKind.Path);
						settings.issuesFinderSettings.pathIgnores = null;

						MigrateAllIgnores(settings.issuesFinderSettings.componentIgnores, ref settings.issuesFinderSettings.componentIgnoresFilters, FilterKind.Type);
						settings.issuesFinderSettings.componentIgnores = null;

						MigrateAllIgnores(settings.issuesFinderSettings.pathIncludes, ref settings.issuesFinderSettings.pathIncludesFilters, FilterKind.Path);
						settings.issuesFinderSettings.pathIncludes = null;

						MigrateAllIgnores(settings.issuesFinderSettings.sceneIncludes, ref settings.issuesFinderSettings.sceneIncludesFilters, FilterKind.Path);
						settings.issuesFinderSettings.sceneIncludes = null;

						MigrateAllIgnores(settings.projectCleanerSettings.pathIgnores, ref settings.projectCleanerSettings.pathIgnoresFilters, FilterKind.Path);
						settings.projectCleanerSettings.pathIgnores = null;

						MigrateAllIgnores(settings.projectCleanerSettings.sceneIgnores, ref settings.projectCleanerSettings.sceneIgnoresFilters, FilterKind.Path);
						settings.projectCleanerSettings.sceneIgnores = null;

						settings.projectCleanerSettings.AddDefaultFilters();
					}

					if (new Version(settings.version) < new Version("1.4.1.0"))
					{
						if (!CSFilterTools.IsValueMatchesAnyFilterOfKind("dummy.asmdef", settings.projectCleanerSettings.pathIgnoresFilters, FilterKind.Extension))
						{
							ArrayUtility.Add(ref settings.projectCleanerSettings.pathIgnoresFilters, FilterItem.Create(".asmdef", FilterKind.Extension));
						}
					}

					if (new Version(settings.version) < new Version("1.5.1"))
					{
						if (settings.projectCleanerSettings.pathIgnoresFilters != null &&
						    settings.projectCleanerSettings.pathIgnoresFilters.Length > 0)
						{
							var defaultFilters = settings.projectCleanerSettings.GetDefaultFilters();
							var mandatoryFilters = settings.projectCleanerSettings.MandatoryFilters;

							var modificationsLog = new StringBuilder();

							for (var i = settings.projectCleanerSettings.pathIgnoresFilters.Length - 1; i >= 0; i--)
							{
								var pathIgnoresFilter = settings.projectCleanerSettings.pathIgnoresFilters[i];
								if (pathIgnoresFilter.ignoreCase) continue;

								var isMandatory = false;

								if (CSFilterTools.IsValueMatchesAnyFilterOfKind(pathIgnoresFilter.value, mandatoryFilters, pathIgnoresFilter.kind))
								{
									isMandatory = true;
								}
								else switch (pathIgnoresFilter.kind)
								{
									case FilterKind.Extension:
										var extension = pathIgnoresFilter.value.ToLowerInvariant();
										if (extension == ".dll" ||
										    extension == ".asmdef" ||
										    extension == ".mdb" ||
										    extension == ".xml" ||
										    extension == ".rsp")
										{
											isMandatory = true;
										}
										break;

									case FilterKind.FileName:
										var value = pathIgnoresFilter.value.ToLowerInvariant();
										if (value == "readme" ||
										    value == "manual")
										{
											isMandatory = true;
										}
										break;
								}

								if (isMandatory)
								{
									modificationsLog.Append("Removing Project Cleaner filter '")
										.Append(pathIgnoresFilter.value)
										.AppendLine("': built-in mandatory filter covers it now.");
									ArrayUtility.RemoveAt(ref settings.projectCleanerSettings.pathIgnoresFilters, i);
									continue;
								}

								if (CSFilterTools.IsValueMatchesAnyFilterOfKind(pathIgnoresFilter.value, defaultFilters, pathIgnoresFilter.kind))
								{
									modificationsLog.Append("Changing default Project Cleaner filter '")
										.Append(pathIgnoresFilter.value)
										.AppendLine("': ignore case setting to 'true' for better accuracy.");
									pathIgnoresFilter.ignoreCase = true;
								}
							}

							if (modificationsLog.Length > 0)
							{
								modificationsLog.Insert(0, "Maintainer settings updated, read below for details\n");
								Debug.Log(Maintainer.LogPrefix + modificationsLog);
							}
						}
					}

					SearchResultsStorage.Clear();
					//AssetsMap.Delete();
				}
			}

			settings.hideFlags = HideFlags.HideAndDontSave;
			settings.version = Maintainer.Version;

			return settings;
		}

		private static bool MigrateAllIgnores(string[] oldFilters, ref FilterItem[] newFilters, FilterKind filterKind)
		{
			if (oldFilters == null || oldFilters.Length == 0) return false;

			var newFiltersList = new List<FilterItem>(oldFilters.Length);
			foreach (var oldFilter in oldFilters)
			{
				if (CSFilterTools.IsValueMatchesAnyFilter(oldFilter, newFilters)) continue;
				newFiltersList.Add(FilterItem.Create(oldFilter, filterKind));
			}

			ArrayUtility.AddRange(ref newFilters, newFiltersList.ToArray());

			return true;
		}

		private static MaintainerSettings CreateNewSettingsFile()
		{
			var settingsInstance = CreateInstance();

			settingsInstance.projectCleanerSettings.SetDefaultFilters();
			SaveInstance(settingsInstance);

			return settingsInstance;
		}

		private static void SaveInstance(MaintainerSettings settingsInstance)
		{
			if (!System.IO.Directory.Exists(Directory)) System.IO.Directory.CreateDirectory(Directory);

			try
			{
				 UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new Object[]{settingsInstance}, Path, true);
			}
			catch (Exception ex)
			{
				Debug.LogError(Maintainer.ConstructError("Can't save settings!\n" + ex));
			}
		}

		private static MaintainerSettings LoadInstance()
		{
			MaintainerSettings settingsInstance;

			try
			{
				settingsInstance = (MaintainerSettings)UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(Path)[0];
			}
			catch (Exception ex)
			{
				Debug.Log(Maintainer.LogPrefix + "Can't read settings, resetting them to defaults.\nThis is a harmless message in most cases and can be ignored.\n" + ex);
				settingsInstance = null;
			}

			return settingsInstance;
		}

		private static MaintainerSettings CreateInstance()
		{
			var newInstance = CreateInstance<MaintainerSettings>();
			//var newInstance = new MaintainerSettings();
			newInstance.issuesFinderSettings = new IssuesFinderSettings();
			newInstance.projectCleanerSettings = new ProjectCleanerSettings();
			newInstance.referencesFinderSettings = new ReferencesFinderSettings();
			return newInstance;
		}
	}
}