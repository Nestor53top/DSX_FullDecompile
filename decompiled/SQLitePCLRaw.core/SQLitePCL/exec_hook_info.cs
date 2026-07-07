using System;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class exec_hook_info
{
	private delegate_exec _func;

	private object _user_data;

	public exec_hook_info(delegate_exec func, object v)
	{
		_func = func;
		_user_data = v;
	}

	public static exec_hook_info from_ptr(IntPtr p)
	{
		return ((GCHandle)p).Target as exec_hook_info;
	}

	public int call(int n, IntPtr values_ptr, IntPtr names_ptr)
	{
		IntPtr[] array = new IntPtr[n];
		IntPtr[] array2 = new IntPtr[n];
		int num = Marshal.SizeOf(typeof(IntPtr));
		for (int i = 0; i < n; i++)
		{
			IntPtr intPtr = Marshal.ReadIntPtr(values_ptr, i * num);
			array[i] = intPtr;
			intPtr = Marshal.ReadIntPtr(names_ptr, i * num);
			array2[i] = intPtr;
		}
		return _func(_user_data, array, array2);
	}
}
