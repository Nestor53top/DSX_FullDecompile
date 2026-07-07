using System;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class sqlite3_backup : SafeHandle
{
	public override bool IsInvalid => handle == IntPtr.Zero;

	private sqlite3_backup()
		: base(IntPtr.Zero, ownsHandle: true)
	{
	}

	protected override bool ReleaseHandle()
	{
		raw.internal_sqlite3_backup_finish(handle);
		return true;
	}

	public int manual_close()
	{
		int result = raw.internal_sqlite3_backup_finish(handle);
		handle = IntPtr.Zero;
		return result;
	}
}
