using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace HidSharp.Platform.Linux;

internal static class NativeMethods
{
	public enum error
	{
		OK = 0,
		EPERM = 1,
		EINTR = 4,
		EIO = 5,
		ENXIO = 6,
		EBADF = 9,
		EAGAIN = 11,
		EACCES = 13,
		EBUSY = 16,
		ENODEV = 19,
		EINVAL = 22
	}

	[Flags]
	public enum oflag
	{
		RDONLY = 0,
		WRONLY = 1,
		RDWR = 2,
		CREAT = 0x40,
		EXCL = 0x80,
		NOCTTY = 0x100,
		TRUNC = 0x200,
		APPEND = 0x400,
		NONBLOCK = 0x800
	}

	[Flags]
	public enum pollev : short
	{
		IN = 1,
		PRI = 2,
		OUT = 4,
		ERR = 8,
		HUP = 0x10,
		NVAL = 0x20
	}

	public struct pollfd
	{
		public int fd;

		public pollev events;

		public pollev revents;
	}

	public struct hidraw_report_descriptor
	{
		public uint size;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
		public byte[] value;
	}

	public struct termios
	{
		public uint c_iflag;

		public uint c_oflag;

		public uint c_cflag;

		public uint c_lflag;

		public byte c_line;

		public unsafe fixed byte c_cc[19];

		public uint c_ispeed;

		public uint c_ospeed;
	}

	private const string libc = "libc";

	public const int IOC_NONE = 0;

	public const int IOC_WRITE = 1;

	public const int IOC_READ = 2;

	public const int IOC_NRBITS = 8;

	public const int IOC_TYPEBITS = 8;

	public const int IOC_SIZEBITS = 14;

	public const int IOC_DIRBITS = 2;

	public const int IOC_NRSHIFT = 0;

	public const int IOC_TYPESHIFT = 8;

	public const int IOC_SIZESHIFT = 16;

	public const int IOC_DIRSHIFT = 30;

	public const int HID_MAX_DESCRIPTOR_SIZE = 4096;

	public const int VTIME = 5;

	public const int VMIN = 6;

	public const uint IGNBRK = 1u;

	public const uint BRKINT = 2u;

	public const uint PARMRK = 8u;

	public const uint ISTRIP = 32u;

	public const uint INLCR = 64u;

	public const uint IGNCR = 128u;

	public const uint ICRNL = 256u;

	public const uint IXON = 1024u;

	public const uint OPOST = 1u;

	public const uint CBAUD = 4111u;

	public const uint BOTHER = 4096u;

	public const uint CSIZE = 48u;

	public const uint CS7 = 32u;

	public const uint CS8 = 48u;

	public const uint CSTOPB = 64u;

	public const uint CREAD = 128u;

	public const uint PARENB = 256u;

	public const uint PARODD = 512u;

	public const uint CLOCAL = 2048u;

	public const uint CRTSCTS = 2147483648u;

	public const uint ECHO = 8u;

	public const uint ECHONL = 64u;

	public const uint ICANON = 2u;

	public const uint ISIG = 1u;

	public const uint IEXTEN = 32768u;

	public const int TCIFLUSH = 0;

	public const int TCSANOW = 0;

	public static readonly UIntPtr HIDIOCGRDESCSIZE = IOR(72, 1, 4);

	public static readonly UIntPtr HIDIOCGRDESC = IOR(72, 2, Marshal.SizeOf(typeof(hidraw_report_descriptor)));

	public static readonly UIntPtr TIOCEXCL = (UIntPtr)21516u;

	public static readonly UIntPtr TIOCNXCL = (UIntPtr)21517u;

	public static readonly UIntPtr TCGETS2 = IOR(84, 42, Marshal.SizeOf(typeof(termios)));

	public static readonly UIntPtr TCSETS2 = IOW(84, 43, Marshal.SizeOf(typeof(termios)));

	public static readonly UIntPtr TCSETSW2 = IOW(84, 44, Marshal.SizeOf(typeof(termios)));

	public static readonly UIntPtr TCSETSF2 = IOW(84, 45, Marshal.SizeOf(typeof(termios)));

	public static int retry(Func<int> sysfunc)
	{
		int num;
		error lastWin32Error;
		do
		{
			num = sysfunc();
			lastWin32Error = (error)Marshal.GetLastWin32Error();
		}
		while (num < 0 && lastWin32Error == error.EINTR);
		return num;
	}

	public static IntPtr retry(Func<IntPtr> sysfunc)
	{
		IntPtr intPtr;
		error lastWin32Error;
		do
		{
			intPtr = sysfunc();
			lastWin32Error = (error)Marshal.GetLastWin32Error();
		}
		while ((long)intPtr < 0 && lastWin32Error == error.EINTR);
		return intPtr;
	}

	public static bool uname(out string sysname, out Version release)
	{
		release = null;
		if (!uname(out sysname, out string release2))
		{
			return false;
		}
		release2 = new string(release2.Trim().TakeWhile((char ch) => (ch >= '0' && ch <= '9') || ch == '.').ToArray());
		release = new Version(release2);
		return true;
	}

	public static bool uname(out string sysname, out string release)
	{
		string typeName = "Mono.Unix.Native.Syscall, Mono.Posix, PublicKeyToken=0738eb9f132ed756";
		Type type = Type.GetType(typeName);
		if ((object)type != null)
		{
			object[] array = new object[1];
			int num = (int)type.InvokeMember("uname", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, array, CultureInfo.InvariantCulture);
			if (num >= 0)
			{
				object uname = array[0];
				Func<string, string> func = (string s) => (string)uname.GetType().InvokeMember(s, BindingFlags.GetField, null, uname, new object[0], CultureInfo.InvariantCulture);
				sysname = func("sysname");
				release = func("release");
				return true;
			}
		}
		try
		{
			if (File.Exists("/proc/sys/kernel/ostype") && File.Exists("/proc/sys/kernel/osrelease"))
			{
				sysname = File.ReadAllText("/proc/sys/kernel/ostype").TrimEnd(new char[1] { '\n' });
				release = File.ReadAllText("/proc/sys/kernel/osrelease").TrimEnd(new char[1] { '\n' });
				if (sysname != "" && release != "")
				{
					return true;
				}
			}
		}
		catch
		{
		}
		sysname = null;
		release = null;
		return false;
	}

	[DllImport("libc", SetLastError = true)]
	public static extern int open([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "HidSharp.Platform.Utf8Marshaler")] string filename, oflag oflag);

	[DllImport("libc", SetLastError = true)]
	public static extern int close(int filedes);

	[DllImport("libc", SetLastError = true)]
	public static extern IntPtr read(int filedes, IntPtr buffer, UIntPtr size);

	[DllImport("libc", SetLastError = true)]
	public static extern IntPtr write(int filedes, IntPtr buffer, UIntPtr size);

	[DllImport("libc", SetLastError = true)]
	public static extern int poll(pollfd[] fds, IntPtr nfds, int timeout);

	[DllImport("libc", SetLastError = true)]
	public static extern int poll(ref pollfd fds, IntPtr nfds, int timeout);

	public static bool TryParseHex(string hex, out int result)
	{
		return int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryParseVersion(string version, out int major, out int minor)
	{
		major = 0;
		minor = 0;
		if (version == null)
		{
			return false;
		}
		string[] array = version.Split(new char[1] { '.' }, 2);
		if (array.Length != 2)
		{
			return false;
		}
		if (int.TryParse(array[0], out major))
		{
			return int.TryParse(array[1], out minor);
		}
		return false;
	}

	public static UIntPtr IOC(int dir, int type, int nr, int size)
	{
		uint num = (uint)((dir << 30) | (type << 8) | nr | (size << 16));
		return (UIntPtr)num;
	}

	public static UIntPtr IOW(int type, int nr, int size)
	{
		return IOC(1, type, nr, size);
	}

	public static UIntPtr IOR(int type, int nr, int size)
	{
		return IOC(2, type, nr, size);
	}

	public static UIntPtr IOWR(int type, int nr, int size)
	{
		return IOC(3, type, nr, size);
	}

	public static UIntPtr HIDIOCSFEATURE(int length)
	{
		return IOWR(72, 6, length);
	}

	public static UIntPtr HIDIOCGFEATURE(int length)
	{
		return IOWR(72, 7, length);
	}

	[DllImport("libc", SetLastError = true)]
	public static extern int ioctl(int filedes, UIntPtr command, out uint value);

	[DllImport("libc", SetLastError = true)]
	public static extern int ioctl(int filedes, UIntPtr command, ref hidraw_report_descriptor value);

	[DllImport("libc", SetLastError = true)]
	public static extern int ioctl(int filedes, UIntPtr command, IntPtr value);

	[DllImport("libc", SetLastError = true)]
	public static extern int ioctl(int filedes, UIntPtr command, ref termios value);

	[DllImport("libc", SetLastError = true)]
	public static extern int ioctl(int filedes, UIntPtr command);

	public static void cfmakeraw(ref termios termios)
	{
		termios.c_iflag &= 4294965780u;
		termios.c_oflag &= 4294967294u;
		termios.c_lflag &= 4294934452u;
		termios.c_cflag &= 4294966991u;
		termios.c_cflag |= 48u;
	}

	public static int cfsetspeed(ref termios termios, uint speed)
	{
		termios.c_cflag &= 4294963184u;
		termios.c_cflag |= 4096u;
		termios.c_ispeed = speed;
		termios.c_ospeed = speed;
		return 0;
	}

	public static int tcgetattr(int filedes, out termios termios)
	{
		termios = default(termios);
		return ioctl(filedes, TCGETS2, ref termios);
	}

	public static int tcsetattr(int filedes, int actions, ref termios termios)
	{
		return ioctl(filedes, TCSETS2, ref termios);
	}

	[DllImport("libc", SetLastError = true)]
	public static extern int tcdrain(int filedes);

	[DllImport("libc", SetLastError = true)]
	public static extern int tcflush(int filedes, int action);
}
