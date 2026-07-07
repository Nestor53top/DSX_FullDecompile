using System;
using System.IO;
using System.Text;
using SharpCompress.Converters;

namespace SharpCompress.Common.Tar.Headers;

internal class TarHeader
{
	internal static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	internal const int BlockSize = 512;

	internal string Name { get; set; }

	internal long Size { get; set; }

	internal DateTime LastModifiedTime { get; set; }

	internal EntryType EntryType { get; set; }

	internal Stream PackedStream { get; set; }

	public long? DataStartPosition { get; set; }

	public string Magic { get; set; }

	internal void Write(Stream output)
	{
		byte[] array = new byte[512];
		WriteOctalBytes(511L, array, 100, 8);
		WriteOctalBytes(0L, array, 108, 8);
		WriteOctalBytes(0L, array, 116, 8);
		if (Name.Length > 100)
		{
			WriteStringBytes("././@LongLink", array, 0, 100);
			array[156] = 76;
			WriteOctalBytes(Name.Length + 1, array, 124, 12);
		}
		else
		{
			WriteStringBytes(Name, array, 0, 100);
			WriteOctalBytes(Size, array, 124, 12);
			WriteOctalBytes((long)(LastModifiedTime.ToUniversalTime() - Epoch).TotalSeconds, array, 136, 12);
			array[156] = (byte)EntryType;
			if (Size >= 8589934591L)
			{
				byte[] bytes = DataConverter.BigEndian.GetBytes(Size);
				byte[] array2 = new byte[12];
				bytes.CopyTo(array2, 12 - bytes.Length);
				array2[0] |= 128;
				array2.CopyTo(array, 124);
			}
		}
		WriteOctalBytes(RecalculateChecksum(array), array, 148, 8);
		output.Write(array, 0, array.Length);
		if (Name.Length > 100)
		{
			WriteLongFilenameHeader(output);
			Name = Name.Substring(0, 100);
			Write(output);
		}
	}

	private void WriteLongFilenameHeader(Stream output)
	{
		byte[] bytes = ArchiveEncoding.Default.GetBytes(Name);
		output.Write(bytes, 0, bytes.Length);
		int num = 512 - bytes.Length % 512;
		if (num == 0)
		{
			num = 512;
		}
		output.Write(new byte[num], 0, num);
	}

	internal bool Read(BinaryReader reader)
	{
		byte[] array = ReadBlock(reader);
		if (array.Length == 0)
		{
			return false;
		}
		if (ReadEntryType(array) == EntryType.LongName)
		{
			Name = ReadLongName(reader, array);
			array = ReadBlock(reader);
		}
		else
		{
			Name = ArchiveEncoding.Default.GetString(array, 0, 100).TrimNulls();
		}
		EntryType = ReadEntryType(array);
		Size = ReadSize(array);
		long num = ReadASCIIInt64Base8(array, 136, 11);
		DateTime epoch = Epoch;
		LastModifiedTime = epoch.AddSeconds(num).ToLocalTime();
		Magic = ArchiveEncoding.Default.GetString(array, 257, 6).TrimNulls();
		if (!string.IsNullOrEmpty(Magic) && "ustar".Equals(Magic))
		{
			string source = ArchiveEncoding.Default.GetString(array, 345, 157);
			source = source.TrimNulls();
			if (!string.IsNullOrEmpty(source))
			{
				Name = source + "/" + Name;
			}
		}
		if (EntryType != EntryType.LongName && Name.Length == 0)
		{
			return false;
		}
		return true;
	}

	private string ReadLongName(BinaryReader reader, byte[] buffer)
	{
		int num = (int)ReadSize(buffer);
		byte[] array = reader.ReadBytes(num);
		int num2 = 512 - num % 512;
		if (num2 < 512)
		{
			reader.ReadBytes(num2);
		}
		return ArchiveEncoding.Default.GetString(array, 0, array.Length).TrimNulls();
	}

	private static EntryType ReadEntryType(byte[] buffer)
	{
		return (EntryType)buffer[156];
	}

	private long ReadSize(byte[] buffer)
	{
		if ((buffer[124] & 0x80) == 128)
		{
			return DataConverter.BigEndian.GetInt64(buffer, 128);
		}
		return ReadASCIIInt64Base8(buffer, 124, 11);
	}

	private static byte[] ReadBlock(BinaryReader reader)
	{
		byte[] array = reader.ReadBytes(512);
		if (array.Length != 0 && array.Length < 512)
		{
			throw new InvalidOperationException("Buffer is invalid size");
		}
		return array;
	}

	private static void WriteStringBytes(string name, byte[] buffer, int offset, int length)
	{
		int i;
		for (i = 0; i < length - 1 && i < name.Length; i++)
		{
			buffer[offset + i] = (byte)name[i];
		}
		for (; i < length; i++)
		{
			buffer[offset + i] = 0;
		}
	}

	private static void WriteOctalBytes(long value, byte[] buffer, int offset, int length)
	{
		string text = Convert.ToString(value, 8);
		int num = length - text.Length - 1;
		for (int i = 0; i < num; i++)
		{
			buffer[offset + i] = 32;
		}
		for (int j = 0; j < text.Length; j++)
		{
			buffer[offset + j + num] = (byte)text[j];
		}
	}

	private static int ReadASCIIInt32Base8(byte[] buffer, int offset, int count)
	{
		string value = Encoding.UTF8.GetString(buffer, offset, count).TrimNulls();
		if (string.IsNullOrEmpty(value))
		{
			return 0;
		}
		return Convert.ToInt32(value, 8);
	}

	private static long ReadASCIIInt64Base8(byte[] buffer, int offset, int count)
	{
		string value = Encoding.UTF8.GetString(buffer, offset, count).TrimNulls();
		if (string.IsNullOrEmpty(value))
		{
			return 0L;
		}
		return Convert.ToInt64(value, 8);
	}

	private static long ReadASCIIInt64(byte[] buffer, int offset, int count)
	{
		string value = Encoding.UTF8.GetString(buffer, offset, count).TrimNulls();
		if (string.IsNullOrEmpty(value))
		{
			return 0L;
		}
		return Convert.ToInt64(value);
	}

	internal static int RecalculateChecksum(byte[] buf)
	{
		Encoding.UTF8.GetBytes("        ").CopyTo(buf, 148);
		int num = 0;
		foreach (byte b in buf)
		{
			num += b;
		}
		return num;
	}

	internal static int RecalculateAltChecksum(byte[] buf)
	{
		Encoding.UTF8.GetBytes("        ").CopyTo(buf, 148);
		int num = 0;
		foreach (byte b in buf)
		{
			num = (((b & 0x80) != 128) ? (num + b) : (num - (b ^ 0x80)));
		}
		return num;
	}
}
