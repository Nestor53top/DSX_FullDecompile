using System;

namespace SharpCompress.Compressors.LZMA;

internal class InvalidParamException : Exception
{
	public InvalidParamException()
		: base("Invalid Parameter")
	{
	}
}
