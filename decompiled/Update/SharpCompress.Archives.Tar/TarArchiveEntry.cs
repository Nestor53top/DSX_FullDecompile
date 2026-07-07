using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Tar;

namespace SharpCompress.Archives.Tar;

internal class TarArchiveEntry : TarEntry, IArchiveEntry, IEntry
{
	public IArchive Archive { get; }

	public bool IsComplete => true;

	internal TarArchiveEntry(TarArchive archive, TarFilePart part, CompressionType compressionType)
		: base(part, compressionType)
	{
		Archive = archive;
	}

	public virtual Stream OpenEntryStream()
	{
		return Parts.Single().GetCompressedStream();
	}
}
