using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class sqlite3 : SafeHandle
{
	private ConcurrentDictionary<IntPtr, sqlite3_stmt> _stmts;

	private IDisposable extra;

	public override bool IsInvalid => handle == IntPtr.Zero;

	private sqlite3()
		: base(IntPtr.Zero, ownsHandle: true)
	{
	}

	protected override bool ReleaseHandle()
	{
		raw.internal_sqlite3_close_v2(handle);
		dispose_extra();
		return true;
	}

	public int manual_close_v2()
	{
		int result = raw.internal_sqlite3_close_v2(handle);
		handle = IntPtr.Zero;
		dispose_extra();
		return result;
	}

	public int manual_close()
	{
		int result = raw.internal_sqlite3_close(handle);
		handle = IntPtr.Zero;
		dispose_extra();
		return result;
	}

	internal static sqlite3 New(IntPtr p)
	{
		sqlite3 obj = new sqlite3();
		obj.SetHandle(p);
		return obj;
	}

	public void enable_sqlite3_next_stmt(bool enabled)
	{
		if (enabled)
		{
			if (_stmts == null)
			{
				_stmts = new ConcurrentDictionary<IntPtr, sqlite3_stmt>();
			}
		}
		else
		{
			_stmts = null;
		}
	}

	internal void add_stmt(sqlite3_stmt stmt)
	{
		if (_stmts != null)
		{
			_stmts[stmt.ptr] = stmt;
		}
	}

	internal sqlite3_stmt find_stmt(IntPtr p)
	{
		if (_stmts != null)
		{
			return _stmts[p];
		}
		throw new Exception("The sqlite3_next_stmt() function is disabled.  To enable it, call sqlite3.enable_sqlite3_next_stmt(true) immediately after opening the sqlite3 connection.");
	}

	internal void remove_stmt(sqlite3_stmt s)
	{
		if (_stmts != null)
		{
			_stmts.TryRemove(s.ptr, out var _);
		}
	}

	public T GetOrCreateExtra<T>(Func<T> f) where T : class, IDisposable
	{
		if (extra != null)
		{
			return (T)extra;
		}
		return (T)(extra = f());
	}

	private void dispose_extra()
	{
		if (extra != null)
		{
			extra.Dispose();
			extra = null;
		}
	}
}
