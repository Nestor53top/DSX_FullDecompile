using System.IO;
using SharpCompress.Readers;

namespace SharpCompress.Common.GZip;

internal class GZipVolume : Volume
{
	public override bool IsFirstVolume => true;

	public override bool IsMultiVolume => true;

	public GZipVolume(Stream stream, ReaderOptions options)
		: base(stream, options)
	{
	}

	public GZipVolume(FileInfo fileInfo, ReaderOptions options)
		: base(fileInfo.OpenRead(), options)
	{
		options.LeaveStreamOpen = false;
	}
}
