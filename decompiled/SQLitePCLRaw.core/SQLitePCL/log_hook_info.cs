using System;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class log_hook_info
{
	private delegate_log _func;

	private object _user_data;

	public log_hook_info(delegate_log func, object v)
	{
		_func = func;
		_user_data = v;
	}

	public static log_hook_info from_ptr(IntPtr p)
	{
		return ((GCHandle)p).Target as log_hook_info;
	}

	public void call(int rc, utf8z msg)
	{
		_func(_user_data, rc, msg);
	}
}
