using System.Collections.Generic;
using System.IO;
using SharpCompress.Common.Tar.Headers;
using SharpCompress.IO;

namespace SharpCompress.Common.Tar;

internal static class TarHeaderFactory
{
	internal static IEnumerable<TarHeader> ReadHeader(StreamingMode mode, Stream stream)
	{
		while (true)
		{
			TarHeader tarHeader;
			try
			{
				BinaryReader binaryReader = new BinaryReader(stream);
				tarHeader = new TarHeader();
				if (!tarHeader.Read(binaryReader))
				{
					break;
				}
				switch (mode)
				{
				case StreamingMode.Seekable:
					tarHeader.DataStartPosition = binaryReader.BaseStream.Position;
					binaryReader.BaseStream.Position += PadTo512(tarHeader.Size);
					break;
				case StreamingMode.Streaming:
					tarHeader.PackedStream = new TarReadOnlySubStream(stream, tarHeader.Size);
					break;
				default:
					throw new InvalidFormatException("Invalid StreamingMode");
				}
			}
			catch
			{
				tarHeader = null;
			}
			yield return tarHeader;
		}
	}

	private static long PadTo512(long size)
	{
		int num = (int)(size % 512);
		if (num == 0)
		{
			return size;
		}
		return 512 - num + size;
	}
}
