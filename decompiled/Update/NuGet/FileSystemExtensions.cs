using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NuGet.Resources;

namespace NuGet;

internal static class FileSystemExtensions
{
	public static IEnumerable<string> GetFiles(this IFileSystem fileSystem, string path, string filter)
	{
		return fileSystem.GetFiles(path, filter, recursive: false);
	}

	public static void AddFiles(IFileSystem fileSystem, IEnumerable<IPackageFile> files, string rootDir)
	{
		fileSystem.AddFiles(files, rootDir, preserveFilePath: true);
	}

	public static void AddFiles(this IFileSystem fileSystem, IEnumerable<IPackageFile> files, string rootDir, bool preserveFilePath)
	{
		foreach (IPackageFile file in files)
		{
			string path = Path.Combine(rootDir, preserveFilePath ? file.Path : Path.GetFileName(file.Path));
			fileSystem.AddFileWithCheck(path, file.GetStream);
		}
	}

	internal static void DeleteFiles(IFileSystem fileSystem, IEnumerable<IPackageFile> files, string rootDir)
	{
		ILookup<string, IPackageFile> lookup = files.ToLookup((IPackageFile p) => Path.GetDirectoryName(p.Path));
		foreach (string item in from grouping in lookup
			from directory in GetDirectories(grouping.Key)
			orderby directory.Length descending
			select directory)
		{
			IEnumerable<IPackageFile> enumerable = (lookup.Contains(item) ? lookup[item] : Enumerable.Empty<IPackageFile>());
			string path = Path.Combine(rootDir, item);
			if (!fileSystem.DirectoryExists(path))
			{
				continue;
			}
			foreach (IPackageFile item2 in enumerable)
			{
				string path2 = Path.Combine(rootDir, item2.Path);
				fileSystem.DeleteFileSafe(path2, item2.GetStream);
			}
			if (!fileSystem.GetFilesSafe(path).Any() && !fileSystem.GetDirectoriesSafe(path).Any())
			{
				fileSystem.DeleteDirectorySafe(path, recursive: false);
			}
		}
	}

	internal static IEnumerable<string> GetDirectoriesSafe(this IFileSystem fileSystem, string path)
	{
		try
		{
			return fileSystem.GetDirectories(path);
		}
		catch (Exception ex)
		{
			fileSystem.Logger.Log(MessageLevel.Warning, ex.Message);
		}
		return Enumerable.Empty<string>();
	}

	internal static IEnumerable<string> GetFilesSafe(this IFileSystem fileSystem, string path)
	{
		return fileSystem.GetFilesSafe(path, "*.*");
	}

	internal static IEnumerable<string> GetFilesSafe(this IFileSystem fileSystem, string path, string filter)
	{
		try
		{
			return fileSystem.GetFiles(path, filter);
		}
		catch (Exception ex)
		{
			fileSystem.Logger.Log(MessageLevel.Warning, ex.Message);
		}
		return Enumerable.Empty<string>();
	}

	internal static void DeleteDirectorySafe(this IFileSystem fileSystem, string path, bool recursive)
	{
		DoSafeAction(delegate
		{
			fileSystem.DeleteDirectory(path, recursive);
		}, fileSystem.Logger);
	}

	internal static void DeleteFileSafe(this IFileSystem fileSystem, string path)
	{
		DoSafeAction(delegate
		{
			fileSystem.DeleteFile(path);
		}, fileSystem.Logger);
	}

	public static bool ContentEqual(IFileSystem fileSystem, string path, Func<Stream> streamFactory)
	{
		using Stream stream = streamFactory();
		using Stream otherStream = fileSystem.OpenFile(path);
		return stream.ContentEquals(otherStream);
	}

	public static void DeleteFileSafe(this IFileSystem fileSystem, string path, Func<Stream> streamFactory)
	{
		if (fileSystem.FileExists(path))
		{
			if (ContentEqual(fileSystem, path, streamFactory))
			{
				fileSystem.DeleteFileSafe(path);
				return;
			}
			fileSystem.Logger.Log(MessageLevel.Warning, NuGetResources.Warning_FileModified, path);
		}
	}

	public static void DeleteFileAndParentDirectoriesIfEmpty(this IFileSystem fileSystem, string filePath)
	{
		fileSystem.DeleteFileSafe(filePath);
		string directoryName = Path.GetDirectoryName(filePath);
		while (!string.IsNullOrEmpty(directoryName) && !fileSystem.GetFiles(directoryName, "*.*").Any() && !fileSystem.GetDirectories(directoryName).Any())
		{
			fileSystem.DeleteDirectorySafe(directoryName, recursive: false);
			directoryName = Path.GetDirectoryName(directoryName);
		}
	}

	internal static void AddFileWithCheck(this IFileSystem fileSystem, string path, Func<Stream> streamFactory)
	{
		if (fileSystem.FileExists(path))
		{
			fileSystem.Logger.Log(MessageLevel.Warning, NuGetResources.Warning_FileAlreadyExists, path);
			return;
		}
		using Stream stream = streamFactory();
		fileSystem.AddFile(path, stream);
	}

	internal static void AddFileWithCheck(this IFileSystem fileSystem, string path, Action<Stream> write)
	{
		if (fileSystem.FileExists(path))
		{
			fileSystem.Logger.Log(MessageLevel.Warning, NuGetResources.Warning_FileAlreadyExists, path);
		}
		else
		{
			fileSystem.AddFile(path, write);
		}
	}

	internal static IEnumerable<string> GetDirectories(string path)
	{
		foreach (int item in IndexOfAll(path, Path.DirectorySeparatorChar))
		{
			yield return path.Substring(0, item);
		}
		yield return path;
	}

	private static IEnumerable<int> IndexOfAll(string value, char ch)
	{
		int index = -1;
		do
		{
			index = value.IndexOf(ch, index + 1);
			if (index >= 0)
			{
				yield return index;
			}
		}
		while (index >= 0);
	}

	private static void DoSafeAction(Action action, ILogger logger)
	{
		try
		{
			Attempt(action);
		}
		catch (Exception ex)
		{
			logger.Log(MessageLevel.Warning, ex.Message);
		}
	}

	private static void Attempt(Action action, int retries = 3, int delayBeforeRetry = 150)
	{
		while (retries > 0)
		{
			try
			{
				action();
				break;
			}
			catch
			{
				retries--;
				if (retries == 0)
				{
					throw;
				}
			}
			Thread.Sleep(delayBeforeRetry);
		}
	}
}
