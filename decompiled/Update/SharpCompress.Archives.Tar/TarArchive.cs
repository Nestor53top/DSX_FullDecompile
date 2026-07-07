using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Tar;
using SharpCompress.Common.Tar.Headers;
using SharpCompress.IO;
using SharpCompress.Readers;
using SharpCompress.Readers.Tar;
using SharpCompress.Writers;
using SharpCompress.Writers.Tar;

namespace SharpCompress.Archives.Tar;

internal class TarArchive : AbstractWritableArchive<TarArchiveEntry, TarVolume>
{
	public static TarArchive Open(string filePath, ReaderOptions readerOptions = null)
	{
		filePath.CheckNotNullOrEmpty("filePath");
		return Open(new FileInfo(filePath), readerOptions ?? new ReaderOptions());
	}

	public static TarArchive Open(FileInfo fileInfo, ReaderOptions readerOptions = null)
	{
		fileInfo.CheckNotNull("fileInfo");
		return new TarArchive(fileInfo, readerOptions ?? new ReaderOptions());
	}

	public static TarArchive Open(Stream stream, ReaderOptions readerOptions = null)
	{
		stream.CheckNotNull("stream");
		return new TarArchive(stream, readerOptions ?? new ReaderOptions());
	}

	public static bool IsTarFile(string filePath)
	{
		return IsTarFile(new FileInfo(filePath));
	}

	public static bool IsTarFile(FileInfo fileInfo)
	{
		if (!fileInfo.Exists)
		{
			return false;
		}
		using Stream stream = fileInfo.OpenRead();
		return IsTarFile(stream);
	}

	public static bool IsTarFile(Stream stream)
	{
		try
		{
			TarHeader tarHeader = new TarHeader();
			tarHeader.Read(new BinaryReader(stream));
			return tarHeader.Name.Length > 0 && Enum.IsDefined(typeof(EntryType), tarHeader.EntryType);
		}
		catch
		{
		}
		return false;
	}

	internal TarArchive(FileInfo fileInfo, ReaderOptions readerOptions)
		: base(ArchiveType.Tar, fileInfo, readerOptions)
	{
	}

	protected override IEnumerable<TarVolume> LoadVolumes(FileInfo file)
	{
		return new TarVolume(file.OpenRead(), base.ReaderOptions).AsEnumerable();
	}

	internal TarArchive(Stream stream, ReaderOptions readerOptions)
		: base(ArchiveType.Tar, stream, readerOptions)
	{
	}

	internal TarArchive()
		: base(ArchiveType.Tar)
	{
	}

	protected override IEnumerable<TarVolume> LoadVolumes(IEnumerable<Stream> streams)
	{
		return new TarVolume(streams.First(), base.ReaderOptions).AsEnumerable();
	}

	protected override IEnumerable<TarArchiveEntry> LoadEntries(IEnumerable<TarVolume> volumes)
	{
		Stream stream = volumes.Single().Stream;
		TarHeader previousHeader = null;
		foreach (TarHeader item in TarHeaderFactory.ReadHeader(StreamingMode.Seekable, stream))
		{
			if (item == null)
			{
				continue;
			}
			if (item.EntryType == EntryType.LongName)
			{
				previousHeader = item;
				continue;
			}
			if (previousHeader != null)
			{
				TarArchiveEntry tarArchiveEntry = new TarArchiveEntry(this, new TarFilePart(previousHeader, stream), CompressionType.None);
				long position = stream.Position;
				using (Stream source = tarArchiveEntry.OpenEntryStream())
				{
					using MemoryStream memoryStream = new MemoryStream();
					source.TransferTo(memoryStream);
					memoryStream.Position = 0L;
					byte[] array = memoryStream.ToArray();
					item.Name = ArchiveEncoding.Default.GetString(array, 0, array.Length).TrimNulls();
				}
				stream.Position = position;
				previousHeader = null;
			}
			yield return new TarArchiveEntry(this, new TarFilePart(item, stream), CompressionType.None);
		}
	}

	public static TarArchive Create()
	{
		return new TarArchive();
	}

	protected override TarArchiveEntry CreateEntryInternal(string filePath, Stream source, long size, DateTime? modified, bool closeStream)
	{
		return new TarWritableArchiveEntry(this, source, CompressionType.Unknown, filePath, size, modified, closeStream);
	}

	protected override void SaveTo(Stream stream, WriterOptions options, IEnumerable<TarArchiveEntry> oldEntries, IEnumerable<TarArchiveEntry> newEntries)
	{
		using TarWriter tarWriter = new TarWriter(stream, options);
		foreach (TarArchiveEntry item in from x in oldEntries.Concat(newEntries)
			where !x.IsDirectory
			select x)
		{
			using Stream source = item.OpenEntryStream();
			tarWriter.Write(item.Key, source, item.LastModifiedTime, item.Size);
		}
	}

	protected override IReader CreateReaderForSolidExtraction()
	{
		Stream stream = base.Volumes.Single().Stream;
		stream.Position = 0L;
		return TarReader.Open(stream);
	}
}
