using System;
using System.Text;

namespace SQLitePCL;

public readonly ref struct utf8z
{
	private readonly ReadOnlySpan<byte> sp;

	public ref readonly byte GetPinnableReference()
	{
		return ref sp.GetPinnableReference();
	}

	private utf8z(ReadOnlySpan<byte> a)
	{
		sp = a;
	}

	private static utf8z FromSpan(ReadOnlySpan<byte> a)
	{
		if (a.Length > 0 && a[a.Length - 1] != 0)
		{
			throw new ArgumentException("zero terminator required");
		}
		return new utf8z(a);
	}

	public static utf8z FromString(string s)
	{
		if (s == null)
		{
			return new utf8z(ReadOnlySpan<byte>.Empty);
		}
		return new utf8z(s.to_utf8_with_z());
	}

	private unsafe static long my_strlen(byte* p)
	{
		byte* ptr;
		for (ptr = p; *ptr != 0; ptr++)
		{
		}
		return ptr - p;
	}

	private unsafe static ReadOnlySpan<byte> find_zero_terminator(byte* p)
	{
		int num = (int)my_strlen(p);
		return new ReadOnlySpan<byte>(p, num + 1);
	}

	public unsafe static utf8z FromPtr(byte* p)
	{
		if (p == null)
		{
			return new utf8z(ReadOnlySpan<byte>.Empty);
		}
		return new utf8z(find_zero_terminator(p));
	}

	public unsafe static utf8z FromPtrLen(byte* p, int len)
	{
		if (p == null)
		{
			return new utf8z(ReadOnlySpan<byte>.Empty);
		}
		return FromSpan(new ReadOnlySpan<byte>(p, len + 1));
	}

	public unsafe static utf8z FromIntPtr(IntPtr p)
	{
		if (p == IntPtr.Zero)
		{
			return new utf8z(ReadOnlySpan<byte>.Empty);
		}
		return new utf8z(find_zero_terminator((byte*)p.ToPointer()));
	}

	public unsafe string utf8_to_string()
	{
		if (sp.Length == 0)
		{
			return null;
		}
		fixed (byte* bytes = sp)
		{
			return Encoding.UTF8.GetString(bytes, sp.Length - 1);
		}
	}
}
