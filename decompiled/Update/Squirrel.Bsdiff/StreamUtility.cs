using System;
using System.IO;

namespace Squirrel.Bsdiff;

internal static class StreamUtility
{
	public static byte[] ReadExactly(this Stream stream, int count)
	{
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		byte[] array = new byte[count];
		ReadExactly(stream, array, 0, count);
		return array;
	}

	public static void ReadExactly(this Stream stream, byte[] buffer, int offset, int count)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (offset < 0 || offset > buffer.Length)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		if (count < 0 || buffer.Length - offset < count)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		while (count > 0)
		{
			int num = stream.Read(buffer, offset, count);
			if (num == 0)
			{
				throw new EndOfStreamException();
			}
			offset += num;
			count -= num;
		}
	}
}
