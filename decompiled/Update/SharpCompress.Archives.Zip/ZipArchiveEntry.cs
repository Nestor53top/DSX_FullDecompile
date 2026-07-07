using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Zip;

namespace SharpCompress.Archives.Zip;

internal class ZipArchiveEntry : ZipEntry, IArchiveEntry, IEntry
{
	public IArchive Archive { get; }

	public bool IsComplete => true;

	public string Comment => (Parts.Single() as SeekableZipFilePart).Comment;

	internal ZipArchiveEntry(ZipArchive archive, SeekableZipFilePart part)
		: base(part)
	{
		Archive = archive;
	}

	public virtual Stream OpenEntryStream()
	{
		return Parts.Single().GetCompressedStream();
	}
}
