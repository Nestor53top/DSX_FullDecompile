using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;

namespace NuGet;

internal static class UriUtility
{
	internal static string GetPath(Uri uri)
	{
		string text = uri.OriginalString;
		if (text.StartsWith("/", StringComparison.Ordinal))
		{
			text = text.Substring(1);
		}
		return Uri.UnescapeDataString(text.Replace('/', Path.DirectorySeparatorChar));
	}

	internal static Uri CreatePartUri(string path)
	{
		IEnumerable<string> values = path.Split(new char[2]
		{
			'/',
			Path.DirectorySeparatorChar
		}, StringSplitOptions.None).Select(Uri.EscapeDataString);
		return PackUriHelper.CreatePartUri(new Uri(string.Join("/", values), UriKind.Relative));
	}

	private static Uri CreateODataAgnosticUri(string uri)
	{
		if (uri.EndsWith("$metadata", StringComparison.OrdinalIgnoreCase))
		{
			uri = uri.Substring(0, uri.Length - 9).TrimEnd(new char[1] { '/' });
		}
		return new Uri(uri);
	}

	public static bool UriEquals(Uri uri1, Uri uri2)
	{
		uri1 = CreateODataAgnosticUri(uri1.OriginalString.TrimEnd(new char[1] { '/' }));
		uri2 = CreateODataAgnosticUri(uri2.OriginalString.TrimEnd(new char[1] { '/' }));
		return Uri.Compare(uri1, uri2, UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0;
	}

	public static bool UriStartsWith(Uri uri1, Uri uri2)
	{
		if (!UriEquals(uri1, uri2))
		{
			return uri1.IsBaseOf(uri2);
		}
		return true;
	}
}
