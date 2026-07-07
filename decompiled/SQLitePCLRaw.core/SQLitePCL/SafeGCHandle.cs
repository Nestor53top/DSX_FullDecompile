using System;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class SafeGCHandle : SafeHandle
{
	public override bool IsInvalid => handle == IntPtr.Zero;

	public SafeGCHandle(object v, GCHandleType typ)
		: base(IntPtr.Zero, ownsHandle: true)
	{
		if (v != null)
		{
			GCHandle value = GCHandle.Alloc(v, typ);
			SetHandle(GCHandle.ToIntPtr(value));
		}
	}

	protected override bool ReleaseHandle()
	{
		GCHandle.FromIntPtr(handle).Free();
		return true;
	}
}
