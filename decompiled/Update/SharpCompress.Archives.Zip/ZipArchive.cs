using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Zip;
using SharpCompress.Common.Zip.Headers;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Readers;
using SharpCompress.Readers.Zip;
using SharpCompress.Writers;
using SharpCompress.Writers.Zip;

namespace SharpCompress.Archives.Zip;

internal class ZipArchive : AbstractWritableArchive<ZipArchiveEntry, ZipVolume>
{
	private readonly SeekableZipHeaderFactory headerFactory;

	public CompressionLevel DeflateCompressionLevel { get; set; }

	public static ZipArchive Open(string filePath, ReaderOptions readerOptions = null)
	{
		filePath.CheckNotNullOrEmpty("filePath");
		return Open(new FileInfo(filePath), readerOptions ?? new ReaderOptions());
	}

	public static ZipArchive Open(FileInfo fileInfo, ReaderOptions readerOptions = null)
	{
		fileInfo.CheckNotNull("fileInfo");
		return new ZipArchive(fileInfo, readerOptions ?? new ReaderOptions());
	}

	public static ZipArchive Open(Stream stream, ReaderOptions readerOptions = null)
	{
		stream.CheckNotNull("stream");
		return new ZipArchive(stream, readerOptions ?? new ReaderOptions());
	}

	public static bool IsZipFile(string filePath, string password = null)
	{
		return IsZipFile(new FileInfo(filePath), password);
	}

	public static bool IsZipFile(FileInfo fileInfo, string password = null)
	{
		if (!fileInfo.Exists)
		{
			return false;
		}
		using Stream stream = fileInfo.OpenRead();
		return IsZipFile(stream, password);
	}

	public static bool IsZipFile(Stream stream, string password = null)
	{
		StreamingZipHeaderFactory streamingZipHeaderFactory = new StreamingZipHeaderFactory(password);
		try
		{
			ZipHeader zipHeader = streamingZipHeaderFactory.ReadStreamHeader(stream).FirstOrDefault((ZipHeader x) => x.ZipHeaderType != ZipHeaderType.Split);
			if (zipHeader == null)
			{
				return false;
			}
			return Enum.IsDefined(typeof(ZipHeaderType), zipHeader.ZipHeaderType);
		}
		catch (CryptographicException)
		{
			return true;
		}
		catch
		{
			return false;
		}
	}

	internal ZipArchive(FileInfo fileInfo, ReaderOptions readerOptions)
		: base(ArchiveType.Zip, fileInfo, readerOptions)
	{
		headerFactory = new SeekableZipHeaderFactory(readerOptions.Password);
	}

	protected override IEnumerable<ZipVolume> LoadVolumes(FileInfo file)
	{
		return new ZipVolume(file.OpenRead(), base.ReaderOptions).AsEnumerable();
	}

	internal ZipArchive()
		: base(ArchiveType.Zip)
	{
	}

	internal ZipArchive(Stream stream, ReaderOptions readerOptions)
		: base(ArchiveType.Zip, stream, readerOptions)
	{
		headerFactory = new SeekableZipHeaderFactory(readerOptions.Password);
	}

	protected override IEnumerable<ZipVolume> LoadVolumes(IEnumerable<Stream> streams)
	{
		return new ZipVolume(streams.First(), base.ReaderOptions).AsEnumerable();
	}

	protected override IEnumerable<ZipArchiveEntry> LoadEntries(IEnumerable<ZipVolume> volumes)
	{
		ZipVolume volume = volumes.Single();
		Stream stream = volume.Stream;
		foreach (DirectoryEntryHeader h in headerFactory.ReadSeekableHeader(stream))
		{
			if (h != null)
			{
				switch (h.ZipHeaderType)
				{
				case ZipHeaderType.DirectoryEntry:
					yield return new ZipArchiveEntry(this, new SeekableZipFilePart(headerFactory, h as DirectoryEntryHeader, stream));
					break;
				case ZipHeaderType.DirectoryEnd:
				{
					byte[] comment = (h as DirectoryEndHeader).Comment;
					volume.Comment = ArchiveEncoding.Default.GetString(comment, 0, comment.Length);
					yield break;
				}
				}
			}
		}
	}

	public void SaveTo(Stream stream)
	{
		SaveTo(stream, new WriterOptions(CompressionType.Deflate));
	}

	protected override void SaveTo(Stream stream, WriterOptions options, IEnumerable<ZipArchiveEntry> oldEntries, IEnumerable<ZipArchiveEntry> newEntries)
	{
		using ZipWriter zipWriter = new ZipWriter(stream, new ZipWriterOptions(options));
		foreach (ZipArchiveEntry item in from x in oldEntries.Concat(newEntries)
			where !x.IsDirectory
			select x)
		{
			using Stream source = item.OpenEntryStream();
			zipWriter.Write(item.Key, source, item.LastModifiedTime);
		}
	}

	protected override ZipArchiveEntry CreateEntryInternal(string filePath, Stream source, long size, DateTime? modified, bool closeStream)
	{
		return new ZipWritableArchiveEntry(this, source, filePath, size, modified, closeStream);
	}

	public static ZipArchive Create()
	{
		return new ZipArchive();
	}

	protected override IReader CreateReaderForSolidExtraction()
	{
		Stream stream = base.Volumes.Single().Stream;
		stream.Position = 0L;
		return ZipReader.Open(stream);
	}
}
