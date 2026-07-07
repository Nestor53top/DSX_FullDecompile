using System.IO;
using SharpCompress.Readers;

namespace SharpCompress.Common.Zip;

internal class ZipVolume : Volume
{
	public string Comment { get; internal set; }

	public ZipVolume(Stream stream, ReaderOptions readerOptions)
		: base(stream, readerOptions)
	{
	}
}
