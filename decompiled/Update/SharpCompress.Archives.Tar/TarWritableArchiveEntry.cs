using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.IO;

namespace SharpCompress.Archives.Tar;

internal class TarWritableArchiveEntry : TarArchiveEntry, IWritableArchiveEntry
{
	private readonly bool closeStream;

	private readonly Stream stream;

	public override long Crc => 0L;

	public override string Key { get; }

	public override long CompressedSize => 0L;

	public override long Size { get; }

	public override DateTime? LastModifiedTime { get; }

	public override DateTime? CreatedTime => null;

	public override DateTime? LastAccessedTime => null;

	public override DateTime? ArchivedTime => null;

	public override bool IsEncrypted => false;

	public override bool IsDirectory => false;

	public override bool IsSplit => false;

	internal override IEnumerable<FilePart> Parts
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	Stream IWritableArchiveEntry.Stream => stream;

	internal TarWritableArchiveEntry(TarArchive archive, Stream stream, CompressionType compressionType, string path, long size, DateTime? lastModified, bool closeStream)
		: base(archive, null, compressionType)
	{
		this.stream = stream;
		Key = path;
		Size = size;
		LastModifiedTime = lastModified;
		this.closeStream = closeStream;
	}

	public override Stream OpenEntryStream()
	{
		stream.Seek(0L, SeekOrigin.Begin);
		return new NonDisposingStream(stream);
	}

	internal override void Close()
	{
		if (closeStream)
		{
			stream.Dispose();
		}
	}
}
