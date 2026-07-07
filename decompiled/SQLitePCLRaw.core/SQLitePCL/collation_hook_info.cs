using System;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class collation_hook_info
{
	private delegate_collation _func;

	private object _user_data;

	public collation_hook_info(delegate_collation func, object v)
	{
		_func = func;
		_user_data = v;
	}

	public static collation_hook_info from_ptr(IntPtr p)
	{
		return ((GCHandle)p).Target as collation_hook_info;
	}

	public int call(ReadOnlySpan<byte> s1, ReadOnlySpan<byte> s2)
	{
		return _func(_user_data, s1, s2);
	}
}
