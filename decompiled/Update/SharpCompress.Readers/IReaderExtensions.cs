using System.IO;
using SharpCompress.Common;

namespace SharpCompress.Readers;

internal static class IReaderExtensions
{
	public static void WriteEntryTo(this IReader reader, string filePath)
	{
		using Stream writableStream = File.Open(filePath, FileMode.Create, FileAccess.Write);
		reader.WriteEntryTo(writableStream);
	}

	public static void WriteEntryTo(this IReader reader, FileInfo filePath)
	{
		using Stream writableStream = filePath.Open(FileMode.Create);
		reader.WriteEntryTo(writableStream);
	}

	public static void WriteAllToDirectory(this IReader reader, string destinationDirectory, ExtractionOptions options = null)
	{
		while (reader.MoveToNextEntry())
		{
			reader.WriteEntryToDirectory(destinationDirectory, options);
		}
	}

	public static void WriteEntryToDirectory(this IReader reader, string destinationDirectory, ExtractionOptions options = null)
	{
		string empty = string.Empty;
		string fileName = Path.GetFileName(reader.Entry.Key);
		options = options ?? new ExtractionOptions
		{
			Overwrite = true
		};
		if (options.ExtractFullPath)
		{
			string directoryName = Path.GetDirectoryName(reader.Entry.Key);
			string text = Path.Combine(destinationDirectory, directoryName);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			empty = Path.Combine(text, fileName);
		}
		else
		{
			empty = Path.Combine(destinationDirectory, fileName);
		}
		if (!reader.Entry.IsDirectory)
		{
			reader.WriteEntryToFile(empty, options);
		}
		else if (options.ExtractFullPath && !Directory.Exists(empty))
		{
			Directory.CreateDirectory(empty);
		}
	}

	public static void WriteEntryToFile(this IReader reader, string destinationFileName, ExtractionOptions options = null)
	{
		FileMode mode = FileMode.Create;
		options = options ?? new ExtractionOptions
		{
			Overwrite = true
		};
		if (!options.Overwrite)
		{
			mode = FileMode.CreateNew;
		}
		using (FileStream writableStream = File.Open(destinationFileName, mode))
		{
			reader.WriteEntryTo(writableStream);
		}
		reader.Entry.PreserveExtractionOptions(destinationFileName, options);
	}
}
