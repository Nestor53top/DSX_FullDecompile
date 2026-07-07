using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace SharpCompress;

internal static class Utility
{
	public static ReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> items)
	{
		return new ReadOnlyCollection<T>(items.ToList());
	}

	public static int URShift(int number, int bits)
	{
		if (number >= 0)
		{
			return number >> bits;
		}
		return (number >> bits) + (2 << ~bits);
	}

	public static long URShift(long number, int bits)
	{
		if (number >= 0)
		{
			return number >> bits;
		}
		return (number >> bits) + (2L << ~bits);
	}

	public static void Fill<T>(T[] array, int fromindex, int toindex, T val) where T : struct
	{
		if (array.Length == 0)
		{
			throw new NullReferenceException();
		}
		if (fromindex > toindex)
		{
			throw new ArgumentException();
		}
		if (fromindex < 0 || array.Length < toindex)
		{
			throw new IndexOutOfRangeException();
		}
		for (int i = ((fromindex > 0) ? fromindex-- : fromindex); i < toindex; i++)
		{
			array[i] = val;
		}
	}

	public static void Fill<T>(T[] array, T val) where T : struct
	{
		Fill(array, 0, array.Length, val);
	}

	public static void SetSize(this List<byte> list, int count)
	{
		if (count > list.Count)
		{
			for (int i = list.Count; i < count; i++)
			{
				list.Add(0);
			}
		}
		else
		{
			byte[] array = new byte[count];
			list.CopyTo(array, 0);
			list.Clear();
			list.AddRange(array);
		}
	}

	public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> source)
	{
		foreach (T item in source)
		{
			destination.Add(item);
		}
	}

	public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
	{
		foreach (T item in items)
		{
			action(item);
		}
	}

	public static IEnumerable<T> AsEnumerable<T>(this T item)
	{
		yield return item;
	}

	public static void CheckNotNull(this object obj, string name)
	{
		if (obj == null)
		{
			throw new ArgumentNullException(name);
		}
	}

	public static void CheckNotNullOrEmpty(this string obj, string name)
	{
		obj.CheckNotNull(name);
		if (obj.Length == 0)
		{
			throw new ArgumentException("String is empty.");
		}
	}

	public static void Skip(this Stream source, long advanceAmount)
	{
		byte[] array = new byte[32768];
		int num = 0;
		int num2 = 0;
		do
		{
			num2 = array.Length;
			if (num2 > advanceAmount)
			{
				num2 = (int)advanceAmount;
			}
			num = source.Read(array, 0, num2);
			if (num > 0)
			{
				advanceAmount -= num;
				continue;
			}
			break;
		}
		while (advanceAmount != 0L);
	}

	public static void SkipAll(this Stream source)
	{
		byte[] array = new byte[32768];
		while (source.Read(array, 0, array.Length) == array.Length)
		{
		}
	}

	public static DateTime DosDateToDateTime(ushort iDate, ushort iTime)
	{
		int year = iDate / 512 + 1980;
		int num = iDate % 512 / 32;
		int num2 = iDate % 512 % 32;
		int hour = iTime / 2048;
		int minute = iTime % 2048 / 32;
		int second = iTime % 2048 % 32 * 2;
		if (iDate == ushort.MaxValue || num == 0 || num2 == 0)
		{
			year = 1980;
			num = 1;
			num2 = 1;
		}
		if (iTime == ushort.MaxValue)
		{
			hour = (minute = (second = 0));
		}
		DateTime result;
		try
		{
			return new DateTime(year, num, num2, hour, minute, second, DateTimeKind.Local);
		}
		catch
		{
			result = default(DateTime);
		}
		return result;
	}

	public static uint DateTimeToDosTime(this DateTime? dateTime)
	{
		if (!dateTime.HasValue)
		{
			return 0u;
		}
		DateTime dateTime2 = dateTime.Value.ToLocalTime();
		return (uint)((dateTime2.Second / 2) | (dateTime2.Minute << 5) | (dateTime2.Hour << 11) | (dateTime2.Day << 16) | (dateTime2.Month << 21) | (dateTime2.Year - 1980 << 25));
	}

	public static DateTime DosDateToDateTime(uint iTime)
	{
		return DosDateToDateTime((ushort)(iTime / 65536), (ushort)(iTime % 65536));
	}

	public static DateTime DosDateToDateTime(int iTime)
	{
		return DosDateToDateTime((uint)iTime);
	}

	public static long TransferTo(this Stream source, Stream destination)
	{
		byte[] transferByteArray = GetTransferByteArray();
		long num = 0L;
		int count;
		while (ReadTransferBlock(source, transferByteArray, out count))
		{
			num += count;
			destination.Write(transferByteArray, 0, count);
		}
		return num;
	}

	public static long TransferTo(this Stream source, Stream destination, Entry entry, IReaderExtractionListener readerExtractionListener)
	{
		byte[] transferByteArray = GetTransferByteArray();
		int num = 0;
		long num2 = 0L;
		int count;
		while (ReadTransferBlock(source, transferByteArray, out count))
		{
			num2 += count;
			destination.Write(transferByteArray, 0, count);
			num++;
			readerExtractionListener.FireEntryExtractionProgress(entry, num2, num);
		}
		return num2;
	}

	private static bool ReadTransferBlock(Stream source, byte[] array, out int count)
	{
		return (count = source.Read(array, 0, array.Length)) != 0;
	}

	private static byte[] GetTransferByteArray()
	{
		return new byte[81920];
	}

	public static bool ReadFully(this Stream stream, byte[] buffer)
	{
		int num = 0;
		int num2;
		while ((num2 = stream.Read(buffer, num, buffer.Length - num)) > 0)
		{
			num += num2;
			if (num >= buffer.Length)
			{
				return true;
			}
		}
		return num >= buffer.Length;
	}

	public static string TrimNulls(this string source)
	{
		return source.Replace('\0', ' ').Trim();
	}

	public static bool BinaryEquals(this byte[] source, byte[] target)
	{
		if (source.Length != target.Length)
		{
			return false;
		}
		for (int i = 0; i < source.Length; i++)
		{
			if (source[i] != target[i])
			{
				return false;
			}
		}
		return true;
	}

	public static void CopyTo(this byte[] array, byte[] destination, int index)
	{
		Array.Copy(array, 0, destination, index, array.Length);
	}
}
