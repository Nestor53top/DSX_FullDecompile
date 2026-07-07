using System;

namespace SharpCompress.Compressors.Deflate;

public class ZlibException : Exception
{
	public ZlibException()
	{
	}

	public ZlibException(string s)
		: base(s)
	{
	}
}
