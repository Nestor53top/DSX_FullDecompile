using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Squirrel;

internal static class UnsafeUtility
{
	public unsafe static List<Tuple<string, int>> EnumerateProcesses()
	{
		int pBytesReturned = 0;
		int[] pids = new int[2048];
		fixed (int* ptr = pids)
		{
			if (!NativeMethods.EnumProcesses((IntPtr)ptr, 4 * pids.Length, out pBytesReturned))
			{
				throw new Win32Exception("Failed to enumerate processes");
			}
			if (pBytesReturned < 1)
			{
				throw new Exception("Failed to enumerate processes");
			}
		}
		return (from i in Enumerable.Range(0, pBytesReturned / 4)
			where pids[i] > 0
			select i).Select(delegate(int i)
		{
			try
			{
				IntPtr intPtr = NativeMethods.OpenProcess(ProcessAccess.QueryLimitedInformation, bInheritHandle: false, pids[i]);
				if (intPtr == IntPtr.Zero)
				{
					throw new Win32Exception();
				}
				StringBuilder stringBuilder = new StringBuilder(256);
				int nSize = stringBuilder.Capacity;
				if (!NativeMethods.QueryFullProcessImageName(intPtr, 0, stringBuilder, ref nSize))
				{
					throw new Win32Exception();
				}
				NativeMethods.CloseHandle(intPtr);
				return Tuple.Create(stringBuilder.ToString(), pids[i]);
			}
			catch (Exception)
			{
				return Tuple.Create<string, int>(null, pids[i]);
			}
		}).ToList();
	}
}
