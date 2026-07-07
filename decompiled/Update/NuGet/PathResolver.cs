using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NuGet;

internal static class PathResolver
{
	private struct SearchPathResult
	{
		private readonly string _path;

		private readonly bool _isFile;

		public string Path => _path;

		public bool IsFile => _isFile;

		public SearchPathResult(string path, bool isFile)
		{
			_path = path;
			_isFile = isFile;
		}
	}

	private static readonly string OneDotSlash;

	private static readonly string TwoDotSlash;

	public static IEnumerable<T> GetMatches<T>(IEnumerable<T> source, Func<T, string> getPath, IEnumerable<string> wildcards)
	{
		IEnumerable<Regex> filters = wildcards.Select(WildcardToRegex);
		return source.Where(delegate(T item)
		{
			string path = getPath(item);
			return filters.Any((Regex f) => f.IsMatch(path));
		});
	}

	public static void FilterPackageFiles<T>(ICollection<T> source, Func<T, string> getPath, IEnumerable<string> wildcards)
	{
		HashSet<T> hashSet = new HashSet<T>(GetMatches(source, getPath, wildcards));
		source.RemoveAll(hashSet.Contains);
	}

	public static string NormalizeWildcardForExcludedFiles(string basePath, string wildcard)
	{
		if (wildcard.StartsWith("**", StringComparison.OrdinalIgnoreCase))
		{
			return wildcard;
		}
		basePath = NormalizeBasePath(basePath, ref wildcard);
		return Path.Combine(basePath, wildcard);
	}

	private static Regex WildcardToRegex(string wildcard)
	{
		string text = Regex.Escape(wildcard);
		text = ((Path.DirectorySeparatorChar != '/') ? text.Replace("/", "\\\\").Replace("\\*\\*\\\\", ".*").Replace("\\*\\*", ".*")
			.Replace("\\*", "[^\\\\]*(\\\\)?")
			.Replace("\\?", ".") : text.Replace("\\*\\*/", ".*").Replace("\\*\\*", ".*").Replace("\\*", "[^/]*(/)?")
			.Replace("\\?", "."));
		return new Regex("^" + text + "$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
	}

	internal static IEnumerable<PhysicalPackageFile> ResolveSearchPattern(string basePath, string searchPath, string targetPath, bool includeEmptyDirectories)
	{
		string normalizedBasePath;
		return from result in PerformWildcardSearchInternal(basePath, searchPath, includeEmptyDirectories, out normalizedBasePath)
			select (!result.IsFile) ? new EmptyFrameworkFolderFile(ResolvePackagePath(normalizedBasePath, searchPath, result.Path, targetPath))
			{
				SourcePath = result.Path
			} : new PhysicalPackageFile
			{
				SourcePath = result.Path,
				TargetPath = ResolvePackagePath(normalizedBasePath, searchPath, result.Path, targetPath)
			};
	}

	public static IEnumerable<string> PerformWildcardSearch(string basePath, string searchPath)
	{
		string normalizedBasePath;
		return from s in PerformWildcardSearchInternal(basePath, searchPath, includeEmptyDirectories: false, out normalizedBasePath)
			select s.Path;
	}

	private static IEnumerable<SearchPathResult> PerformWildcardSearchInternal(string basePath, string searchPath, bool includeEmptyDirectories, out string normalizedBasePath)
	{
		if (!searchPath.StartsWith("\\\\", StringComparison.OrdinalIgnoreCase) && Path.DirectorySeparatorChar != '/')
		{
			searchPath = searchPath.TrimStart(new char[1] { Path.DirectorySeparatorChar });
		}
		bool flag = false;
		if (IsDirectoryPath(searchPath))
		{
			string text = searchPath;
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			searchPath = text + "**" + directorySeparatorChar + "*";
			flag = true;
		}
		basePath = NormalizeBasePath(basePath, ref searchPath);
		normalizedBasePath = GetPathToEnumerateFrom(basePath, searchPath);
		Regex searchRegex = WildcardToRegex(Path.Combine(basePath, searchPath));
		SearchOption searchOption = SearchOption.AllDirectories;
		bool num = searchPath.IndexOf("**", StringComparison.OrdinalIgnoreCase) != -1;
		bool flag2 = Enumerable.Contains(Path.GetDirectoryName(searchPath), '*');
		if (!num && !flag2)
		{
			searchOption = SearchOption.TopDirectoryOnly;
		}
		IEnumerable<SearchPathResult> enumerable = from file in Directory.GetFiles(normalizedBasePath, "*.*", searchOption)
			where searchRegex.IsMatch(file)
			select new SearchPathResult(file, isFile: true);
		if (!includeEmptyDirectories)
		{
			return enumerable;
		}
		IEnumerable<SearchPathResult> enumerable2 = from directory in Directory.GetDirectories(normalizedBasePath, "*.*", searchOption)
			where searchRegex.IsMatch(directory) && IsEmptyDirectory(directory)
			select new SearchPathResult(directory, isFile: false);
		if (flag && IsEmptyDirectory(normalizedBasePath))
		{
			enumerable2 = enumerable2.Concat(new SearchPathResult[1]
			{
				new SearchPathResult(normalizedBasePath, isFile: false)
			});
		}
		return enumerable.Concat(enumerable2);
	}

	internal static string GetPathToEnumerateFrom(string basePath, string searchPath)
	{
		int num = searchPath.IndexOf('*');
		if (num == -1)
		{
			string directoryName = Path.GetDirectoryName(searchPath);
			return Path.Combine(basePath, directoryName);
		}
		int num2 = searchPath.LastIndexOf(Path.DirectorySeparatorChar, num);
		if (num2 == -1)
		{
			return basePath;
		}
		string path = searchPath.Substring(0, num2);
		return Path.Combine(basePath, path);
	}

	internal static string ResolvePackagePath(string searchDirectory, string searchPattern, string fullPath, string targetPath)
	{
		bool flag = IsDirectoryPath(searchPattern);
		bool flag2 = IsWildcardSearch(searchPattern);
		string path;
		if (((flag2 && searchPattern.IndexOf("**", StringComparison.OrdinalIgnoreCase) != -1) || flag) && fullPath.StartsWith(searchDirectory, StringComparison.OrdinalIgnoreCase))
		{
			path = fullPath.Substring(searchDirectory.Length).TrimStart(new char[1] { Path.DirectorySeparatorChar });
		}
		else
		{
			if (!flag2 && Path.GetExtension(searchPattern).Equals(Path.GetExtension(targetPath), StringComparison.OrdinalIgnoreCase))
			{
				return targetPath;
			}
			path = Path.GetFileName(fullPath);
		}
		return Path.Combine(targetPath ?? string.Empty, path);
	}

	internal static string NormalizeBasePath(string basePath, ref string searchPath)
	{
		basePath = (string.IsNullOrEmpty(basePath) ? OneDotSlash : basePath);
		while (searchPath.StartsWith(TwoDotSlash, StringComparison.OrdinalIgnoreCase))
		{
			basePath = Path.Combine(basePath, TwoDotSlash);
			searchPath = searchPath.Substring(TwoDotSlash.Length);
		}
		return Path.GetFullPath(basePath);
	}

	internal static bool IsWildcardSearch(string filter)
	{
		return filter.IndexOf('*') != -1;
	}

	internal static bool IsDirectoryPath(string path)
	{
		if (path != null && path.Length > 1)
		{
			if (path[path.Length - 1] != Path.DirectorySeparatorChar)
			{
				return path[path.Length - 1] == Path.AltDirectorySeparatorChar;
			}
			return true;
		}
		return false;
	}

	private static bool IsEmptyDirectory(string directory)
	{
		return !Directory.EnumerateFileSystemEntries(directory).Any();
	}

	static PathResolver()
	{
		char directorySeparatorChar = Path.DirectorySeparatorChar;
		OneDotSlash = "." + directorySeparatorChar;
		directorySeparatorChar = Path.DirectorySeparatorChar;
		TwoDotSlash = ".." + directorySeparatorChar;
	}
}
