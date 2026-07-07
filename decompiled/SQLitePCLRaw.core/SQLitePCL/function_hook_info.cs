using System;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class function_hook_info
{
	private class agg_sqlite3_context : sqlite3_context
	{
		public agg_sqlite3_context(object v)
			: base(v)
		{
		}

		public void fix_ptr(IntPtr p)
		{
			set_context_ptr(p);
		}
	}

	private class scalar_sqlite3_context : sqlite3_context
	{
		public scalar_sqlite3_context(IntPtr p, object v)
			: base(v)
		{
			set_context_ptr(p);
		}
	}

	private delegate_function_scalar _func_scalar;

	private delegate_function_aggregate_step _func_step;

	private delegate_function_aggregate_final _func_final;

	private object _user_data;

	public function_hook_info(delegate_function_scalar func_scalar, object user_data)
	{
		_func_scalar = func_scalar;
		_user_data = user_data;
	}

	public function_hook_info(delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final, object user_data)
	{
		_func_step = func_step;
		_func_final = func_final;
		_user_data = user_data;
	}

	public static function_hook_info from_ptr(IntPtr p)
	{
		return ((GCHandle)p).Target as function_hook_info;
	}

	private sqlite3_context get_context(IntPtr context, IntPtr agg_context)
	{
		IntPtr intPtr = Marshal.ReadIntPtr(agg_context);
		agg_sqlite3_context agg_sqlite3_context2;
		if (intPtr == IntPtr.Zero)
		{
			agg_sqlite3_context2 = new agg_sqlite3_context(_user_data);
			GCHandle gCHandle = GCHandle.Alloc(agg_sqlite3_context2);
			Marshal.WriteIntPtr(agg_context, (IntPtr)gCHandle);
		}
		else
		{
			agg_sqlite3_context2 = ((GCHandle)intPtr).Target as agg_sqlite3_context;
		}
		agg_sqlite3_context2.fix_ptr(context);
		return agg_sqlite3_context2;
	}

	public void call_scalar(IntPtr context, int num_args, IntPtr argsptr)
	{
		scalar_sqlite3_context ctx = new scalar_sqlite3_context(context, _user_data);
		sqlite3_value[] array = new sqlite3_value[num_args];
		int num = Marshal.SizeOf(typeof(IntPtr));
		for (int i = 0; i < num_args; i++)
		{
			IntPtr p = Marshal.ReadIntPtr(argsptr, i * num);
			array[i] = new sqlite3_value(p);
		}
		_func_scalar(ctx, _user_data, array);
	}

	public void call_step(IntPtr context, IntPtr agg_context, int num_args, IntPtr argsptr)
	{
		sqlite3_context ctx = get_context(context, agg_context);
		sqlite3_value[] array = new sqlite3_value[num_args];
		int num = Marshal.SizeOf(typeof(IntPtr));
		for (int i = 0; i < num_args; i++)
		{
			IntPtr p = Marshal.ReadIntPtr(argsptr, i * num);
			array[i] = new sqlite3_value(p);
		}
		_func_step(ctx, _user_data, array);
	}

	public void call_final(IntPtr context, IntPtr agg_context)
	{
		sqlite3_context ctx = get_context(context, agg_context);
		_func_final(ctx, _user_data);
		((GCHandle)Marshal.ReadIntPtr(agg_context)).Free();
	}
}
