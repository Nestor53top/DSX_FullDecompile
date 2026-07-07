using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Common.GZip;

namespace SharpCompress.Readers.GZip;

internal class GZipReader : AbstractReader<GZipEntry, GZipVolume>
{
	public override GZipVolume Volume { get; }

	internal GZipReader(Stream stream, ReaderOptions options)
		: base(options, ArchiveType.GZip)
	{
		Volume = new GZipVolume(stream, options);
	}

	public static GZipReader Open(Stream stream, ReaderOptions options = null)
	{
		stream.CheckNotNull("stream");
		return new GZipReader(stream, options ?? new ReaderOptions());
	}

	internal override IEnumerable<GZipEntry> GetEntries(Stream stream)
	{
		return GZipEntry.GetEntries(stream);
	}
}
