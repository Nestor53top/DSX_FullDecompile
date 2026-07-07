using System;

namespace SharpCompress.Compressors.Deflate;

internal class ZlibException : Exception
{
	public ZlibException()
	{
	}

	public ZlibException(string s)
		: base(s)
	{
	}
}
