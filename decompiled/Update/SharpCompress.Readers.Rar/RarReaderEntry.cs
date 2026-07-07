using System.Collections.Generic;
using SharpCompress.Common;
using SharpCompress.Common.Rar;
using SharpCompress.Common.Rar.Headers;

namespace SharpCompress.Readers.Rar;

internal class RarReaderEntry : RarEntry
{
	internal RarFilePart Part { get; }

	internal override IEnumerable<FilePart> Parts => ((FilePart)Part).AsEnumerable();

	internal override FileHeader FileHeader => Part.FileHeader;

	public override CompressionType CompressionType => CompressionType.Rar;

	public override long CompressedSize => Part.FileHeader.CompressedSize;

	public override long Size => Part.FileHeader.UncompressedSize;

	internal RarReaderEntry(bool solid, RarFilePart part)
	{
		Part = part;
		base.IsSolid = solid;
	}
}
