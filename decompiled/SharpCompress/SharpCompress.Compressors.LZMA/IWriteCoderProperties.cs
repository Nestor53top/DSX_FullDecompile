using System.IO;

namespace SharpCompress.Compressors.LZMA;

internal interface IWriteCoderProperties
{
	void WriteCoderProperties(Stream outStream);
}
