using System;
using System.IO;

namespace SharpCompress.Compressors.Xz;

internal static class BinaryUtils
{
	public static int ReadLittleEndianInt32(this BinaryReader reader)
	{
		byte[] array = reader.ReadBytes(4);
		return array[0] + (array[1] << 8) + (array[2] << 16) + (array[3] << 24);
	}

	internal static uint ReadLittleEndianUInt32(this BinaryReader reader)
	{
		return (uint)reader.ReadLittleEndianInt32();
	}

	public static int ReadLittleEndianInt32(this Stream stream)
	{
		byte[] array = new byte[4];
		if (stream.Read(array, 0, 4) != 4)
		{
			throw new EndOfStreamException();
		}
		return array[0] + (array[1] << 8) + (array[2] << 16) + (array[3] << 24);
	}

	internal static uint ReadLittleEndianUInt32(this Stream stream)
	{
		return (uint)stream.ReadLittleEndianInt32();
	}

	internal static byte[] ToBigEndianBytes(this uint uint32)
	{
		byte[] bytes = BitConverter.GetBytes(uint32);
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse((Array)bytes);
		}
		return bytes;
	}

	internal static byte[] ToLittleEndianBytes(this uint uint32)
	{
		byte[] bytes = BitConverter.GetBytes(uint32);
		if (!BitConverter.IsLittleEndian)
		{
			Array.Reverse((Array)bytes);
		}
		return bytes;
	}
}
