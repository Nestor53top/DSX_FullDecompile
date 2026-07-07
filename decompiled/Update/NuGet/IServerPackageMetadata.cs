using System;

namespace NuGet;

internal interface IServerPackageMetadata
{
	Uri ReportAbuseUrl { get; }

	int DownloadCount { get; }
}
