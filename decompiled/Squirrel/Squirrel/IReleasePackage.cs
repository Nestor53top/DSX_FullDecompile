using System;

namespace Squirrel;

public interface IReleasePackage
{
	string InputPackageFile { get; }

	string ReleasePackageFile { get; }

	string SuggestedReleaseFileName { get; }

	string CreateReleasePackage(string outputFile, string packagesRootDir = null, Func<string, string> releaseNotesProcessor = null, Action<string> contentsPostProcessHook = null);
}
