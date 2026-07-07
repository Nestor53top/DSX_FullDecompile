using System;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class update_hook_info
{
	private delegate_update _func;

	private object _user_data;

	public update_hook_info(delegate_update func, object v)
	{
		_func = func;
		_user_data = v;
	}

	public static update_hook_info from_ptr(IntPtr p)
	{
		return ((GCHandle)p).Target as update_hook_info;
	}

	public void call(int typ, utf8z db, utf8z tbl, long rowid)
	{
		_func(_user_data, typ, db, tbl, rowid);
	}
}
