using System;
using System.Runtime.InteropServices;

namespace SQLitePCL;

public class hook_handle : SafeGCHandle
{
	public hook_handle(object target)
		: base(target, GCHandleType.Normal)
	{
	}

	public IDisposable ForDispose()
	{
		if (IsInvalid)
		{
			return null;
		}
		return this;
	}
}
