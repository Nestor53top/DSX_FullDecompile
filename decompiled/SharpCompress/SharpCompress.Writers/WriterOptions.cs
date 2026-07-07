using SharpCompress.Common;

namespace SharpCompress.Writers;

public class WriterOptions : OptionsBase
{
	public CompressionType CompressionType { get; set; }

	public WriterOptions(CompressionType compressionType)
	{
		CompressionType = compressionType;
	}

	public static implicit operator WriterOptions(CompressionType compressionType)
	{
		return new WriterOptions(compressionType);
	}
}
