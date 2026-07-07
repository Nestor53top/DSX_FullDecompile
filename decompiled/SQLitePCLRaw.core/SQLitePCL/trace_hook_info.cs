using System;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class trace_hook_info
{
	private delegate_trace _func;

	private object _user_data;

	public trace_hook_info(delegate_trace func, object v)
	{
		_func = func;
		_user_data = v;
	}

	public static trace_hook_info from_ptr(IntPtr p)
	{
		return ((GCHandle)p).Target as trace_hook_info;
	}

	public void call(utf8z s)
	{
		_func(_user_data, s);
	}
}
