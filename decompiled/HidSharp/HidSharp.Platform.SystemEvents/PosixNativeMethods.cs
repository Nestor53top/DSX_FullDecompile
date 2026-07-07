using System;
using System.Runtime.InteropServices;

namespace HidSharp.Platform.SystemEvents;

internal abstract class PosixNativeMethods
{
	public static readonly IntPtr IntPtrNegativeOne = ((IntPtr.Size == 8) ? new IntPtr(-1L) : new IntPtr(-1));

	public abstract int MAP_SHARED { get; }

	public abstract int O_RDWR { get; }

	public abstract int O_CREAT { get; }

	public abstract int PROT_READ { get; }

	public abstract int PROT_WRITE { get; }

	public abstract int EINTR { get; }

	public abstract int GetTickCount();

	public abstract int shm_open(string filename, int oflag, int mode);

	public abstract int chmod(string filename, int mode);

	public abstract int fchmod(int filedes, int mode);

	public abstract int ftruncate(int filedes, long length);

	public abstract int close(int filedes);

	public abstract IntPtr mmap(IntPtr addr, UIntPtr size, int prot, int flags, int fd, long offset);

	public abstract int munmap(IntPtr addr, UIntPtr length);

	public int GetLastError()
	{
		return Marshal.GetLastWin32Error();
	}

	public int retry(Func<int> sysfunc)
	{
		int num;
		do
		{
			num = sysfunc();
		}
		while (num == -1 && GetLastError() == EINTR);
		return num;
	}

	public IntPtr retry(Func<IntPtr> sysfunc)
	{
		IntPtr intPtr;
		do
		{
			intPtr = sysfunc();
		}
		while (!(intPtr != IntPtrNegativeOne) && GetLastError() == EINTR);
		return intPtr;
	}
}
