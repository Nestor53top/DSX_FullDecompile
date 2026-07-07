using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HidSharp.Platform.MacOS;

internal static class NativeMethods
{
	public enum error
	{
		EINTR = 4,
		EACCES = 13,
		EBUSY = 16,
		EAGAIN = 35
	}

	public enum oflag
	{
		RDWR = 2,
		NONBLOCK = 4,
		NOCTTY = 0x20000
	}

	public enum pollev : short
	{
		IN = 1,
		OUT = 4,
		ERR = 8,
		HUP = 0x10,
		NVAL = 0x20
	}

	public delegate void IOHIDCallback(IntPtr context, IOReturn result, IntPtr sender);

	public delegate void IOHIDDeviceCallback(IntPtr context, IOReturn result, IntPtr sender, IntPtr device);

	public delegate void IOHIDReportCallback(IntPtr context, IOReturn result, IntPtr sender, IOHIDReportType type, uint reportID, IntPtr report, IntPtr reportLength);

	public delegate void IOServiceMatchingCallback(IntPtr context, int iterator);

	public enum OSErr : short
	{
		noErr = 0,
		gestaltUnknownErr = -5550,
		gestaltUndefSelectorErr = -5551,
		gestaltDupSelectorErr = -5552,
		gestaltLocationErr = -5553
	}

	public enum OSType : uint
	{
		gestaltSystemVersion = 1937339254u,
		gestaltSystemVersionMajor = 1937339185u,
		gestaltSystemVersionMinor = 1937339186u,
		gestaltSystemVersionBugFix = 1937339187u
	}

	public enum IOOptionBits
	{
		None
	}

	public enum IOHIDElementType
	{
		InputMisc = 1,
		InputButton = 2,
		InputAxis = 3,
		InputScanCodes = 4,
		Output = 129,
		Feature = 257,
		Collection = 513
	}

	public enum IOHIDReportType
	{
		Input,
		Output,
		Feature
	}

	public enum IOReturn
	{
		Success = 0,
		ExclusiveAccess = -536870203,
		NotSupported = -536870201,
		Offline = -536870185,
		NotPermitted = -536870174
	}

	public struct io_string_t
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
		public byte[] Value;

		public io_string_t Clone()
		{
			return new io_string_t
			{
				Value = (byte[])Value.Clone()
			};
		}

		public override bool Equals(object obj)
		{
			if (obj is io_string_t)
			{
				return this == (io_string_t)obj;
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (Value.Length < 1)
			{
				return -1;
			}
			return Value[0];
		}

		public override string ToString()
		{
			return Encoding.UTF8.GetString(Value.TakeWhile((byte ch) => ch != 0).ToArray());
		}

		public static bool operator ==(io_string_t io1, io_string_t io2)
		{
			return Enumerable.SequenceEqual(io1.Value, io2.Value);
		}

		public static bool operator !=(io_string_t io1, io_string_t io2)
		{
			return !(io1 == io2);
		}
	}

	public struct pollfd
	{
		public int fd;

		public pollev events;

		public pollev revents;
	}

	public struct termios
	{
		public UIntPtr c_iflag;

		public UIntPtr c_oflag;

		public UIntPtr c_cflag;

		public UIntPtr c_lflag;

		public unsafe fixed byte c_cc[20];

		public UIntPtr c_ispeed;

		public UIntPtr c_ospeed;
	}

	public enum CFNumberType
	{
		Int = 9
	}

	public struct CFRange
	{
		public IntPtr Start;

		public IntPtr Length;
	}

	public struct IOObject : IDisposable
	{
		public int Handle { get; set; }

		public bool IsSet => Handle != 0;

		void IDisposable.Dispose()
		{
			if (IsSet)
			{
				IOObjectRelease(Handle);
				Handle = 0;
			}
		}

		public static implicit operator int(IOObject self)
		{
			return self.Handle;
		}
	}

	public struct CFType : IDisposable
	{
		public IntPtr Handle { get; set; }

		public bool IsSet => Handle != IntPtr.Zero;

		void IDisposable.Dispose()
		{
			if (IsSet)
			{
				CFRelease(Handle);
				Handle = IntPtr.Zero;
			}
		}

		public static implicit operator IntPtr(CFType self)
		{
			return self.Handle;
		}
	}

	private const string libc = "libc";

	private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

	private const string CoreServices = "/System/Library/Frameworks/CoreServices.framework/CoreServices";

	private const string IOKit = "/System/Library/Frameworks/IOKit.framework/IOKit";

	public const int F_SETFL = 4;

	public const int TCIFLUSH = 1;

	public const int TCOFLUSH = 2;

	public const int TCIOFLUSH = 3;

	public const int TCSANOW = 0;

	public const int TCSADRAIN = 1;

	public const int TCSAFLUSH = 2;

	public const int VMIN = 16;

	public const int VTIME = 17;

	public const ulong IXON = 512uL;

	public const ulong IXOFF = 1024uL;

	public const ulong CSIZE = 768uL;

	public const ulong CS7 = 512uL;

	public const ulong CS8 = 768uL;

	public const ulong CSTOPB = 1024uL;

	public const ulong CREAD = 2048uL;

	public const ulong PARENB = 4096uL;

	public const ulong PARODD = 8192uL;

	public const ulong HUPCL = 16384uL;

	public const ulong CLOCAL = 32768uL;

	public const ulong CRTSCTS = 196608uL;

	public const ulong OPOST = 1uL;

	public const uint IOC_VOID = 536870912u;

	public const uint IOC_OUT = 1073741824u;

	public const uint IOC_IN = 2147483648u;

	public const uint IOC_INOUT = 3221225472u;

	public static readonly UIntPtr TIOCEXCL = _IO(116, 13);

	public static readonly UIntPtr TIOCNXCL = _IO(116, 14);

	public static readonly IntPtr kCFRunLoopDefaultMode = CFStringCreateWithCharacters("kCFRunLoopDefaultMode");

	public static readonly IntPtr kIOFirstMatchNotification = CFStringCreateWithCharacters("IOServiceFirstMatch");

	public static readonly IntPtr kIOMatchedNotification = CFStringCreateWithCharacters("IOServiceMatched");

	public static readonly IntPtr kIOTerminatedNotification = CFStringCreateWithCharacters("IOServiceTerminate");

	public static readonly IntPtr kIOHIDVendorIDKey = CFStringCreateWithCharacters("VendorID");

	public static readonly IntPtr kIOHIDProductIDKey = CFStringCreateWithCharacters("ProductID");

	public static readonly IntPtr kIOHIDVersionNumberKey = CFStringCreateWithCharacters("VersionNumber");

	public static readonly IntPtr kIOHIDManufacturerKey = CFStringCreateWithCharacters("Manufacturer");

	public static readonly IntPtr kIOHIDProductKey = CFStringCreateWithCharacters("Product");

	public static readonly IntPtr kIOHIDSerialNumberKey = CFStringCreateWithCharacters("SerialNumber");

	public static readonly IntPtr kIOHIDLocationIDKey = CFStringCreateWithCharacters("LocationID");

	public static readonly IntPtr kIOHIDMaxInputReportSizeKey = CFStringCreateWithCharacters("MaxInputReportSize");

	public static readonly IntPtr kIOHIDMaxOutputReportSizeKey = CFStringCreateWithCharacters("MaxOutputReportSize");

	public static readonly IntPtr kIOHIDMaxFeatureReportSizeKey = CFStringCreateWithCharacters("MaxFeatureReportSize");

	public static readonly IntPtr kIOHIDReportDescriptorKey = CFStringCreateWithCharacters("ReportDescriptor");

	public static readonly IntPtr kIOCalloutDeviceKey = CFStringCreateWithCharacters("IOCalloutDevice");

	private static UIntPtr _IOC(uint inout, byte group, byte num, int len)
	{
		return (UIntPtr)(inout | (uint)(len << 16) | (uint)(group << 8) | num);
	}

	private static UIntPtr _IO(byte group, byte num)
	{
		return _IOC(536870912u, group, num, 0);
	}

	public static CFType ToCFType(this IntPtr handle)
	{
		return new CFType
		{
			Handle = handle
		};
	}

	public static IOObject ToIOObject(this int handle)
	{
		return new IOObject
		{
			Handle = handle
		};
	}

	[DllImport("/System/Library/Frameworks/CoreServices.framework/CoreServices")]
	public static extern OSErr Gestalt(OSType selector, out IntPtr response);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern uint CFGetTypeID(IntPtr type);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern uint CFArrayGetTypeID();

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern uint CFDataGetTypeID();

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern uint CFNumberGetTypeID();

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern uint CFStringGetTypeID();

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern IntPtr CFDictionaryCreateMutable(IntPtr allocator, IntPtr capacity, IntPtr keyCallbacks, IntPtr valueCallbacks);

	public static IntPtr CFDictionaryCreateMutable()
	{
		return CFDictionaryCreateMutable(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
	}

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern void CFDictionarySetValue(IntPtr dict, IntPtr key, IntPtr value);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern IntPtr CFArrayGetCount(IntPtr array);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern IntPtr CFArrayGetValueAtIndex(IntPtr array, IntPtr index);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern void CFDataGetBytes(IntPtr data, CFRange range, byte[] buffer);

	public static byte[] CFDataGetBytes(IntPtr data)
	{
		if (data == IntPtr.Zero || CFGetTypeID(data) != CFDataGetTypeID())
		{
			return null;
		}
		byte[] array = new byte[(int)CFDataGetLength(data)];
		CFDataGetBytes(data, new CFRange
		{
			Start = (IntPtr)0,
			Length = (IntPtr)array.Length
		}, array);
		return array;
	}

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern IntPtr CFDataGetLength(IntPtr data);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern IntPtr CFNumberCreate(IntPtr allocator, CFNumberType type, ref int value);

	public static IntPtr CFNumberCreate(int value)
	{
		return CFNumberCreate(IntPtr.Zero, CFNumberType.Int, ref value);
	}

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	[return: MarshalAs(UnmanagedType.I1)]
	public static extern bool CFNumberGetValue(IntPtr number, CFNumberType type, out int value);

	public static int? CFNumberGetValue(IntPtr number)
	{
		if (!(number != IntPtr.Zero) || CFGetTypeID(number) != CFNumberGetTypeID() || !CFNumberGetValue(number, CFNumberType.Int, out var value))
		{
			return null;
		}
		return value;
	}

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", CharSet = CharSet.Unicode)]
	public static extern IntPtr CFStringCreateWithCharacters(IntPtr allocator, char[] buffer, IntPtr length);

	public static IntPtr CFStringCreateWithCharacters(string str)
	{
		return CFStringCreateWithCharacters(IntPtr.Zero, str.ToCharArray(), (IntPtr)str.Length);
	}

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", CharSet = CharSet.Unicode)]
	public static extern void CFStringGetCharacters(IntPtr str, CFRange range, char[] buffer);

	public static string CFStringGetCharacters(IntPtr str)
	{
		if (str == IntPtr.Zero || CFGetTypeID(str) != CFStringGetTypeID())
		{
			return null;
		}
		char[] array = new char[(int)CFStringGetLength(str)];
		CFStringGetCharacters(str, new CFRange
		{
			Start = (IntPtr)0,
			Length = (IntPtr)array.Length
		}, array);
		return new string(array);
	}

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern IntPtr CFStringGetLength(IntPtr str);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern void CFRunLoopRun();

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern IntPtr CFRunLoopGetCurrent();

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern void CFRunLoopAddSource(IntPtr runLoop, IntPtr source, IntPtr mode);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern void CFRunLoopRemoveSource(IntPtr runLoop, IntPtr source, IntPtr mode);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern void CFRunLoopStop(IntPtr runLoop);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern void CFRelease(IntPtr obj);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern void CFRetain(IntPtr obj);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern IntPtr CFSetGetCount(IntPtr set);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	public static extern void CFSetGetValues(IntPtr set, IntPtr[] values);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IntPtr IOHIDDeviceCreate(IntPtr allocator, int service);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IOReturn IOHIDDeviceOpen(IntPtr device, IOOptionBits options = IOOptionBits.None);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IntPtr IOHIDDeviceCopyMatchingElements(IntPtr device, IntPtr matching, IOOptionBits options = IOOptionBits.None);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern void IOHIDDeviceRegisterInputReportCallback(IntPtr device, IntPtr report, IntPtr reportLength, IOHIDReportCallback callback, IntPtr context);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern void IOHIDDeviceRegisterRemovalCallback(IntPtr device, IOHIDCallback callback, IntPtr context);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IOReturn IOHIDDeviceGetReport(IntPtr device, IOHIDReportType type, IntPtr reportID, IntPtr report, ref IntPtr reportLength);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IOReturn IOHIDDeviceSetReport(IntPtr device, IOHIDReportType type, IntPtr reportID, IntPtr report, IntPtr reportLength);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern void IOHIDDeviceScheduleWithRunLoop(IntPtr device, IntPtr runLoop, IntPtr runLoopMode);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern void IOHIDDeviceUnscheduleFromRunLoop(IntPtr device, IntPtr runLoop, IntPtr runLoopMode);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IOReturn IOHIDDeviceClose(IntPtr device, IOOptionBits options = IOOptionBits.None);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern uint IOHIDElementGetReportID(IntPtr element);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IOHIDElementType IOHIDElementGetType(IntPtr element);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IntPtr IOHIDManagerCreate(IntPtr allocator, IOOptionBits options = IOOptionBits.None);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern void IOHIDManagerSetDeviceMatching(IntPtr manager, IntPtr matching);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern void IOHIDManagerRegisterDeviceMatchingCallback(IntPtr manager, IOHIDDeviceCallback callback, IntPtr context);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern void IOHIDManagerRegisterDeviceRemovalCallback(IntPtr manager, IOHIDDeviceCallback callback, IntPtr context);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern void IOHIDManagerScheduleWithRunLoop(IntPtr manager, IntPtr runLoop, IntPtr runLoopMode);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern void IOHIDManagerUnscheduleFromRunLoop(IntPtr manager, IntPtr runLoop, IntPtr runLoopMode);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IntPtr IONotificationPortCreate(int masterPort);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IntPtr IONotificationPortGetRunLoopSource(IntPtr notifyPort);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern int IOIteratorNext(int iterator);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IOReturn IOObjectRetain(int @object);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IOReturn IOObjectRelease(int @object);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IntPtr IORegistryEntryCreateCFProperty(int entry, IntPtr strKey, IntPtr allocator, IOOptionBits options = IOOptionBits.None);

	public static int? IORegistryEntryGetCFProperty_Int(int entry, IntPtr intKey)
	{
		using CFType cFType = IORegistryEntryCreateCFProperty(entry, intKey, IntPtr.Zero).ToCFType();
		return CFNumberGetValue(cFType);
	}

	public static string IORegistryEntryGetCFProperty_String(int entry, IntPtr strKey)
	{
		using CFType cFType = IORegistryEntryCreateCFProperty(entry, strKey, IntPtr.Zero).ToCFType();
		return CFStringGetCharacters(cFType);
	}

	public static byte[] IORegistryEntryGetCFProperty_Data(int entry, IntPtr dataKey)
	{
		using CFType cFType = IORegistryEntryCreateCFProperty(entry, dataKey, IntPtr.Zero).ToCFType();
		return CFDataGetBytes(cFType);
	}

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern int IORegistryEntryFromPath(int masterPort, ref io_string_t path);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IOReturn IORegistryEntryGetPath(int entry, [MarshalAs(UnmanagedType.LPStr)] string plane, out io_string_t path);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IOReturn IOServiceAddMatchingNotification(IntPtr notifyPort, IntPtr notifyType, IntPtr matching, IOServiceMatchingCallback callback, IntPtr context, out int iterator);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IOReturn IOServiceGetMatchingServices(int masterPort, IntPtr matching, out int iterator);

	[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
	public static extern IntPtr IOServiceMatching([MarshalAs(UnmanagedType.LPStr)] string name);

	[DllImport("libc", SetLastError = true)]
	public static extern int open([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "HidSharp.Platform.Utf8Marshaler")] string filename, oflag oflag);

	[DllImport("libc", SetLastError = true)]
	public static extern IntPtr read(int filedes, IntPtr buffer, UIntPtr size);

	[DllImport("libc", SetLastError = true)]
	public static extern IntPtr write(int filedes, IntPtr buffer, UIntPtr size);

	[DllImport("libc")]
	public static extern int poll(ref pollfd fd, uint nfds, int timeout = -1);

	[DllImport("libc", SetLastError = true)]
	public static extern int fcntl(int filedes, int cmd, int arg);

	[DllImport("libc", SetLastError = true)]
	public static extern int ioctl(int filedes, UIntPtr request);

	[DllImport("libc", SetLastError = true)]
	public static extern int close(int filedes);

	[DllImport("libc", SetLastError = true)]
	public static extern void cfmakeraw(ref termios termios);

	[DllImport("libc", SetLastError = true)]
	public static extern int cfsetspeed(ref termios termios, UIntPtr speed);

	[DllImport("libc", SetLastError = true)]
	public static extern int tcgetattr(int filedes, out termios termios);

	[DllImport("libc", SetLastError = true)]
	public static extern int tcsetattr(int filedes, int actions, ref termios termios);

	[DllImport("libc", SetLastError = true)]
	public static extern int tcdrain(int filedes);

	[DllImport("libc", SetLastError = true)]
	public static extern int tcflush(int filedes, int action);

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
}
