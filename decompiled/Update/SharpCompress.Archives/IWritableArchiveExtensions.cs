using System;
using System.IO;
using SharpCompress.Writers;

namespace SharpCompress.Archives;

internal static class IWritableArchiveExtensions
{
	public static void AddEntry(this IWritableArchive writableArchive, string entryPath, string filePath)
	{
		FileInfo fileInfo = new FileInfo(filePath);
		if (!fileInfo.Exists)
		{
			throw new FileNotFoundException("Could not AddEntry: " + filePath);
		}
		writableArchive.AddEntry(entryPath, new FileInfo(filePath).OpenRead(), closeStream: true, fileInfo.Length, fileInfo.LastWriteTime);
	}

	public static void SaveTo(this IWritableArchive writableArchive, string filePath, WriterOptions options)
	{
		writableArchive.SaveTo(new FileInfo(filePath), options);
	}

	public static void SaveTo(this IWritableArchive writableArchive, FileInfo fileInfo, WriterOptions options)
	{
		using FileStream stream = fileInfo.Open(FileMode.Create, FileAccess.Write);
		writableArchive.SaveTo(stream, options);
	}

	public static void AddAllFromDirectory(this IWritableArchive writableArchive, string filePath, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
	{
		foreach (string item in Directory.EnumerateFiles(filePath, searchPattern, searchOption))
		{
			FileInfo fileInfo = new FileInfo(item);
			writableArchive.AddEntry(item.Substring(filePath.Length), fileInfo.OpenRead(), closeStream: true, fileInfo.Length, fileInfo.LastWriteTime);
		}
	}

	public static IArchiveEntry AddEntry(this IWritableArchive writableArchive, string key, FileInfo fileInfo)
	{
		if (!fileInfo.Exists)
		{
			throw new ArgumentException("FileInfo does not exist.");
		}
		return writableArchive.AddEntry(key, fileInfo.OpenRead(), closeStream: true, fileInfo.Length, fileInfo.LastWriteTime);
	}
}
