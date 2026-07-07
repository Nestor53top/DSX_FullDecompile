using System.Collections.Generic;

namespace SharpCompress.Compressors.Xz;

internal static class Crc64
{
	public const ulong DefaultSeed = 0uL;

	internal static ulong[] Table;

	public const ulong Iso3309Polynomial = 15564440312192434176uL;

	public static ulong Compute(byte[] buffer)
	{
		return Compute(0uL, buffer);
	}

	public static ulong Compute(ulong seed, byte[] buffer)
	{
		if (Table == null)
		{
			Table = CreateTable(15564440312192434176uL);
		}
		return CalculateHash(seed, Table, buffer, 0, buffer.Length);
	}

	public static ulong CalculateHash(ulong seed, ulong[] table, IList<byte> buffer, int start, int size)
	{
		ulong num = seed;
		for (int i = start; i < size; i++)
		{
			num = (num >> 8) ^ table[(buffer[i] ^ num) & 0xFF];
		}
		return num;
	}

	public static ulong[] CreateTable(ulong polynomial)
	{
		ulong[] array = new ulong[256];
		for (int i = 0; i < 256; i++)
		{
			ulong num = (ulong)i;
			for (int j = 0; j < 8; j++)
			{
				num = (((num & 1) != 1) ? (num >> 1) : ((num >> 1) ^ polynomial));
			}
			array[i] = num;
		}
		return array;
	}
}
