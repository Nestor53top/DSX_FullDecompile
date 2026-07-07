using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Common.Zip;
using SharpCompress.Common.Zip.Headers;

namespace SharpCompress.Readers.Zip;

internal class ZipReader : AbstractReader<ZipEntry, ZipVolume>
{
	private readonly StreamingZipHeaderFactory headerFactory;

	public override ZipVolume Volume { get; }

	internal ZipReader(Stream stream, ReaderOptions options)
		: base(options, ArchiveType.Zip)
	{
		Volume = new ZipVolume(stream, options);
		headerFactory = new StreamingZipHeaderFactory(options.Password);
	}

	public static ZipReader Open(Stream stream, ReaderOptions options = null)
	{
		stream.CheckNotNull("stream");
		return new ZipReader(stream, options ?? new ReaderOptions());
	}

	internal override IEnumerable<ZipEntry> GetEntries(Stream stream)
	{
		foreach (ZipHeader item in headerFactory.ReadStreamHeader(stream))
		{
			if (item != null)
			{
				switch (item.ZipHeaderType)
				{
				case ZipHeaderType.LocalEntry:
					yield return new ZipEntry(new StreamingZipFilePart(item as LocalEntryHeader, stream));
					break;
				case ZipHeaderType.DirectoryEnd:
					yield break;
				}
			}
		}
	}
}
