using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace DualSenseX;

[SuppressUnmanagedCodeSecurity]
internal static class NativeMethods
{
	internal struct BLUETOOTH_FIND_RADIO_PARAMS
	{
		[MarshalAs(UnmanagedType.U4)]
		public int dwSize;
	}

	internal struct OVERLAPPED
	{
		public int Internal;

		public int InternalHigh;

		public int Offset;

		public int OffsetHigh;

		public int hEvent;
	}

	internal struct SECURITY_ATTRIBUTES
	{
		public int nLength;

		public IntPtr lpSecurityDescriptor;

		public bool bInheritHandle;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal class DEV_BROADCAST_DEVICEINTERFACE
	{
		internal int dbcc_size;

		internal int dbcc_devicetype;

		internal int dbcc_reserved;

		internal Guid dbcc_classguid;

		internal short dbcc_name;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal class DEV_BROADCAST_DEVICEINTERFACE_1
	{
		internal int dbcc_size;

		internal int dbcc_devicetype;

		internal int dbcc_reserved;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.U1)]
		internal byte[] dbcc_classguid;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
		internal char[] dbcc_name;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal class DEV_BROADCAST_HANDLE
	{
		internal int dbch_size;

		internal int dbch_devicetype;

		internal int dbch_reserved;

		internal int dbch_handle;

		internal int dbch_hdevnotify;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal class DEV_BROADCAST_HDR
	{
		internal int dbch_size;

		internal int dbch_devicetype;

		internal int dbch_reserved;
	}

	internal struct SP_DEVICE_INTERFACE_DATA
	{
		internal int cbSize;

		internal Guid InterfaceClassGuid;

		internal int Flags;

		internal IntPtr Reserved;
	}

	internal struct SP_DEVINFO_DATA
	{
		internal int cbSize;

		internal Guid ClassGuid;

		internal int DevInst;

		internal IntPtr Reserved;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	internal struct SP_DEVICE_INTERFACE_DETAIL_DATA
	{
		internal int Size;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		internal string DevicePath;
	}

	internal struct DEVPROPKEY
	{
		public Guid fmtid;

		public ulong pid;
	}

	internal struct SP_CLASSINSTALL_HEADER
	{
		internal int cbSize;

		internal int installFunction;
	}

	internal struct SP_PROPCHANGE_PARAMS
	{
		internal SP_CLASSINSTALL_HEADER classInstallHeader;

		internal int stateChange;

		internal int scope;

		internal int hwProfile;
	}

	internal struct HIDD_ATTRIBUTES
	{
		internal int Size;

		internal ushort VendorID;

		internal ushort ProductID;

		internal short VersionNumber;
	}

	internal struct HIDP_CAPS
	{
		internal ushort Usage;

		internal ushort UsagePage;

		internal short InputReportByteLength;

		internal short OutputReportByteLength;

		internal short FeatureReportByteLength;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
		internal short[] Reserved;

		internal short NumberLinkCollectionNodes;

		internal short NumberInputButtonCaps;

		internal short NumberInputValueCaps;

		internal short NumberInputDataIndices;

		internal short NumberOutputButtonCaps;

		internal short NumberOutputValueCaps;

		internal short NumberOutputDataIndices;

		internal short NumberFeatureButtonCaps;

		internal short NumberFeatureValueCaps;

		internal short NumberFeatureDataIndices;
	}

	internal struct HIDP_VALUE_CAPS
	{
		internal short UsagePage;

		internal byte ReportID;

		internal int IsAlias;

		internal short BitField;

		internal short LinkCollection;

		internal short LinkUsage;

		internal short LinkUsagePage;

		internal int IsRange;

		internal int IsStringRange;

		internal int IsDesignatorRange;

		internal int IsAbsolute;

		internal int HasNull;

		internal byte Reserved;

		internal short BitSize;

		internal short ReportCount;

		internal short Reserved2;

		internal short Reserved3;

		internal short Reserved4;

		internal short Reserved5;

		internal short Reserved6;

		internal int LogicalMin;

		internal int LogicalMax;

		internal int PhysicalMin;

		internal int PhysicalMax;

		internal short UsageMin;

		internal short UsageMax;

		internal short StringMin;

		internal short StringMax;

		internal short DesignatorMin;

		internal short DesignatorMax;

		internal short DataIndexMin;

		internal short DataIndexMax;
	}

	internal const uint FILE_ATTRIBUTE_NORMAL = 128u;

	internal const int FILE_FLAG_OVERLAPPED = 1073741824;

	internal const uint FILE_FLAG_NO_BUFFERING = 536870912u;

	internal const uint FILE_FLAG_WRITE_THROUGH = 2147483648u;

	internal const uint FILE_ATTRIBUTE_TEMPORARY = 256u;

	internal const short FILE_SHARE_READ = 1;

	internal const short FILE_SHARE_WRITE = 2;

	internal const uint GENERIC_READ = 2147483648u;

	internal const uint GENERIC_WRITE = 1073741824u;

	internal const int FileShareRead = 1;

	internal const int FileShareWrite = 2;

	internal const int OpenExisting = 3;

	internal const int ACCESS_NONE = 0;

	internal const int INVALID_HANDLE_VALUE = -1;

	internal const short OPEN_EXISTING = 3;

	internal const int WAIT_TIMEOUT = 258;

	internal const uint WAIT_OBJECT_0 = 0u;

	internal const uint WAIT_FAILED = uint.MaxValue;

	internal const int WAIT_INFINITE = 65535;

	internal const int DBT_DEVICEARRIVAL = 32768;

	internal const int DBT_DEVICEREMOVECOMPLETE = 32772;

	internal const int DBT_DEVTYP_DEVICEINTERFACE = 5;

	internal const int DBT_DEVTYP_HANDLE = 6;

	internal const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;

	internal const int DEVICE_NOTIFY_SERVICE_HANDLE = 1;

	internal const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;

	internal const int WM_DEVICECHANGE = 537;

	internal const short DIGCF_PRESENT = 2;

	internal const short DIGCF_DEVICEINTERFACE = 16;

	internal const int DIGCF_ALLCLASSES = 4;

	internal const int DICS_ENABLE = 1;

	internal const int DICS_DISABLE = 2;

	internal const int DICS_FLAG_GLOBAL = 1;

	internal const int DIF_PROPERTYCHANGE = 18;

	internal const int MAX_DEV_LEN = 1000;

	internal const int SPDRP_ADDRESS = 28;

	internal const int SPDRP_BUSNUMBER = 21;

	internal const int SPDRP_BUSTYPEGUID = 19;

	internal const int SPDRP_CAPABILITIES = 15;

	internal const int SPDRP_CHARACTERISTICS = 27;

	internal const int SPDRP_CLASS = 7;

	internal const int SPDRP_CLASSGUID = 8;

	internal const int SPDRP_COMPATIBLEIDS = 2;

	internal const int SPDRP_CONFIGFLAGS = 10;

	internal const int SPDRP_DEVICE_POWER_DATA = 30;

	internal const int SPDRP_DEVICEDESC = 0;

	internal const int SPDRP_DEVTYPE = 25;

	internal const int SPDRP_DRIVER = 9;

	internal const int SPDRP_ENUMERATOR_NAME = 22;

	internal const int SPDRP_EXCLUSIVE = 26;

	internal const int SPDRP_FRIENDLYNAME = 12;

	internal const int SPDRP_HARDWAREID = 1;

	internal const int SPDRP_LEGACYBUSTYPE = 20;

	internal const int SPDRP_LOCATION_INFORMATION = 13;

	internal const int SPDRP_LOWERFILTERS = 18;

	internal const int SPDRP_MFG = 11;

	internal const int SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 14;

	internal const int SPDRP_REMOVAL_POLICY = 31;

	internal const int SPDRP_REMOVAL_POLICY_HW_DEFAULT = 32;

	internal const int SPDRP_REMOVAL_POLICY_OVERRIDE = 33;

	internal const int SPDRP_SECURITY = 23;

	internal const int SPDRP_SECURITY_SDS = 24;

	internal const int SPDRP_SERVICE = 4;

	internal const int SPDRP_UI_NUMBER = 16;

	internal const int SPDRP_UI_NUMBER_DESC_FORMAT = 29;

	internal const int SPDRP_UPPERFILTERS = 17;

	internal static DEVPROPKEY DEVPKEY_Device_BusReportedDeviceDesc = new DEVPROPKEY
	{
		fmtid = new Guid(1410045054u, 35648, 17852, 168, 162, 106, 11, 137, 76, 189, 162),
		pid = 4uL
	};

	internal static DEVPROPKEY DEVPKEY_Device_DeviceDesc = new DEVPROPKEY
	{
		fmtid = new Guid(2757502286u, 57116, 20221, 128, 32, 103, 209, 70, 168, 80, 224),
		pid = 2uL
	};

	internal static DEVPROPKEY DEVPKEY_Device_HardwareIds = new DEVPROPKEY
	{
		fmtid = new Guid(2757502286u, 57116, 20221, 128, 32, 103, 209, 70, 168, 80, 224),
		pid = 3uL
	};

	internal static DEVPROPKEY DEVPKEY_Device_UINumber = new DEVPROPKEY
	{
		fmtid = new Guid(2757502286u, 57116, 20221, 128, 32, 103, 209, 70, 168, 80, 224),
		pid = 18uL
	};

	internal static DEVPROPKEY DEVPKEY_Device_DriverVersion = new DEVPROPKEY
	{
		fmtid = new Guid(2830656989u, 11837, 16532, 173, 151, 229, 147, 167, 12, 117, 214),
		pid = 3uL
	};

	internal static DEVPROPKEY DEVPKEY_Device_Manufacturer = new DEVPROPKEY
	{
		fmtid = new Guid(2757502286u, 57116, 20221, 128, 32, 103, 209, 70, 168, 80, 224),
		pid = 13uL
	};

	internal static DEVPROPKEY DEVPKEY_Device_Provider = new DEVPROPKEY
	{
		fmtid = new Guid(2830656989u, 11837, 16532, 173, 151, 229, 147, 167, 12, 117, 214),
		pid = 9uL
	};

	internal static DEVPROPKEY DEVPKEY_Device_Parent = new DEVPROPKEY
	{
		fmtid = new Guid(1128310469u, 37882, 18182, 151, 44, 123, 100, 128, 8, 165, 167),
		pid = 8uL
	};

	internal static DEVPROPKEY DEVPKEY_Device_Siblings = new DEVPROPKEY
	{
		fmtid = new Guid(1128310469u, 37882, 18182, 151, 44, 123, 100, 128, 8, 165, 167),
		pid = 10uL
	};

	internal static DEVPROPKEY DEVPKEY_Device_InstanceId = new DEVPROPKEY
	{
		fmtid = new Guid(2026065864, 4170, 19146, 158, 164, 82, 77, 82, 153, 110, 87),
		pid = 256uL
	};

	internal const short HIDP_INPUT = 0;

	internal const short HIDP_OUTPUT = 1;

	internal const short HIDP_FEATURE = 2;

	[DllImport("bthprops.cpl", CharSet = CharSet.Auto)]
	internal static extern IntPtr BluetoothFindFirstRadio(ref BLUETOOTH_FIND_RADIO_PARAMS pbtfrp, ref IntPtr phRadio);

	[DllImport("bthprops.cpl", CharSet = CharSet.Auto)]
	internal static extern bool BluetoothFindNextRadio(IntPtr hFind, ref IntPtr phRadio);

	[DllImport("bthprops.cpl", CharSet = CharSet.Auto)]
	internal static extern bool BluetoothFindRadioClose(IntPtr hFind);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern bool DeviceIoControl(IntPtr DeviceHandle, uint IoControlCode, ref long InBuffer, int InBufferSize, IntPtr OutBuffer, int OutBufferSize, ref int BytesReturned, IntPtr Overlapped);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern bool DeviceIoControl(IntPtr DeviceHandle, uint IoControlCode, IntPtr InBuffer, int InBufferSize, IntPtr OutBuffer, int OutBufferSize, ref int BytesReturned, IntPtr Overlapped);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
	internal static extern bool CloseHandle(IntPtr hObject);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "QueryDosDeviceW", SetLastError = true)]
	internal static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
	internal static extern bool CancelIo(IntPtr hFile);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
	internal static extern bool CancelIoEx(IntPtr hFile, IntPtr lpOverlapped);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
	internal static extern bool CancelSynchronousIo(IntPtr hObject);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	internal static extern IntPtr CreateEvent(ref SECURITY_ATTRIBUTES securityAttributes, int bManualReset, int bInitialState, string lpName);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, ref SECURITY_ATTRIBUTES lpSecurityAttributes, int dwCreationDisposition, uint dwFlagsAndAttributes, int hTemplateFile);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, uint dwFlagsAndAttributes, int hTemplateFile);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

	[DllImport("kernel32.dll")]
	internal static extern uint WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

	[DllImport("kernel32.dll")]
	internal static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, [In] ref NativeOverlapped lpOverlapped);

	[DllImport("setupapi.dll")]
	public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, int propertyVal, ref int propertyRegDataType, byte[] propertyBuffer, int propertyBufferSize, ref int requiredSize);

	[DllImport("setupapi.dll", EntryPoint = "SetupDiGetDevicePropertyW", SetLastError = true)]
	public static extern bool SetupDiGetDeviceProperty(IntPtr deviceInfo, ref SP_DEVINFO_DATA deviceInfoData, ref DEVPROPKEY propkey, ref ulong propertyDataType, byte[] propertyBuffer, int propertyBufferSize, ref int requiredSize, uint flags);

	[DllImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceInterfacePropertyW", SetLastError = true)]
	public static extern bool SetupDiGetDeviceInterfaceProperty(IntPtr deviceInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref DEVPROPKEY propkey, ref ulong propertyDataType, byte[] propertyBuffer, int propertyBufferSize, ref int requiredSize, uint flags);

	[DllImport("setupapi.dll")]
	internal static extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet, int memberIndex, ref SP_DEVINFO_DATA deviceInfoData);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	internal static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr notificationFilter, int flags);

	[DllImport("setupapi.dll")]
	internal static extern int SetupDiCreateDeviceInfoList(ref Guid classGuid, int hwndParent);

	[DllImport("setupapi.dll")]
	internal static extern int SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

	[DllImport("setupapi.dll")]
	internal static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, ref Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

	[DllImport("setupapi.dll")]
	internal static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData, ref Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto)]
	internal static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, string enumerator, int hwndParent, int flags);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto)]
	internal static extern IntPtr SetupDiGetClassDevs(IntPtr classGuid, string enumerator, int hwndParent, int flags);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto, EntryPoint = "SetupDiGetDeviceInterfaceDetail")]
	internal static extern bool SetupDiGetDeviceInterfaceDetailBuffer(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, IntPtr deviceInfoData);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto)]
	internal static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, IntPtr deviceInfoData);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto)]
	internal static extern bool SetupDiSetClassInstallParams(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, ref SP_PROPCHANGE_PARAMS classInstallParams, int classInstallParamsSize);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto)]
	internal static extern bool SetupDiCallClassInstaller(int installFunction, IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto)]
	internal static extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, char[] deviceInstanceId, int deviceInstanceIdSize, ref int requiredSize);

	[DllImport("setupapi.dll", SetLastError = true)]
	internal static extern bool SetupDiClassGuidsFromName(string ClassName, ref Guid ClassGuidArray1stItem, uint ClassGuidArraySize, out uint RequiredSize);

	[DllImport("user32.dll")]
	internal static extern bool UnregisterDeviceNotification(IntPtr handle);

	[DllImport("hid.dll")]
	internal static extern bool HidD_FlushQueue(IntPtr hidDeviceObject);

	[DllImport("hid.dll")]
	internal static extern bool HidD_FlushQueue(SafeFileHandle hidDeviceObject);

	[DllImport("hid.dll")]
	internal static extern bool HidD_GetAttributes(IntPtr hidDeviceObject, ref HIDD_ATTRIBUTES attributes);

	[DllImport("hid.dll")]
	internal static extern bool HidD_GetFeature(IntPtr hidDeviceObject, byte[] lpReportBuffer, int reportBufferLength);

	[DllImport("hid.dll", SetLastError = true)]
	internal static extern bool HidD_GetInputReport(SafeFileHandle HidDeviceObject, byte[] lpReportBuffer, int ReportBufferLength);

	[DllImport("hid.dll")]
	internal static extern void HidD_GetHidGuid(ref Guid hidGuid);

	[DllImport("hid.dll")]
	internal static extern bool HidD_GetNumInputBuffers(IntPtr hidDeviceObject, ref int numberBuffers);

	[DllImport("hid.dll")]
	internal static extern bool HidD_GetPreparsedData(IntPtr hidDeviceObject, ref IntPtr preparsedData);

	[DllImport("hid.dll")]
	internal static extern bool HidD_FreePreparsedData(IntPtr preparsedData);

	[DllImport("hid.dll")]
	internal static extern bool HidD_SetFeature(IntPtr hidDeviceObject, byte[] lpReportBuffer, int reportBufferLength);

	[DllImport("hid.dll")]
	internal static extern bool HidD_SetFeature(SafeFileHandle hidDeviceObject, byte[] lpReportBuffer, int reportBufferLength);

	[DllImport("hid.dll")]
	internal static extern bool HidD_SetNumInputBuffers(IntPtr hidDeviceObject, int numberBuffers);

	[DllImport("hid.dll")]
	internal static extern bool HidD_SetOutputReport(IntPtr hidDeviceObject, byte[] lpReportBuffer, int reportBufferLength);

	[DllImport("hid.dll", SetLastError = true)]
	internal static extern bool HidD_SetOutputReport(SafeFileHandle hidDeviceObject, byte[] lpReportBuffer, int reportBufferLength);

	[DllImport("hid.dll")]
	internal static extern int HidP_GetCaps(IntPtr preparsedData, ref HIDP_CAPS capabilities);

	[DllImport("hid.dll")]
	internal static extern int HidP_GetValueCaps(short reportType, ref byte valueCaps, ref short valueCapsLength, IntPtr preparsedData);

	[DllImport("hid.dll")]
	internal static extern bool HidD_GetSerialNumberString(IntPtr HidDeviceObject, byte[] Buffer, uint BufferLength);
}
