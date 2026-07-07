using System.IO;
using SharpCompress.Readers;

namespace SharpCompress.Common.SevenZip;

internal class SevenZipVolume : Volume
{
	public SevenZipVolume(Stream stream, ReaderOptions readerFactoryOptions)
		: base(stream, readerFactoryOptions)
	{
	}
}
