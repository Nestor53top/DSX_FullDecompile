using System;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class rollback_hook_info
{
	private delegate_rollback _func;

	private object _user_data;

	public rollback_hook_info(delegate_rollback func, object v)
	{
		_func = func;
		_user_data = v;
	}

	public static rollback_hook_info from_ptr(IntPtr p)
	{
		return ((GCHandle)p).Target as rollback_hook_info;
	}

	public void call()
	{
		_func(_user_data);
	}
}
