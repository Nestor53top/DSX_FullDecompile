using System;
using System.Runtime.InteropServices;

namespace HidSharp.Platform.SystemEvents;

internal class MacOSNativeMethods : PosixNativeMethods
{
	public struct mach_msg_header_t
	{
		public uint msgh_bits;

		public uint msgh_size;

		public uint msgh_remote_port;

		public uint msgh_local_port;

		public uint msgh_voucher_port;

		public int msgh_id;
	}

	public struct mach_msg_trailer_t
	{
		public uint type;

		public uint size;
	}

	public struct mach_msg_t
	{
		public mach_msg_header_t header;

		public mach_msg_trailer_t trailer;
	}

	private struct mach_timebase_info_data_t
	{
		public uint numer;

		public uint denom;
	}

	public struct pollfd
	{
		public int fd;

		public short events;

		public short revents;
	}

	private const string libc = "libc";

	private const string libdl = "libdl";

	public const int KERN_SUCCESS = 0;

	public const int RTLD_LAZY = 1;

	public const int MACH_PORT_RIGHT_RECEIVE = 1;

	public const int MACH_RCV_MSG = 2;

	public const int MACH_RCV_TOO_LARGE = 268451844;

	public const int NOTIFY_REUSE = 1;

	public const int NOTIFY_STATUS_OK = 0;

	public const short POLLIN = 1;

	public const short POLLERR = 8;

	public const int EBADF = 9;

	public const int ENODEV = 19;

	public const int EINVAL = 22;

	public const int ESPIPE = 29;

	private static uint _machTaskSelf;

	private static double _scale;

	public override int MAP_SHARED => 1;

	public override int PROT_READ => 1;

	public override int PROT_WRITE => 2;

	public override int O_RDWR => 2;

	public override int O_CREAT => 512;

	public override int EINTR => 4;

	static MacOSNativeMethods()
	{
		IntPtr intPtr = dlopen("/usr/lib/libSystem.dylib", 1);
		if (intPtr == IntPtr.Zero)
		{
			throw new InvalidOperationException("Unable to load libSystem.");
		}
		IntPtr intPtr2 = dlsym(intPtr, "mach_task_self_");
		if (intPtr2 == IntPtr.Zero)
		{
			throw new InvalidOperationException("Unable to load mach_task_self.");
		}
		_machTaskSelf = (uint)Marshal.ReadInt32(intPtr2);
		dlclose(intPtr);
		mach_timebase_info(out var info);
		_scale = (double)info.numer / (double)info.denom / 1000000.0;
	}

	public static uint GetMachTaskSelf()
	{
		return _machTaskSelf;
	}

	public override int GetTickCount()
	{
		return (int)(ulong)Math.Round((double)(ulong)mach_absolute_time() * _scale);
	}

	[DllImport("libdl")]
	private static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPStr)] string path, int mode);

	[DllImport("libdl")]
	private static extern IntPtr dlsym(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string symbol);

	[DllImport("libdl")]
	private static extern int dlclose(IntPtr handle);

	[DllImport("libc")]
	private static extern long mach_absolute_time();

	[DllImport("libc")]
	private static extern void mach_timebase_info(out mach_timebase_info_data_t info);

	[DllImport("libc")]
	public static extern int mach_msg_overwrite(IntPtr msg, int option, uint send_size, uint recv_size, uint recv_name, uint timeout, uint notify, IntPtr recv_msg, uint recv_limit);

	[DllImport("libc")]
	public static extern int mach_port_allocate(uint task, uint right, out uint name);

	[DllImport("libc")]
	public static extern uint notify_post([MarshalAs(UnmanagedType.LPStr)] string name);

	[DllImport("libc")]
	public static extern uint notify_register_file_descriptor([MarshalAs(UnmanagedType.LPStr)] string name, ref int fd, int flags, out int token);

	[DllImport("libc")]
	public static extern uint notify_register_mach_port([MarshalAs(UnmanagedType.LPStr)] string name, ref uint port, int flags, out int token);

	[DllImport("libc")]
	public static extern uint notify_cancel(int token);

	[DllImport("libc")]
	public static extern int poll(ref pollfd fd, uint nfds, int timeout = -1);

	[DllImport("libc")]
	public static extern IntPtr read(int filedes, byte[] buffer, UIntPtr nbyte);

	public override int shm_open(string filename, int oflag, int mode)
	{
		return native_shm_open(filename, oflag, mode);
	}

	[DllImport("libc", EntryPoint = "shm_open", SetLastError = true)]
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
		return native_ftruncate(filedes, length);
	}

	[DllImport("libc", EntryPoint = "ftruncate", SetLastError = true)]
	private static extern int native_ftruncate(int filedes, long length);

	public override int close(int filedes)
	{
		return native_close(filedes);
	}

	[DllImport("libc", EntryPoint = "close", SetLastError = true)]
	private static extern int native_close(int filedes);

	public override IntPtr mmap(IntPtr addr, UIntPtr size, int prot, int flags, int fd, long offset)
	{
		return native_mmap(addr, size, prot, flags, fd, offset);
	}

	[DllImport("libc", EntryPoint = "mmap", SetLastError = true)]
	private static extern IntPtr native_mmap(IntPtr addr, UIntPtr size, int prot, int flags, int fd, long offset);

	public override int munmap(IntPtr addr, UIntPtr length)
	{
		return native_munmap(addr, length);
	}

	[DllImport("libc", EntryPoint = "munmap", SetLastError = true)]
	private static extern int native_munmap(IntPtr addr, UIntPtr length);
}
