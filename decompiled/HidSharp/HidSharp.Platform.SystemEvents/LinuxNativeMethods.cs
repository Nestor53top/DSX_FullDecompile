using System;
using System.Runtime.InteropServices;

namespace HidSharp.Platform.SystemEvents;

internal class LinuxNativeMethods : PosixNativeMethods
{
	public struct inotify_event
	{
		public int wd;

		public uint mask;

		public uint cookie;

		public uint len;
	}

	public struct timespec
	{
		public IntPtr seconds;

		public IntPtr nanoseconds;
	}

	private const string libc = "libc";

	private const string librt = "librt.so.1";

	public const int CLOCK_MONOTONIC = 1;

	public const int IN_ACCESS = 1;

	public const int IN_MODIFY = 2;

	public const int IN_ATTRIB = 4;

	public const int NAME_MAX = 255;

	public override int MAP_SHARED => 1;

	public override int PROT_READ => 1;

	public override int PROT_WRITE => 2;

	public override int O_RDWR => 2;

	public override int O_CREAT => 64;

	public override int EINTR => 4;

	public override int GetTickCount()
	{
		if (clock_gettime(1, out var timespec2) < 0)
		{
			throw new InvalidOperationException();
		}
		return (int)((long)timespec2.seconds * 1000 + (long)timespec2.nanoseconds / 1000000);
	}

	[DllImport("libc", SetLastError = true)]
	private static extern int clock_gettime(int clockid, out timespec timespec);

	public override int shm_open(string filename, int oflag, int mode)
	{
		return native_shm_open(filename, oflag, mode);
	}

	[DllImport("librt.so.1", EntryPoint = "shm_open", SetLastError = true)]
	private static extern int native_shm_open([MarshalAs(UnmanagedType.LPStr)] string filename, int oflag, int mode);

	public override int chmod(string filename, int mode)
	{
		return native_chmod(filename, mode);
	}

	[DllImport("libc", EntryPoint = "chmod", SetLastError = true)]
	private static extern int native_chmod([MarshalAs(UnmanagedType.LPStr)] string filename, int mode);

	public override int fchmod(int filedes, int mode)
	{
		return native_fchmod(filedes, mode);
	}

	[DllImport("libc", EntryPoint = "fchmod", SetLastError = true)]
	private static extern int native_fchmod(int filedes, int mode);

	public override int ftruncate(int filedes, long length)
	{
		return native_ftruncate64(filedes, length);
	}

	[DllImport("libc", EntryPoint = "ftruncate64", SetLastError = true)]
	private static extern int native_ftruncate64(int filedes, long length);

	[DllImport("libc", SetLastError = true)]
	public static extern IntPtr read(int filedes, IntPtr buffer, UIntPtr size);

	public override int close(int filedes)
	{
		return native_close(filedes);
	}

	[DllImport("libc", EntryPoint = "close", SetLastError = true)]
	private static extern int native_close(int filedes);

	public override IntPtr mmap(IntPtr addr, UIntPtr size, int prot, int flags, int fd, long offset)
	{
		return native_mmap64(addr, size, prot, flags, fd, offset);
	}

	[DllImport("libc", EntryPoint = "mmap64", SetLastError = true)]
	private static extern IntPtr native_mmap64(IntPtr addr, UIntPtr size, int prot, int flags, int fd, long offset);

	public override int munmap(IntPtr addr, UIntPtr length)
	{
		return native_munmap(addr, length);
	}

	[DllImport("libc", EntryPoint = "munmap", SetLastError = true)]
	private static extern int native_munmap(IntPtr addr, UIntPtr length);

	[DllImport("libc", SetLastError = true)]
	public static extern int utime([MarshalAs(UnmanagedType.LPStr)] string file, IntPtr times);

	[DllImport("libc", SetLastError = true)]
	public static extern int inotify_init();

	[DllImport("libc", SetLastError = true)]
	public static extern int inotify_add_watch(int fd, [MarshalAs(UnmanagedType.LPStr)] string pathname, int mask);

	[DllImport("libc", SetLastError = true)]
	public static extern int inotify_rm_watch(int fd, int wd);
}
