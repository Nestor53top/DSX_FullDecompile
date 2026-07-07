using System;
using System.IO;
using System.Text;
using SharpCompress.Common.Zip;
using SharpCompress.Common.Zip.Headers;
using SharpCompress.Converters;

namespace SharpCompress.Writers.Zip;

internal class ZipCentralDirectoryEntry
{
	private readonly ZipCompressionMethod compression;

	private readonly string fileName;

	internal DateTime? ModificationTime { get; set; }

	internal string Comment { get; set; }

	internal uint Crc { get; set; }

	internal ulong Compressed { get; set; }

	internal ulong Decompressed { get; set; }

	internal ushort Zip64HeaderOffset { get; set; }

	internal ulong HeaderOffset { get; }

	public ZipCentralDirectoryEntry(ZipCompressionMethod compression, string fileName, ulong headerOffset)
	{
		this.compression = compression;
		this.fileName = fileName;
		HeaderOffset = headerOffset;
	}

	internal uint Write(Stream outputStream)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(fileName);
		byte[] bytes2 = Encoding.UTF8.GetBytes(Comment);
		bool flag = Compressed >= uint.MaxValue || Decompressed >= uint.MaxValue;
		bool flag2 = flag || HeaderOffset >= uint.MaxValue || Zip64HeaderOffset != 0;
		uint value = (uint)(flag2 ? uint.MaxValue : Compressed);
		uint value2 = (uint)(flag2 ? uint.MaxValue : Decompressed);
		uint value3 = (uint)(flag2 ? uint.MaxValue : HeaderOffset);
		int num = (flag2 ? 32 : 0);
		byte b = (byte)(flag2 ? 45u : 20u);
		HeaderFlags headerFlags = HeaderFlags.UTF8;
		if (!outputStream.CanSeek)
		{
			if (!flag)
			{
				headerFlags |= HeaderFlags.UsePostDataDescriptor;
			}
			if (compression == ZipCompressionMethod.LZMA)
			{
				headerFlags |= HeaderFlags.Bit1;
			}
		}
		byte[] obj = new byte[8] { 80, 75, 1, 2, 0, 0, 0, 0 };
		obj[4] = b;
		obj[6] = b;
		outputStream.Write(obj, 0, 8);
		outputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)headerFlags), 0, 2);
		outputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)compression), 0, 2);
		outputStream.Write(DataConverter.LittleEndian.GetBytes(ModificationTime.DateTimeToDosTime()), 0, 4);
		outputStream.Write(DataConverter.LittleEndian.GetBytes(Crc), 0, 4);
		outputStream.Write(DataConverter.LittleEndian.GetBytes(value), 0, 4);
		outputStream.Write(DataConverter.LittleEndian.GetBytes(value2), 0, 4);
		outputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)bytes.Length), 0, 2);
		outputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)num), 0, 2);
		outputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)bytes2.Length), 0, 2);
		outputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)0), 0, 2);
		outputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)0), 0, 2);
		outputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)0), 0, 2);
		outputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)33024), 0, 2);
		outputStream.Write(DataConverter.LittleEndian.GetBytes(value3), 0, 4);
		outputStream.Write(bytes, 0, bytes.Length);
		if (flag2)
		{
			outputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)1), 0, 2);
			outputStream.Write(DataConverter.LittleEndian.GetBytes((ushort)(num - 4)), 0, 2);
			outputStream.Write(DataConverter.LittleEndian.GetBytes(Decompressed), 0, 8);
			outputStream.Write(DataConverter.LittleEndian.GetBytes(Compressed), 0, 8);
			outputStream.Write(DataConverter.LittleEndian.GetBytes(HeaderOffset), 0, 8);
			outputStream.Write(DataConverter.LittleEndian.GetBytes(0), 0, 4);
		}
		outputStream.Write(bytes2, 0, bytes2.Length);
		return (uint)(46 + bytes.Length + num + bytes2.Length);
	}
}
