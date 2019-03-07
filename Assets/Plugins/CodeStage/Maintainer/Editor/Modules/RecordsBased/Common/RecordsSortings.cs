#region copyright
//------------------------------------------------------------------------
// Copyright (C) 2015-2018 Dmitriy Yukhanov - focus [http://codestage.net]
//------------------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer
{
	using System;

	using Cleaner;
	using Issues;

	internal class RecordsSortings
	{
		internal static Func<CleanerRecord, string>				cleanerRecordByPath = record => record is AssetRecord ? ((AssetRecord)record).path : null;
		internal static Func<CleanerRecord, long>				cleanerRecordBySize = record => record is AssetRecord ? ((AssetRecord)record).size : 0;
		internal static Func<CleanerRecord, RecordType>			cleanerRecordByType = record => record.type;
		internal static Func<CleanerRecord, string>				cleanerRecordByAssetType = record => record is AssetRecord ? ((AssetRecord)record).type == RecordType.UnreferencedAsset ? ((AssetRecord)record).assetType.FullName : null : null;

		internal static Func<IssueRecord, string>				issueRecordByPath = record => record is GameObjectIssueRecord ? ((GameObjectIssueRecord)record).Path : null;
		internal static Func<IssueRecord, IssueKind>			issueRecordByType = record => record.Kind;
		internal static Func<IssueRecord, RecordSeverity>		issueRecordBySeverity = record => record.Severity;
	}
}