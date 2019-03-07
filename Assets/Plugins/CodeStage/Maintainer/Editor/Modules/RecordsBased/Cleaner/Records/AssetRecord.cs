#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Cleaner
{
	using System;
	using System.IO;
	using System.Text;
	
	using UnityEditor;
	using UnityEngine;

	using Core;
	using Settings;
	using Tools;
	using UI;

	[Serializable]
	public class AssetRecord : CleanerRecord, IShowableRecord
	{
		public string path;
		public long size; // in bytes
		public string beautyPath;
		public string assetDatabasePath;
		public Type assetType;
		public bool isTexture;

		public void Show()
		{
			if (!CSSelectionTools.RevealAndSelectFileAsset(assetDatabasePath))
			{
				MaintainerWindow.ShowNotification("Can't show it properly");
			}
		}

		internal static AssetRecord CreateEmptyFolderRecord(string folderPath)
		{
			var newRecord = new AssetRecord(folderPath);

			if (!string.IsNullOrEmpty(newRecord.path))
			{
				return newRecord;
			}

			return null;
		}

		internal static AssetRecord Create(RecordType type, AssetInfo assetInfo)
		{
			var newRecord = new AssetRecord(type, assetInfo);
			return newRecord;
		}

		internal override bool MatchesFilter(FilterItem newFilter)
		{
			var filters = new[] { newFilter };

			switch (newFilter.kind)
			{
				case FilterKind.Path:
				case FilterKind.Directory:
				case FilterKind.FileName:
				case FilterKind.Extension:
					return !string.IsNullOrEmpty(assetDatabasePath) && CSFilterTools.IsValueMatchesAnyFilterOfKind(assetDatabasePath, filters, newFilter.kind);
				case FilterKind.Type:
					return assetType != null && CSFilterTools.IsValueMatchesAnyFilterOfKind(assetType.Name, filters, newFilter.kind);
				case FilterKind.NotSet:
					return false;
				default:
					Debug.LogWarning(Maintainer.LogPrefix + "Unknown filter kind: " + newFilter.kind);
					return false;
			}
		}

		protected AssetRecord(string folderPath) : base(RecordType.EmptyFolder, RecordLocation.Asset)
		{
			path = folderPath;

			assetDatabasePath = !Path.IsPathRooted(folderPath) ? folderPath : folderPath.Substring(CSPathTools.assetsFolderIndex + 1);
			beautyPath = CSPathTools.NicifyAssetPath(assetDatabasePath);
		}

		protected AssetRecord(RecordType type, AssetInfo assetInfo) : base(type, RecordLocation.Asset)
		{
			path = assetInfo.Path;

			assetDatabasePath = CSPathTools.GetPathRelativeToProject(path);
			beautyPath = CSPathTools.NicifyAssetPath(assetDatabasePath);
			assetType = assetInfo.Type;

			if (assetType.BaseType == CSReflectionTools.textureType)
			{
				isTexture = true;
			}

			if (type == RecordType.UnreferencedAsset)
			{
				size = assetInfo.Size;
			}
		}

		protected override void ConstructCompactLine(StringBuilder text)
		{
			text.Append(beautyPath);
		}

		protected override void ConstructHeader(StringBuilder header)
		{
			base.ConstructHeader(header);

			if (type == RecordType.UnreferencedAsset)
			{
				header.Append(assetType.Name);
			}
		}

		protected override void ConstructBody(StringBuilder text)
		{
			text.Append("<b>Path:</b> ").Append(beautyPath);
			if (size > 0)
			{
				text.AppendLine().Append("<b>Size:</b> ").Append(CSEditorTools.FormatBytes(size));
			}
			if (type == RecordType.UnreferencedAsset)
			{
				text.AppendLine().Append("<b>Full Type:</b> ").Append(assetType.FullName);
			}
		}

		protected override bool PerformClean()
		{
			bool result;

			if (MaintainerSettings.Cleaner.useTrashBin)
			{
				result = AssetDatabase.MoveAssetToTrash(assetDatabasePath);
			}
			else
			{
				switch (type)
				{
					case RecordType.EmptyFolder:
						{
							if (Directory.Exists(path))
							{
								Directory.Delete(path, true);
							}
							break;
						}
					case RecordType.UnreferencedAsset:
						{
							CSFileTools.DeleteFile(path);
							break;
						}
					case RecordType.Error:
						break;
					case RecordType.Other:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				var metaPath = path + ".meta";
				CSFileTools.DeleteFile(metaPath);
				result = !(Directory.Exists(path) || File.Exists(path));
			}
				
			if (!result)
			{
				Debug.LogWarning(Maintainer.LogPrefix + ProjectCleaner.ModuleName + " can't clean asset: " + beautyPath);
			}
			else
			{
				var directory = CSPathTools.EnforceSlashes(Path.GetDirectoryName(path));
				if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
				{
					var filesInDir = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);

					if (filesInDir.Length == 0)
					{
						CreateEmptyFolderRecord(directory).Clean();
					}
				}
			}

			return result;
		}
	}
}