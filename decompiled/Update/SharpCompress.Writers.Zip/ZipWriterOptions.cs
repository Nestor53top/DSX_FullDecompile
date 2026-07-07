using SharpCompress.Common;
using SharpCompress.Compressors.Deflate;

namespace SharpCompress.Writers.Zip;

internal class ZipWriterOptions : WriterOptions
{
	public CompressionLevel DeflateCompressionLevel { get; set; } = CompressionLevel.Default;

	public string ArchiveComment { get; set; }

	public bool UseZip64 { get; set; }

	public ZipWriterOptions(CompressionType compressionType)
		: base(compressionType)
	{
	}

	internal ZipWriterOptions(WriterOptions options)
		: base(options.CompressionType)
	{
		base.LeaveStreamOpen = options.LeaveStreamOpen;
		if (options is ZipWriterOptions)
		{
			UseZip64 = ((ZipWriterOptions)options).UseZip64;
		}
	}
}
