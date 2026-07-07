using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NuGet;

internal static class PathValidator
{
	private static readonly char[] _invalidPathChars = Path.GetInvalidPathChars();

	public static bool IsValidSource(string source)
	{
		if (!IsValidLocalPath(source) && !IsValidUncPath(source))
		{
			return IsValidUrl(source);
		}
		return true;
	}

	public static bool IsValidLocalPath(string path)
	{
		try
		{
			if (Environment.OSVersion.Platform != PlatformID.MacOSX && Environment.OSVersion.Platform != PlatformID.Unix && !Regex.IsMatch(path.Trim(), "^[A-Za-z]:\\\\"))
			{
				return false;
			}
			return Path.IsPathRooted(path) && path.IndexOfAny(_invalidPathChars) == -1;
		}
		catch
		{
			return false;
		}
	}

	public static bool IsValidUncPath(string path)
	{
		try
		{
			Path.GetFullPath(path);
			return Regex.IsMatch(path.Trim(), "^\\\\\\\\");
		}
		catch
		{
			return false;
		}
	}

	public static bool IsValidUrl(string url)
	{
		Uri result;
		if (Regex.IsMatch(url, "^\\w+://", RegexOptions.IgnoreCase))
		{
			return Uri.TryCreate(url, UriKind.Absolute, out result);
		}
		return false;
	}
}
