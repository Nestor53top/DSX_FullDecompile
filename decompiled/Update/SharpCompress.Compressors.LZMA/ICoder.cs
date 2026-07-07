using System.IO;

namespace SharpCompress.Compressors.LZMA;

internal interface ICoder
{
	void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress);
}
