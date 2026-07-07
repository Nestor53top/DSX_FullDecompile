using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Rar;
using SharpCompress.Compressors.Rar;

namespace SharpCompress.Readers.Rar;

internal abstract class RarReader : AbstractReader<RarReaderEntry, RarVolume>
{
	private RarVolume volume;

	private readonly Unpack pack = new Unpack();

	public override RarVolume Volume => volume;

	internal RarReader(ReaderOptions options)
		: base(options, ArchiveType.Rar)
	{
	}

	internal abstract void ValidateArchive(RarVolume archive);

	public static RarReader Open(Stream stream, ReaderOptions options = null)
	{
		stream.CheckNotNull("stream");
		return new SingleVolumeRarReader(stream, options ?? new ReaderOptions());
	}

	public static RarReader Open(IEnumerable<Stream> streams, ReaderOptions options = null)
	{
		streams.CheckNotNull("streams");
		return new MultiVolumeRarReader(streams, options ?? new ReaderOptions());
	}

	internal override IEnumerable<RarReaderEntry> GetEntries(Stream stream)
	{
		volume = new RarReaderVolume(stream, base.Options);
		foreach (RarFilePart item in volume.ReadFileParts())
		{
			ValidateArchive(volume);
			yield return new RarReaderEntry(volume.IsSolidArchive, item);
		}
	}

	protected virtual IEnumerable<FilePart> CreateFilePartEnumerableForCurrentEntry()
	{
		return base.Entry.Parts;
	}

	protected override EntryStream GetEntryStream()
	{
		return CreateEntryStream(new RarCrcStream(pack, base.Entry.FileHeader, new MultiVolumeReadOnlyStream(CreateFilePartEnumerableForCurrentEntry().Cast<RarFilePart>(), this)));
	}
}
