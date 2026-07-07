using System;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class commit_hook_info
{
	public delegate_commit _func { get; private set; }

	public object _user_data { get; private set; }

	public commit_hook_info(delegate_commit func, object v)
	{
		_func = func;
		_user_data = v;
	}

	public int call()
	{
		return _func(_user_data);
	}

	public static commit_hook_info from_ptr(IntPtr p)
	{
		return ((GCHandle)p).Target as commit_hook_info;
	}
}
