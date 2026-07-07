using System;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class progress_hook_info
{
	private delegate_progress _func;

	private object _user_data;

	public progress_hook_info(delegate_progress func, object v)
	{
		_func = func;
		_user_data = v;
	}

	public static progress_hook_info from_ptr(IntPtr p)
	{
		return ((GCHandle)p).Target as progress_hook_info;
	}

	public int call()
	{
		return _func(_user_data);
	}
}
