using System;
using System.IO;

namespace SharpCompress.Compressors.Xz;

internal static class MultiByteIntegers
{
	public static ulong ReadXZInteger(this BinaryReader reader, int MaxBytes = 9)
	{
		if (MaxBytes <= 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (MaxBytes > 9)
		{
			MaxBytes = 9;
		}
		byte b = reader.ReadByte();
		ulong num = (ulong)b & 0x7FuL;
		int num2 = 0;
		while ((b & 0x80) != 0)
		{
			if (num2 >= MaxBytes)
			{
				throw new InvalidDataException();
			}
			b = reader.ReadByte();
			if (b == 0)
			{
				throw new InvalidDataException();
			}
			num |= (ulong)((long)(b & 0x7F) << num2 * 7);
		}
		return num;
	}
}
