using System;
using System.IO;

namespace SharpCompress.Writers;

internal static class IWriterExtensions
{
	public static void Write(this IWriter writer, string entryPath, Stream source)
	{
		writer.Write(entryPath, source, null);
	}

	public static void Write(this IWriter writer, string entryPath, FileInfo source)
	{
		if (!source.Exists)
		{
			throw new ArgumentException("Source does not exist: " + source.FullName);
		}
		using FileStream source2 = source.OpenRead();
		writer.Write(entryPath, source2, source.LastWriteTime);
	}

	public static void Write(this IWriter writer, string entryPath, string source)
	{
		writer.Write(entryPath, new FileInfo(source));
	}

	public static void WriteAll(this IWriter writer, string directory, string searchPattern = "*", SearchOption option = SearchOption.TopDirectoryOnly)
	{
		if (!Directory.Exists(directory))
		{
			throw new ArgumentException("Directory does not exist: " + directory);
		}
		foreach (string item in Directory.EnumerateFiles(directory, searchPattern, option))
		{
			writer.Write(item.Substring(directory.Length), item);
		}
	}
}
