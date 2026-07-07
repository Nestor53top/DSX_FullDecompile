using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NuGet;

internal static class StreamExtensions
{
	public static byte[] ReadAllBytes(this Stream stream)
	{
		if (stream is MemoryStream memoryStream)
		{
			return memoryStream.ToArray();
		}
		MemoryStream memoryStream2;
		using (memoryStream2 = new MemoryStream())
		{
			stream.CopyTo(memoryStream2);
			return memoryStream2.ToArray();
		}
	}

	public static Func<Stream> ToStreamFactory(this Stream stream)
	{
		byte[] buffer;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			try
			{
				stream.CopyTo(memoryStream);
				buffer = memoryStream.ToArray();
			}
			finally
			{
				stream.Close();
			}
		}
		return () => new MemoryStream(buffer);
	}

	public static string ReadToEnd(this Stream stream)
	{
		using StreamReader streamReader = new StreamReader(stream);
		return streamReader.ReadToEnd();
	}

	public static Stream AsStream(this string value)
	{
		return value.AsStream(Encoding.UTF8);
	}

	public static Stream AsStream(this string value, Encoding encoding)
	{
		return new MemoryStream(encoding.GetBytes(value));
	}

	public static bool ContentEquals(this Stream stream, Stream otherStream)
	{
		bool num = IsBinary(otherStream);
		otherStream.Seek(0L, SeekOrigin.Begin);
		if (!num)
		{
			return CompareText(stream, otherStream);
		}
		return CompareBinary(stream, otherStream);
	}

	public static bool IsBinary(Stream stream)
	{
		byte[] array = new byte[30];
		int count = stream.Read(array, 0, 30);
		return Array.FindIndex(array, 0, count, (byte d) => d == 0) >= 0;
	}

	private static bool CompareText(Stream stream, Stream otherStream)
	{
		IEnumerable<string> first = ReadStreamLines(stream);
		IEnumerable<string> second = ReadStreamLines(otherStream);
		return first.SequenceEqual<string>(second, StringComparer.Ordinal);
	}

	private static IEnumerable<string> ReadStreamLines(Stream stream)
	{
		using StreamReader reader = new StreamReader(stream);
		bool hasSeenBeginLine = false;
		while (reader.Peek() != -1)
		{
			string text = reader.ReadLine();
			if (text.IndexOf(Constants.EndIgnoreMarker, StringComparison.OrdinalIgnoreCase) > -1)
			{
				hasSeenBeginLine = false;
			}
			else if (text.IndexOf(Constants.BeginIgnoreMarker, StringComparison.OrdinalIgnoreCase) > -1)
			{
				hasSeenBeginLine = true;
			}
			else if (!hasSeenBeginLine)
			{
				yield return text;
			}
		}
	}

	private static bool CompareBinary(Stream stream, Stream otherStream)
	{
		if (stream.CanSeek && otherStream.CanSeek && stream.Length != otherStream.Length)
		{
			return false;
		}
		byte[] array = new byte[4096];
		byte[] array2 = new byte[4096];
		int num = 0;
		do
		{
			num = stream.Read(array, 0, array.Length);
			if (num <= 0)
			{
				continue;
			}
			int num2 = otherStream.Read(array2, 0, num);
			if (num != num2)
			{
				return false;
			}
			for (int i = 0; i < num; i++)
			{
				if (array[i] != array2[i])
				{
					return false;
				}
			}
		}
		while (num > 0);
		return true;
	}
}
