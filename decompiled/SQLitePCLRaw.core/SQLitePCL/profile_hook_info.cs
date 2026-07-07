using System;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class profile_hook_info
{
	private delegate_profile _func;

	private object _user_data;

	public profile_hook_info(delegate_profile func, object v)
	{
		_func = func;
		_user_data = v;
	}

	public static profile_hook_info from_ptr(IntPtr p)
	{
		return ((GCHandle)p).Target as profile_hook_info;
	}

	public void call(utf8z s, long elapsed)
	{
		_func(_user_data, s, elapsed);
	}
}
