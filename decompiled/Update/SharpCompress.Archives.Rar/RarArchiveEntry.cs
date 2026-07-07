using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Rar;
using SharpCompress.Common.Rar.Headers;
using SharpCompress.Compressors.Rar;

namespace SharpCompress.Archives.Rar;

internal class RarArchiveEntry : RarEntry, IArchiveEntry, IEntry
{
	private readonly ICollection<RarFilePart> parts;

	private readonly RarArchive archive;

	public override CompressionType CompressionType => CompressionType.Rar;

	public IArchive Archive => archive;

	internal override IEnumerable<FilePart> Parts => parts.Cast<FilePart>();

	internal override FileHeader FileHeader => parts.First().FileHeader;

	public override long Crc
	{
		get
		{
			CheckIncomplete();
			return parts.Select((RarFilePart fp) => fp.FileHeader).Single((FileHeader fh) => !fh.FileFlags.HasFlag(FileFlags.SPLIT_AFTER)).FileCRC;
		}
	}

	public override long Size
	{
		get
		{
			CheckIncomplete();
			return parts.First().FileHeader.UncompressedSize;
		}
	}

	public override long CompressedSize
	{
		get
		{
			CheckIncomplete();
			return parts.Aggregate(0L, (long total, RarFilePart fp) => total + fp.FileHeader.CompressedSize);
		}
	}

	public bool IsComplete => parts.Select((RarFilePart fp) => fp.FileHeader).Any((FileHeader fh) => !fh.FileFlags.HasFlag(FileFlags.SPLIT_AFTER));

	internal RarArchiveEntry(RarArchive archive, IEnumerable<RarFilePart> parts)
	{
		this.parts = parts.ToList();
		this.archive = archive;
	}

	public Stream OpenEntryStream()
	{
		if (archive.IsSolid)
		{
			throw new InvalidOperationException("Use ExtractAllEntries to extract SOLID archives.");
		}
		return new RarStream(archive.Unpack, FileHeader, new MultiVolumeReadOnlyStream(Parts.Cast<RarFilePart>(), archive));
	}

	private void CheckIncomplete()
	{
		if (!IsComplete)
		{
			throw new IncompleteArchiveException("ArchiveEntry is incomplete and cannot perform this operation.");
		}
	}
}
