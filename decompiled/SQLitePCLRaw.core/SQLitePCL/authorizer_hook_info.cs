using System;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class authorizer_hook_info
{
	private delegate_authorizer _func;

	private object _user_data;

	public authorizer_hook_info(delegate_authorizer func, object v)
	{
		_func = func;
		_user_data = v;
	}

	public static authorizer_hook_info from_ptr(IntPtr p)
	{
		return ((GCHandle)p).Target as authorizer_hook_info;
	}

	public int call(int action_code, utf8z param0, utf8z param1, utf8z dbName, utf8z inner_most_trigger_or_view)
	{
		return _func(_user_data, action_code, param0, param1, dbName, inner_most_trigger_or_view);
	}
}
