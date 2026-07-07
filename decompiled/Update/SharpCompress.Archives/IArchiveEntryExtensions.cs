using System.IO;
using SharpCompress.Common;
using SharpCompress.IO;
using SharpCompress.Readers;

namespace SharpCompress.Archives;

internal static class IArchiveEntryExtensions
{
	public static void WriteTo(this IArchiveEntry archiveEntry, Stream streamToWriteTo)
	{
		if (archiveEntry.Archive.Type == ArchiveType.Rar && archiveEntry.Archive.IsSolid)
		{
			throw new InvalidFormatException("Cannot use Archive random access on SOLID Rar files.");
		}
		if (archiveEntry.IsDirectory)
		{
			throw new ExtractionException("Entry is a file directory and cannot be extracted.");
		}
		IArchiveExtractionListener archiveExtractionListener = archiveEntry.Archive as IArchiveExtractionListener;
		archiveExtractionListener.EnsureEntriesLoaded();
		archiveExtractionListener.FireEntryExtractionBegin(archiveEntry);
		archiveExtractionListener.FireFilePartExtractionBegin(archiveEntry.Key, archiveEntry.Size, archiveEntry.CompressedSize);
		Stream stream = archiveEntry.OpenEntryStream();
		if (stream == null)
		{
			return;
		}
		using (stream)
		{
			using Stream source = new ListeningStream(archiveExtractionListener, stream);
			source.TransferTo(streamToWriteTo);
		}
		archiveExtractionListener.FireEntryExtractionEnd(archiveEntry);
	}

	public static void WriteToDirectory(this IArchiveEntry entry, string destinationDirectory, ExtractionOptions options = null)
	{
		string fileName = Path.GetFileName(entry.Key);
		options = options ?? new ExtractionOptions
		{
			Overwrite = true
		};
		string destinationFileName;
		if (options.ExtractFullPath)
		{
			string directoryName = Path.GetDirectoryName(entry.Key);
			string text = Path.Combine(destinationDirectory, directoryName);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			destinationFileName = Path.Combine(text, fileName);
		}
		else
		{
			destinationFileName = Path.Combine(destinationDirectory, fileName);
		}
		if (!entry.IsDirectory)
		{
			entry.WriteToFile(destinationFileName, options);
		}
	}

	public static void WriteToFile(this IArchiveEntry entry, string destinationFileName, ExtractionOptions options = null)
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
		using (FileStream streamToWriteTo = File.Open(destinationFileName, mode))
		{
			entry.WriteTo(streamToWriteTo);
		}
		entry.PreserveExtractionOptions(destinationFileName, options);
	}
}
