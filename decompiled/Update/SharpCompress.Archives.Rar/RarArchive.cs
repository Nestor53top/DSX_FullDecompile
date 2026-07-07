using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Rar;
using SharpCompress.Common.Rar.Headers;
using SharpCompress.Compressors.Rar;
using SharpCompress.IO;
using SharpCompress.Readers;
using SharpCompress.Readers.Rar;

namespace SharpCompress.Archives.Rar;

internal class RarArchive : AbstractArchive<RarArchiveEntry, RarVolume>
{
	internal Unpack Unpack { get; } = new Unpack();

	public override bool IsSolid => base.Volumes.First().IsSolidArchive;

	internal RarArchive(FileInfo fileInfo, ReaderOptions options)
		: base(ArchiveType.Rar, fileInfo, options)
	{
	}

	protected override IEnumerable<RarVolume> LoadVolumes(FileInfo file)
	{
		return RarArchiveVolumeFactory.GetParts(file, base.ReaderOptions);
	}

	internal RarArchive(IEnumerable<Stream> streams, ReaderOptions options)
		: base(ArchiveType.Rar, streams, options)
	{
	}

	protected override IEnumerable<RarArchiveEntry> LoadEntries(IEnumerable<RarVolume> volumes)
	{
		return RarArchiveEntryFactory.GetEntries(this, volumes);
	}

	protected override IEnumerable<RarVolume> LoadVolumes(IEnumerable<Stream> streams)
	{
		return RarArchiveVolumeFactory.GetParts(streams, base.ReaderOptions);
	}

	protected override IReader CreateReaderForSolidExtraction()
	{
		Stream stream = base.Volumes.First().Stream;
		stream.Position = 0L;
		return RarReader.Open(stream, base.ReaderOptions);
	}

	public static RarArchive Open(string filePath, ReaderOptions options = null)
	{
		filePath.CheckNotNullOrEmpty("filePath");
		return new RarArchive(new FileInfo(filePath), options ?? new ReaderOptions());
	}

	public static RarArchive Open(FileInfo fileInfo, ReaderOptions options = null)
	{
		fileInfo.CheckNotNull("fileInfo");
		return new RarArchive(fileInfo, options ?? new ReaderOptions());
	}

	public static RarArchive Open(Stream stream, ReaderOptions options = null)
	{
		stream.CheckNotNull("stream");
		return Open(stream.AsEnumerable(), options ?? new ReaderOptions());
	}

	public static RarArchive Open(IEnumerable<Stream> streams, ReaderOptions options = null)
	{
		streams.CheckNotNull("streams");
		return new RarArchive(streams, options ?? new ReaderOptions());
	}

	public static bool IsRarFile(string filePath)
	{
		return IsRarFile(new FileInfo(filePath));
	}

	public static bool IsRarFile(FileInfo fileInfo)
	{
		if (!fileInfo.Exists)
		{
			return false;
		}
		using Stream stream = fileInfo.OpenRead();
		return IsRarFile(stream);
	}

	public static bool IsRarFile(Stream stream, ReaderOptions options = null)
	{
		try
		{
			return new RarHeaderFactory(StreamingMode.Seekable, options ?? new ReaderOptions()).ReadHeaders(stream).FirstOrDefault() is MarkHeader markHeader && markHeader.IsValid();
		}
		catch
		{
			return false;
		}
	}
}
