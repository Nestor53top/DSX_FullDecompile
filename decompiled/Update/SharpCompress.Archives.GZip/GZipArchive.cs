using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.GZip;
using SharpCompress.Readers;
using SharpCompress.Readers.GZip;
using SharpCompress.Writers;
using SharpCompress.Writers.GZip;

namespace SharpCompress.Archives.GZip;

internal class GZipArchive : AbstractWritableArchive<GZipArchiveEntry, GZipVolume>
{
	public static GZipArchive Open(string filePath, ReaderOptions readerOptions = null)
	{
		filePath.CheckNotNullOrEmpty("filePath");
		return Open(new FileInfo(filePath), readerOptions ?? new ReaderOptions());
	}

	public static GZipArchive Open(FileInfo fileInfo, ReaderOptions readerOptions = null)
	{
		fileInfo.CheckNotNull("fileInfo");
		return new GZipArchive(fileInfo, readerOptions ?? new ReaderOptions());
	}

	public static GZipArchive Open(Stream stream, ReaderOptions readerOptions = null)
	{
		stream.CheckNotNull("stream");
		return new GZipArchive(stream, readerOptions ?? new ReaderOptions());
	}

	public static GZipArchive Create()
	{
		return new GZipArchive();
	}

	internal GZipArchive(FileInfo fileInfo, ReaderOptions options)
		: base(ArchiveType.GZip, fileInfo, options)
	{
	}

	protected override IEnumerable<GZipVolume> LoadVolumes(FileInfo file)
	{
		return new GZipVolume(file, base.ReaderOptions).AsEnumerable();
	}

	public static bool IsGZipFile(string filePath)
	{
		return IsGZipFile(new FileInfo(filePath));
	}

	public static bool IsGZipFile(FileInfo fileInfo)
	{
		if (!fileInfo.Exists)
		{
			return false;
		}
		using Stream stream = fileInfo.OpenRead();
		return IsGZipFile(stream);
	}

	public void SaveTo(string filePath)
	{
		SaveTo(new FileInfo(filePath));
	}

	public void SaveTo(FileInfo fileInfo)
	{
		using FileStream stream = fileInfo.Open(FileMode.Create, FileAccess.Write);
		SaveTo(stream, new WriterOptions(CompressionType.GZip));
	}

	public static bool IsGZipFile(Stream stream)
	{
		byte[] array = new byte[10];
		switch (stream.Read(array, 0, array.Length))
		{
		case 0:
			return false;
		default:
			return false;
		case 10:
			if (array[0] != 31 || array[1] != 139 || array[2] != 8)
			{
				return false;
			}
			return true;
		}
	}

	internal GZipArchive(Stream stream, ReaderOptions options)
		: base(ArchiveType.GZip, stream, options)
	{
	}

	internal GZipArchive()
		: base(ArchiveType.GZip)
	{
	}

	protected override GZipArchiveEntry CreateEntryInternal(string filePath, Stream source, long size, DateTime? modified, bool closeStream)
	{
		if (Entries.Any())
		{
			throw new InvalidOperationException("Only one entry is allowed in a GZip Archive");
		}
		return new GZipWritableArchiveEntry(this, source, filePath, size, modified, closeStream);
	}

	protected override void SaveTo(Stream stream, WriterOptions options, IEnumerable<GZipArchiveEntry> oldEntries, IEnumerable<GZipArchiveEntry> newEntries)
	{
		if (Entries.Count > 1)
		{
			throw new InvalidOperationException("Only one entry is allowed in a GZip Archive");
		}
		using GZipWriter gZipWriter = new GZipWriter(stream);
		foreach (GZipArchiveEntry item in from x in oldEntries.Concat(newEntries)
			where !x.IsDirectory
			select x)
		{
			using Stream source = item.OpenEntryStream();
			gZipWriter.Write(item.Key, source, item.LastModifiedTime);
		}
	}

	protected override IEnumerable<GZipVolume> LoadVolumes(IEnumerable<Stream> streams)
	{
		return new GZipVolume(streams.First(), base.ReaderOptions).AsEnumerable();
	}

	protected override IEnumerable<GZipArchiveEntry> LoadEntries(IEnumerable<GZipVolume> volumes)
	{
		Stream stream = volumes.Single().Stream;
		yield return new GZipArchiveEntry(this, new GZipFilePart(stream));
	}

	protected override IReader CreateReaderForSolidExtraction()
	{
		Stream stream = base.Volumes.Single().Stream;
		stream.Position = 0L;
		return GZipReader.Open(stream);
	}
}
