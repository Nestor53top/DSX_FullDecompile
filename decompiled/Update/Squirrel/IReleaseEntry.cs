using System;
using NuGet;

namespace Squirrel;

internal interface IReleaseEntry
{
	string SHA1 { get; }

	string Filename { get; }

	long Filesize { get; }

	bool IsDelta { get; }

	string EntryAsString { get; }

	SemanticVersion Version { get; }

	string PackageName { get; }

	float? StagingPercentage { get; }

	string GetReleaseNotes(string packageDirectory);

	Uri GetIconUrl(string packageDirectory);
}
