using System.Text.RegularExpressions;
using NuGet;

namespace Squirrel;

internal static class VersionExtensions
{
	private static readonly Regex _suffixRegex = new Regex("(-full|-delta)?\\.nupkg$", RegexOptions.Compiled);

	private static readonly Regex _versionRegex = new Regex("\\d+(\\.\\d+){0,3}(-[A-Za-z][0-9A-Za-z-]*)?$", RegexOptions.Compiled);

	public static SemanticVersion ToSemanticVersion(this IReleasePackage package)
	{
		return package.InputPackageFile.ToSemanticVersion();
	}

	public static SemanticVersion ToSemanticVersion(this string fileName)
	{
		string input = _suffixRegex.Replace(fileName, "");
		return new SemanticVersion(_versionRegex.Match(input).Value);
	}
}
