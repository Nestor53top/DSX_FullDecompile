using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SQLitePCL;

internal static class util
{
	public static utf8z to_utf8z(this string s)
	{
		return utf8z.FromString(s);
	}

	public static byte[] to_utf8_with_z(this string sourceText)
	{
		if (sourceText == null)
		{
			return null;
		}
		byte[] array = new byte[Encoding.UTF8.GetByteCount(sourceText) + 1];
		int bytes = Encoding.UTF8.GetBytes(sourceText, 0, sourceText.Length, array, 0);
		array[bytes] = 0;
		return array;
	}

	private static int my_strlen(IntPtr nativeString)
	{
		int i = 0;
		if (nativeString != IntPtr.Zero)
		{
			for (; Marshal.ReadByte(nativeString, i) > 0; i++)
			{
			}
		}
		return i;
	}

	public static string from_utf8_z(IntPtr nativeString)
	{
		return from_utf8(nativeString, my_strlen(nativeString));
	}

	public unsafe static string from_utf8(IntPtr nativeString, int size)
	{
		string result = null;
		if (nativeString != IntPtr.Zero)
		{
			result = Encoding.UTF8.GetString((byte*)nativeString.ToPointer(), size);
		}
		return result;
	}
}
