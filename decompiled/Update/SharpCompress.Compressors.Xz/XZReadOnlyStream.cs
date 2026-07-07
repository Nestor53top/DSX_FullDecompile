using System.IO;

namespace SharpCompress.Compressors.Xz;

internal abstract class XZReadOnlyStream : ReadOnlyStream
{
	public XZReadOnlyStream(Stream stream)
	{
		base.BaseStream = stream;
		if (!base.BaseStream.CanRead)
		{
			throw new InvalidDataException("Must be able to read from stream");
		}
	}
}
