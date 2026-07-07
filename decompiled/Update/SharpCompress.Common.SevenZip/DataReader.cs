using System;
using System.IO;
using System.Text;

namespace SharpCompress.Common.SevenZip;

internal class DataReader
{
	private readonly byte[] _buffer;

	private readonly int _ending;

	public int Offset { get; private set; }

	public static uint Get32(byte[] buffer, int offset)
	{
		return (uint)(buffer[offset] + (buffer[offset + 1] << 8) + (buffer[offset + 2] << 16) + (buffer[offset + 3] << 24));
	}

	public static ulong Get64(byte[] buffer, int offset)
	{
		return buffer[offset] + ((ulong)buffer[offset + 1] << 8) + ((ulong)buffer[offset + 2] << 16) + ((ulong)buffer[offset + 3] << 24) + ((ulong)buffer[offset + 4] << 32) + ((ulong)buffer[offset + 5] << 40) + ((ulong)buffer[offset + 6] << 48) + ((ulong)buffer[offset + 7] << 56);
	}

	public DataReader(byte[] buffer, int offset, int length)
	{
		_buffer = buffer;
		Offset = offset;
		_ending = offset + length;
	}

	public byte ReadByte()
	{
		if (Offset >= _ending)
		{
			throw new EndOfStreamException();
		}
		return _buffer[Offset++];
	}

	public void ReadBytes(byte[] buffer, int offset, int length)
	{
		if (length > _ending - Offset)
		{
			throw new EndOfStreamException();
		}
		while (length-- > 0)
		{
			buffer[offset++] = _buffer[Offset++];
		}
	}

	public void SkipData(long size)
	{
		if (size > _ending - Offset)
		{
			throw new EndOfStreamException();
		}
		Offset += (int)size;
	}

	public void SkipData()
	{
		SkipData(checked((long)ReadNumber()));
	}

	public ulong ReadNumber()
	{
		if (Offset >= _ending)
		{
			throw new EndOfStreamException();
		}
		byte b = _buffer[Offset++];
		byte b2 = 128;
		ulong num = 0uL;
		for (int i = 0; i < 8; i++)
		{
			if ((b & b2) == 0)
			{
				ulong num2 = (uint)(b & (b2 - 1));
				return num + (num2 << i * 8);
			}
			if (Offset >= _ending)
			{
				throw new EndOfStreamException();
			}
			num |= (ulong)_buffer[Offset++] << 8 * i;
			b2 >>= 1;
		}
		return num;
	}

	public int ReadNum()
	{
		ulong num = ReadNumber();
		if (num > int.MaxValue)
		{
			throw new NotSupportedException();
		}
		return (int)num;
	}

	public uint ReadUInt32()
	{
		if (Offset + 4 > _ending)
		{
			throw new EndOfStreamException();
		}
		uint result = Get32(_buffer, Offset);
		Offset += 4;
		return result;
	}

	public ulong ReadUInt64()
	{
		if (Offset + 8 > _ending)
		{
			throw new EndOfStreamException();
		}
		ulong result = Get64(_buffer, Offset);
		Offset += 8;
		return result;
	}

	public string ReadString()
	{
		int num = Offset;
		while (true)
		{
			if (num + 2 > _ending)
			{
				throw new EndOfStreamException();
			}
			if (_buffer[num] == 0 && _buffer[num + 1] == 0)
			{
				break;
			}
			num += 2;
		}
		string result = Encoding.Unicode.GetString(_buffer, Offset, num - Offset);
		Offset = num + 2;
		return result;
	}
}
