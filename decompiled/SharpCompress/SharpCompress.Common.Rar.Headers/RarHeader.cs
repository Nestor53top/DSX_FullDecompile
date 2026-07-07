using System;
using System.IO;
using SharpCompress.IO;

namespace SharpCompress.Common.Rar.Headers;

internal class RarHeader
{
	internal const short BaseBlockSize = 7;

	internal const short LONG_BLOCK = short.MinValue;

	protected long ReadBytes { get; private set; }

	protected ushort HeadCRC { get; private set; }

	internal HeaderType HeaderType { get; private set; }

	protected short Flags { get; private set; }

	protected short HeaderSize { get; private set; }

	protected uint AdditionalSize { get; private set; }

	private void FillBase(RarHeader baseHeader)
	{
		HeadCRC = baseHeader.HeadCRC;
		HeaderType = baseHeader.HeaderType;
		Flags = baseHeader.Flags;
		HeaderSize = baseHeader.HeaderSize;
		AdditionalSize = baseHeader.AdditionalSize;
		ReadBytes = baseHeader.ReadBytes;
	}

	internal static RarHeader Create(RarCrcBinaryReader reader)
	{
		try
		{
			RarHeader rarHeader = new RarHeader();
			reader.Mark();
			rarHeader.ReadStartFromReader(reader);
			rarHeader.ReadBytes += reader.CurrentReadByteCount;
			return rarHeader;
		}
		catch (EndOfStreamException)
		{
			return null;
		}
	}

	private void ReadStartFromReader(RarCrcBinaryReader reader)
	{
		HeadCRC = reader.ReadUInt16();
		reader.ResetCrc();
		HeaderType = (HeaderType)(reader.ReadByte() & 0xFF);
		Flags = reader.ReadInt16();
		HeaderSize = reader.ReadInt16();
		if (FlagUtility.HasFlag(Flags, short.MinValue))
		{
			AdditionalSize = reader.ReadUInt32();
		}
	}

	protected virtual void ReadFromReader(MarkingBinaryReader reader)
	{
		throw new NotImplementedException();
	}

	internal T PromoteHeader<T>(RarCrcBinaryReader reader) where T : RarHeader, new()
	{
		T val = new T();
		val.FillBase(this);
		reader.Mark();
		val.ReadFromReader(reader);
		long readBytes = val.ReadBytes + reader.CurrentReadByteCount;
		val.ReadBytes = readBytes;
		int num = val.HeaderSize - (int)val.ReadBytes;
		if (num > 0)
		{
			reader.ReadBytes(num);
		}
		VerifyHeaderCrc(reader.GetCrc());
		return val;
	}

	private void VerifyHeaderCrc(ushort crc)
	{
		if (HeaderType != HeaderType.MarkHeader && crc != HeadCRC)
		{
			throw new InvalidFormatException("rar header crc mismatch");
		}
	}

	protected virtual void PostReadingBytes(MarkingBinaryReader reader)
	{
	}
}
