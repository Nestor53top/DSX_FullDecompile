using System.Runtime.CompilerServices;

namespace System;

internal struct NUInt
{
	private unsafe readonly void* _value = (void*)value;

	private unsafe NUInt(uint value)
	{
	}

	private unsafe NUInt(ulong value)
	{
	}

	public static implicit operator System.NUInt(uint value)
	{
		return new System.NUInt(value);
	}

	public unsafe static implicit operator IntPtr(System.NUInt value)
	{
		return (IntPtr)value._value;
	}

	public static explicit operator System.NUInt(int value)
	{
		return new System.NUInt((uint)value);
	}

	public unsafe static explicit operator void*(System.NUInt value)
	{
		return value._value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static System.NUInt operator *(System.NUInt left, System.NUInt right)
	{
		if (sizeof(IntPtr) != 4)
		{
			return new System.NUInt((ulong)left._value * (ulong)right._value);
		}
		return new System.NUInt((uint)((int)left._value * (int)right._value));
	}
}
