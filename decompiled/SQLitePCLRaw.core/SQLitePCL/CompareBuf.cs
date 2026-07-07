using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SQLitePCL;

internal class CompareBuf : EqualityComparer<byte[]>
{
	private Func<IntPtr, IntPtr, int, bool> _f;

	public CompareBuf(Func<IntPtr, IntPtr, int, bool> f)
	{
		_f = f;
	}

	public override bool Equals(byte[] p1, byte[] p2)
	{
		if (p1.Length != p2.Length)
		{
			return false;
		}
		GCHandle gCHandle = GCHandle.Alloc(p1, GCHandleType.Pinned);
		GCHandle gCHandle2 = GCHandle.Alloc(p2, GCHandleType.Pinned);
		bool result = _f(gCHandle.AddrOfPinnedObject(), gCHandle2.AddrOfPinnedObject(), p1.Length);
		gCHandle.Free();
		gCHandle2.Free();
		return result;
	}

	public override int GetHashCode(byte[] p)
	{
		return p.Length;
	}
}
