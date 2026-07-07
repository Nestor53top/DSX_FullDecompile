using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace HidSharp.Platform;

internal sealed class Utf8Marshaler : ICustomMarshaler
{
	[ThreadStatic]
	private static HashSet<IntPtr> _allocations;

	private static HashSet<IntPtr> GetAllocations()
	{
		if (_allocations == null)
		{
			_allocations = new HashSet<IntPtr>();
		}
		return _allocations;
	}

	public void CleanUpManagedData(object obj)
	{
	}

	public void CleanUpNativeData(IntPtr ptr)
	{
		HashSet<IntPtr> allocations = GetAllocations();
		if (!(IntPtr.Zero == ptr) && allocations.Contains(ptr))
		{
			Marshal.FreeHGlobal(ptr);
			allocations.Remove(ptr);
		}
	}

	public int GetNativeDataSize()
	{
		return -1;
	}

	public IntPtr MarshalManagedToNative(object obj)
	{
		if (!(obj is string s))
		{
			return IntPtr.Zero;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		IntPtr intPtr = Marshal.AllocHGlobal(bytes.Length + 1);
		Marshal.Copy(bytes, 0, intPtr, bytes.Length);
		Marshal.WriteByte(intPtr, bytes.Length, 0);
		HashSet<IntPtr> allocations = GetAllocations();
		allocations.Add(intPtr);
		return intPtr;
	}

	public object MarshalNativeToManaged(IntPtr ptr)
	{
		if (ptr == IntPtr.Zero)
		{
			return null;
		}
		int i;
		for (i = 0; Marshal.ReadByte(ptr, i) != 0; i++)
		{
		}
		byte[] array = new byte[i];
		Marshal.Copy(ptr, array, 0, array.Length);
		return Encoding.UTF8.GetString(array);
	}

	[Obfuscation(Exclude = true)]
	public static ICustomMarshaler GetInstance(string cookie)
	{
		return new Utf8Marshaler();
	}
}
