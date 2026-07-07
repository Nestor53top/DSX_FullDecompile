using System.IO;
using SharpCompress.Common;
using SharpCompress.Common.SevenZip;

namespace SharpCompress.Archives.SevenZip;

internal class SevenZipArchiveEntry : SevenZipEntry, IArchiveEntry, IEntry
{
	public IArchive Archive { get; }

	public bool IsComplete => true;

	public bool IsAnti => base.FilePart.Header.IsAnti;

	internal SevenZipArchiveEntry(SevenZipArchive archive, SevenZipFilePart part)
		: base(part)
	{
		Archive = archive;
	}

	public Stream OpenEntryStream()
	{
		return base.FilePart.GetCompressedStream();
	}
}
