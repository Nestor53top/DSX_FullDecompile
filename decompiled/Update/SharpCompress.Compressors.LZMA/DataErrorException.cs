using System;

namespace SharpCompress.Compressors.LZMA;

internal class DataErrorException : Exception
{
	public DataErrorException()
		: base("Data Error")
	{
	}
}
