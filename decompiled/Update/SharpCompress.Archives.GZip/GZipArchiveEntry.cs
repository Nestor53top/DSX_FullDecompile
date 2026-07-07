using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.GZip;

namespace SharpCompress.Archives.GZip;

internal class GZipArchiveEntry : GZipEntry, IArchiveEntry, IEntry
{
	public IArchive Archive { get; }

	public bool IsComplete => true;

	internal GZipArchiveEntry(GZipArchive archive, GZipFilePart part)
		: base(part)
	{
		Archive = archive;
	}

	public virtual Stream OpenEntryStream()
	{
		GZipFilePart gZipFilePart = Parts.Single() as GZipFilePart;
		if (gZipFilePart.GetRawStream().Position != gZipFilePart.EntryStartPosition)
		{
			gZipFilePart.GetRawStream().Position = gZipFilePart.EntryStartPosition;
		}
		return Parts.Single().GetCompressedStream();
	}
}
