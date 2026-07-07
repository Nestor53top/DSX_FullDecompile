using System;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class sqlite3_stmt : SafeHandle
{
	private sqlite3 _db;

	public override bool IsInvalid => handle == IntPtr.Zero;

	internal IntPtr ptr => handle;

	internal sqlite3 db => _db;

	internal static sqlite3_stmt From(IntPtr p, sqlite3 db)
	{
		sqlite3_stmt sqlite3_stmt2 = new sqlite3_stmt();
		sqlite3_stmt2.SetHandle(p);
		db.add_stmt(sqlite3_stmt2);
		sqlite3_stmt2._db = db;
		return sqlite3_stmt2;
	}

	private sqlite3_stmt()
		: base(IntPtr.Zero, ownsHandle: true)
	{
	}

	protected override bool ReleaseHandle()
	{
		raw.internal_sqlite3_finalize(handle);
		_db.remove_stmt(this);
		return true;
	}

	public int manual_close()
	{
		int result = raw.internal_sqlite3_finalize(handle);
		handle = IntPtr.Zero;
		_db.remove_stmt(this);
		return result;
	}
}
