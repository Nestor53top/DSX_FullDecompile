using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using HidSharp.Experimental;
using HidSharp.Utility;

namespace HidSharp.Platform.Windows;

internal static class NativeMethods
{
	public enum WM_DEVICECHANGE_wParam
	{
		DBT_CONFIGCHANGECANCELED = 25,
		DBT_CONFIGCHANGED = 24,
		DBT_CUSTOMEVENT = 32774,
		DBT_DEVICEARRIVAL = 32768,
		DBT_DEVICEQUERYREMOVE = 32769,
		DBT_DEVICEQUERYREMOVEFAILED = 32770,
		DBT_DEVICEREMOVECOMPLETE = 32772,
		DBT_DEVICEREMOVEPENDING = 32771,
		DBT_DEVICETYPESPECIFIC = 32773,
		DBT_DEVNODES_CHANGED = 7,
		DBT_QUERYCHANGECONFIG = 23,
		DBT_USERDEFINED = 65535
	}

	[Flags]
	public enum EFileAccess : uint
	{
		None = 0u,
		Read = 0x80000000u,
		Write = 0x40000000u,
		Execute = 0x20000000u,
		All = 0x10000000u
	}

	[Flags]
	public enum EFileShare : uint
	{
		None = 0u,
		Read = 1u,
		Write = 2u,
		Delete = 4u,
		All = 7u
	}

	public enum ECreationDisposition : uint
	{
		New = 1u,
		CreateAlways,
		OpenExisting,
		OpenAlways,
		TruncateExisting
	}

	[Flags]
	public enum EFileAttributes : uint
	{
		Readonly = 1u,
		Hidden = 2u,
		System = 4u,
		Directory = 0x10u,
		Archive = 0x20u,
		Device = 0x40u,
		Normal = 0x80u,
		Temporary = 0x100u,
		SparseFile = 0x200u,
		ReparsePoint = 0x400u,
		Compressed = 0x800u,
		Offline = 0x1000u,
		NotContentIndexed = 0x2000u,
		Encrypted = 0x4000u,
		Writethrough = 0x80000000u,
		Overlapped = 0x40000000u,
		NoBuffering = 0x20000000u,
		RandomAccess = 0x10000000u,
		SequentialScan = 0x8000000u,
		DeleteOnClose = 0x4000000u,
		BackupSemantics = 0x2000000u,
		PosixSemantics = 0x1000000u,
		OpenReparsePoint = 0x200000u,
		OpenNoRecall = 0x100000u,
		FirstPipeInstance = 0x80000u
	}

	[Flags]
	public enum DIGCF
	{
		None = 0,
		Default = 1,
		Present = 2,
		AllClasses = 4,
		Profile = 8,
		DeviceInterface = 0x10
	}

	[Flags]
	public enum SPINT
	{
		None = 0,
		Active = 1,
		Default = 2,
		Removed = 4
	}

	public struct HDEVINFO
	{
		private IntPtr Value;

		public bool IsValid => Value != (IntPtr)(-1);

		public void Invalidate()
		{
			Value = (IntPtr)(-1);
		}
	}

	public struct POINT
	{
		public int X;

		public int Y;
	}

	public struct MSG
	{
		public IntPtr Window;

		public uint Message;

		public IntPtr WParam;

		public IntPtr LParam;

		public uint Time;

		public POINT Point;
	}

	public struct WNDCLASS
	{
		public uint Style;

		public WindowProc WindowProc;

		public int ClassExtra;

		public int WindowExtra;

		public IntPtr Instance;

		public IntPtr Icon;

		public IntPtr Cursor;

		public IntPtr Background;

		public string MenuName;

		public string ClassName;
	}

	public struct DEV_BROADCAST_HDR
	{
		public int Size;

		public int DeviceType;

		public int Reserved;
	}

	public struct DEV_BROADCAST_DEVICEINTERFACE
	{
		public int Size;

		public int DeviceType;

		public int Reserved;

		public Guid ClassGuid;

		public char Name;
	}

	public struct DEV_BROADCAST_HANDLE
	{
		public int Size;

		public int DeviceType;

		public int Reserved;

		public IntPtr DeviceHandle;

		public IntPtr NotifyHandle;

		public Guid EventGuid;

		public int NameOffset;

		public unsafe fixed byte Data[1];
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct OSVERSIONINFO
	{
		public int OSVersionInfoSize;

		public uint MajorVersion;

		public uint MinorVersion;

		public uint BuildNumber;

		public uint PlatformID;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string CSDVersion;
	}

	public struct SetupPacket
	{
		public byte bmRequest;

		public byte bRequest;

		public ushort wValue;

		public ushort wIndex;

		public ushort wLength;
	}

	public struct USB_DESCRIPTOR_REQUEST
	{
		public uint ConnectionIndex;

		public SetupPacket SetupPacket;
	}

	public struct SP_DEVINFO_DATA
	{
		public int Size;

		public Guid ClassGuid;

		public uint DevInst;

		private IntPtr Reserved;
	}

	public struct SP_DEVICE_INTERFACE_DATA
	{
		public int Size;

		public Guid InterfaceClassGuid;

		public SPINT Flags;

		private IntPtr Reserved;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	[Obfuscation(Exclude = true)]
	public struct SP_DEVICE_INTERFACE_DETAIL_DATA
	{
		public int Size;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string DevicePath;
	}

	public enum HIDP_REPORT_TYPE
	{
		Input,
		Output,
		Feature,
		Count
	}

	public struct HIDD_ATTRIBUTES
	{
		public int Size;

		public ushort VendorID;

		public ushort ProductID;

		public ushort VersionNumber;
	}

	public struct HIDP_CAPS
	{
		public ushort Usage;

		public ushort UsagePage;

		public ushort InputReportByteLength;

		public ushort OutputReportByteLength;

		public ushort FeatureReportByteLength;

		private unsafe fixed ushort Reserved[17];

		public ushort NumberLinkCollectionNodes;

		public ushort NumberInputButtonCaps;

		public ushort NumberInputValueCaps;

		public ushort NumberInputDataIndices;

		public ushort NumberOutputButtonCaps;

		public ushort NumberOutputValueCaps;

		public ushort NumberOutputDataIndices;

		public ushort NumberFeatureButtonCaps;

		public ushort NumberFeatureValueCaps;

		public ushort NumberFeatureDataIndices;
	}

	public struct HIDP_LINK_COLLECTION_NODE
	{
		public ushort LinkUsage;

		public ushort LinkUsagePage;

		public ushort Parent;

		public ushort NumberOfChildren;

		public ushort NextSibling;

		public ushort FirstChild;

		public byte CollectionType;

		private byte IsAliasByte;

		private unsafe fixed byte Reserved[2];

		public IntPtr UserContext;

		public byte IsAlias => (byte)(IsAliasByte & 1);
	}

	public struct HIDP_DATA
	{
		public ushort DataIndex;

		private ushort Reserved;

		public uint RawValue;
	}

	[StructLayout(LayoutKind.Sequential, Size = 72)]
	public struct HIDP_DATA_CAPS
	{
		public ushort UsagePage;

		public byte ReportID;

		public byte IsAlias;

		public ushort BitField;

		public ushort LinkCollection;

		public ushort LinkUsage;

		public ushort LinkUsagePage;

		public byte IsRange;

		public byte IsStringRange;

		public byte IsDesignatorRange;

		public byte IsAbsolute;

		public byte VALUE_HasNull;

		private byte Reserved;

		public ushort VALUE_ReportSize;

		public ushort VALUE_ReportCount;

		private unsafe fixed ushort Reserved2[5];

		public uint VALUE_UnitsExp;

		public uint VALUE_Units;

		public int VALUE_LogicalMin;

		public int VALUE_LogicalMax;

		public int VALUE_PhysicalMin;

		public int VALUE_PhysicalMax;

		public ushort UsageIndex;

		public ushort UsageMax;

		public ushort StringIndex;

		public ushort StringMax;

		public ushort DesignatorIndex;

		public ushort DesignatorMax;

		public ushort DataIndex;

		public ushort DataIndexMax;
	}

	public struct USB_NODE_CONNECTION_DRIVERKEY_NAME
	{
		public uint ConnectionIndex;

		public uint ActualLength;

		public unsafe fixed char NodeName[1024];
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct USB_DEVICE_DESCRIPTOR
	{
		public byte bLength;

		public byte bDescriptorType;

		public ushort bcdUSB;

		public byte bDeviceClass;

		public byte bDeviceSubClass;

		public byte bDeviceProtocol;

		public byte bMaxPacketSize0;

		public ushort idVendor;

		public ushort idProduct;

		public ushort bcdDevice;

		public byte iManufacturer;

		public byte iProduct;

		public byte iSerialNumber;

		public byte bNumConfigurations;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct USB_ENDPOINT_DESCRIPTOR
	{
		public byte bLength;

		public byte bDescriptorType;

		public byte bEndpointAddress;

		public byte bmAttributes;

		public ushort wMaxPacketSize;

		public byte bInterval;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct USB_PIPE_INFO
	{
		public USB_ENDPOINT_DESCRIPTOR EndpointDescriptor;

		public uint ScheduleOffset;
	}

	public enum USB_CONNECTION_STATUS
	{
		NoDeviceConnected,
		DeviceConnected
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct USB_NODE_CONNECTION_INFORMATION
	{
		public uint ConnectionIndex;

		public USB_DEVICE_DESCRIPTOR DeviceDescriptor;

		public byte CurrentConfigurationValue;

		public byte LowSpeed;

		public byte DeviceIsHub;

		public ushort DeviceAddress;

		public uint NumberOfOpenPipes;

		public USB_CONNECTION_STATUS ConnectionStatus;

		public USB_PIPE_INFO PipeInfo;
	}

	public struct DCB
	{
		public int DCBlength;

		public uint BaudRate;

		public uint fFlags;

		private ushort Reserved1;

		public ushort XonLim;

		public ushort XoffLim;

		public byte ByteSize;

		public byte Parity;

		public byte StopBits;

		public byte XonChar;

		public byte XoffChar;

		public byte ErrorChar;

		public byte EofChar;

		public byte EvtChar;

		private ushort Reserved2;

		public bool fBinary
		{
			get
			{
				return GetBool(0);
			}
			set
			{
				SetBool(0, value);
			}
		}

		public bool fParity
		{
			get
			{
				return GetBool(1);
			}
			set
			{
				SetBool(1, value);
			}
		}

		public bool fOutxCtsFlow
		{
			get
			{
				return GetBool(2);
			}
			set
			{
				SetBool(2, value);
			}
		}

		public bool fOutxDsrFlow
		{
			get
			{
				return GetBool(3);
			}
			set
			{
				SetBool(3, value);
			}
		}

		public uint fDtrControl
		{
			get
			{
				return GetBits(4, 2);
			}
			set
			{
				SetBits(4, 2, value);
			}
		}

		public bool fDsrSensitivity
		{
			get
			{
				return GetBool(6);
			}
			set
			{
				SetBool(6, value);
			}
		}

		public bool fTXContinueOnXoff
		{
			get
			{
				return GetBool(7);
			}
			set
			{
				SetBool(7, value);
			}
		}

		public bool fOutX
		{
			get
			{
				return GetBool(8);
			}
			set
			{
				SetBool(8, value);
			}
		}

		public bool fInX
		{
			get
			{
				return GetBool(9);
			}
			set
			{
				SetBool(9, value);
			}
		}

		public bool fErrorChar
		{
			get
			{
				return GetBool(10);
			}
			set
			{
				SetBool(10, value);
			}
		}

		public bool fNull
		{
			get
			{
				return GetBool(11);
			}
			set
			{
				SetBool(11, value);
			}
		}

		public uint fRtsControl
		{
			get
			{
				return GetBits(12, 2);
			}
			set
			{
				SetBits(12, 2, value);
			}
		}

		public bool fAbortOnError
		{
			get
			{
				return GetBool(14);
			}
			set
			{
				SetBool(14, value);
			}
		}

		private static uint GetBitMask(int bitCount)
		{
			return (uint)((1 << bitCount) - 1);
		}

		private uint GetBits(int bitOffset, int bitCount)
		{
			return (fFlags >> bitOffset) & GetBitMask(bitCount);
		}

		private void SetBits(int bitOffset, int bitCount, uint value)
		{
			uint bitMask = GetBitMask(bitCount);
			fFlags &= ~(bitMask << bitOffset);
			fFlags |= (value & bitMask) << bitOffset;
		}

		private bool GetBool(int bitOffset)
		{
			return GetBits(bitOffset, 1) != 0;
		}

		private void SetBool(int bitOffset, bool value)
		{
			SetBits(bitOffset, 1, value ? 1u : 0u);
		}
	}

	public struct COMMTIMEOUTS
	{
		public uint ReadIntervalTimeout;

		public uint ReadTotalTimeoutMultiplier;

		public uint ReadTotalTimeoutConstant;

		public uint WriteTotalTimeoutMultiplier;

		public uint WriteTotalTimeoutConstant;
	}

	[StructLayout(LayoutKind.Explicit, Size = 20)]
	public struct BTH_LE_UUID
	{
		[FieldOffset(0)]
		[MarshalAs(UnmanagedType.I1)]
		public byte IsShortUuid;

		[FieldOffset(4)]
		public ushort ShortUuid;

		[FieldOffset(4)]
		public Guid LongUuid;

		public BleUuid ToGuid()
		{
			if (IsShortUuid == 0)
			{
				return new BleUuid(LongUuid);
			}
			return new BleUuid(ShortUuid);
		}
	}

	public struct BTH_LE_GATT_SERVICE
	{
		public BTH_LE_UUID ServiceUuid;

		public ushort AttributeHandle;
	}

	[StructLayout(LayoutKind.Explicit, Size = 36)]
	public struct BTH_LE_GATT_CHARACTERISTIC
	{
		public const int Size = 36;

		[FieldOffset(0)]
		public ushort ServiceHandle;

		[FieldOffset(4)]
		public BTH_LE_UUID CharacteristicUuid;

		[FieldOffset(24)]
		public ushort AttributeHandle;

		[FieldOffset(26)]
		public ushort CharacteristicValueHandle;

		[FieldOffset(28)]
		public byte IsBroadcastable;

		[FieldOffset(29)]
		public byte IsReadable;

		[FieldOffset(30)]
		public byte IsWritable;

		[FieldOffset(31)]
		public byte IsWritableWithoutResponse;

		[FieldOffset(32)]
		public byte IsSignedWritable;

		[FieldOffset(33)]
		public byte IsNotifiable;

		[FieldOffset(34)]
		public byte IsIndicatable;

		[FieldOffset(35)]
		public byte HasExtendedProperties;
	}

	public struct BLUETOOTH_GATT_VALUE_CHANGED_EVENT
	{
		public ushort ChangedAttributeHandle;

		public UIntPtr CharacteristicValueDataSize;

		public unsafe BTH_LE_GATT_CHARACTERISTIC_VALUE* CharacteristicValue;
	}

	[StructLayout(LayoutKind.Explicit, Size = 4)]
	public struct BLUETOOTH_GATT_VALUE_CHANGED_EVENT_REGISTRATION
	{
		public const int Size = 4;

		[FieldOffset(0)]
		public ushort NumCharacteristics;
	}

	[StructLayout(LayoutKind.Explicit, Size = 4)]
	public struct BTH_LE_GATT_CHARACTERISTIC_VALUE
	{
		public const int Size = 4;

		[FieldOffset(0)]
		public uint DataSize;

		[FieldOffset(4)]
		public unsafe fixed byte Data[1];
	}

	public struct BTH_LE_GATT_DESCRIPTOR
	{
		public ushort ServiceHandle;

		public ushort CharacteristicHandle;

		public BTH_LE_GATT_DESCRIPTOR_TYPE DescriptorType;

		public BTH_LE_UUID DescriptorUuid;

		public ushort AttributeHandle;
	}

	public enum BTH_LE_GATT_DESCRIPTOR_TYPE
	{
		CharacteristicExtendedProperties,
		CharacteristicUserDescription,
		ClientCharacteristicConfiguration,
		ServerCharacteristicConfiguration,
		CharacteristicFormat,
		CharacteristicAggregateFormat,
		CustomDescriptor
	}

	public struct BTH_LE_GATT_DESCRIPTOR_VALUE_EXTENDED_PROPERTIES
	{
		public byte IsReliableWriteEnabled;

		public byte IsAuxiliariesWritable;
	}

	public struct BTH_LE_GATT_DESCRIPTOR_VALUE_CCCD
	{
		public byte IsSubscribeToNotification;

		public byte IsSubscribeToIndication;
	}

	public struct BTH_LE_GATT_DESCRIPTOR_VALUE_SCCD
	{
		public byte IsBroadcast;
	}

	[StructLayout(LayoutKind.Explicit, Size = 48)]
	public struct BTH_LE_GATT_DESCRIPTOR_VALUE_FORMAT
	{
		[FieldOffset(0)]
		public byte Format;

		[FieldOffset(1)]
		public byte Exponent;

		[FieldOffset(4)]
		public BTH_LE_UUID Unit;

		[FieldOffset(24)]
		public byte Namespace;

		[FieldOffset(28)]
		public BTH_LE_UUID Description;
	}

	[StructLayout(LayoutKind.Explicit, Size = 48)]
	public struct BTH_LE_GATT_DESCRIPTOR_VALUE_PARAMS
	{
		[FieldOffset(0)]
		public BTH_LE_GATT_DESCRIPTOR_VALUE_EXTENDED_PROPERTIES ExtendedProperties;

		[FieldOffset(0)]
		public BTH_LE_GATT_DESCRIPTOR_VALUE_CCCD Cccd;

		[FieldOffset(0)]
		public BTH_LE_GATT_DESCRIPTOR_VALUE_SCCD Sccd;

		[FieldOffset(0)]
		public BTH_LE_GATT_DESCRIPTOR_VALUE_FORMAT Format;
	}

	public struct BTH_LE_GATT_DESCRIPTOR_VALUE
	{
		public const int Size = 76;

		public BTH_LE_GATT_DESCRIPTOR_TYPE DescriptorType;

		public BTH_LE_UUID DescriptorUuid;

		public BTH_LE_GATT_DESCRIPTOR_VALUE_PARAMS Params;

		public BTH_LE_GATT_CHARACTERISTIC_VALUE Value;
	}

	public enum BTH_LE_GATT_EVENT_TYPE
	{
		CharacteristicValueChangedEvent
	}

	public unsafe delegate void BLUETOOTH_GATT_EVENT_CALLBACK(BTH_LE_GATT_EVENT_TYPE eventType, BLUETOOTH_GATT_VALUE_CHANGED_EVENT* eventParameter, IntPtr context);

	[Flags]
	public enum BLUETOOTH_GATT_FLAGS : uint
	{
		ENCRYPTED = 1u,
		AUTHENTICATED = 2u,
		FORCE_READ_FROM_DEVICE = 4u,
		FORCE_READ_FROM_CACHE = 8u,
		SIGNED_WRITE = 0x10u,
		WRITE_WITHOUT_RESPONSE = 0x20u
	}

	public struct BLUETOOTH_FIND_RADIO_PARAMS
	{
		public int Size;
	}

	public struct BLUETOOTH_DEVICE_SEARCH_PARAMS
	{
		public int dwSize;

		public int fReturnAuthenticated;

		public int fReturnRemembered;

		public int fReturnUnknown;

		public int fReturnConnected;

		public int fIssueInquiry;

		public byte cTimeoutMultiplier;

		public IntPtr hRadio;
	}

	public struct BLUETOOTH_DEVICE_INFO
	{
		public int dwSize;

		public BLUETOOTH_ADDRESS Address;

		public uint ulClassOfDevice;

		public int fConnected;

		public int fRemembered;

		public int fAuthenticated;

		public SYSTEMTIME stLastSeen;

		public SYSTEMTIME stLastUsed;

		public unsafe fixed char szName[248];
	}

	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public struct BLUETOOTH_ADDRESS
	{
		[FieldOffset(0)]
		public ulong Addr;

		[FieldOffset(0)]
		public unsafe fixed byte Bytes[6];
	}

	public struct BTH_DEVICE_INFO
	{
		public BDIF flags;

		public ulong address;

		public uint classOfDevice;

		public unsafe fixed byte name[248];
	}

	public struct BTH_HCI_EVENT_INFO
	{
		public ulong bthAddress;

		public byte connectionType;

		public byte connected;
	}

	public struct BTH_RADIO_IN_RANGE
	{
		public BTH_DEVICE_INFO deviceInfo;

		public BDIF previousDeviceFlags;
	}

	public struct SYSTEMTIME
	{
		public ushort wYear;

		public ushort wMonth;

		public ushort wDayOfWeek;

		public ushort wDay;

		public ushort wHour;

		public ushort wMinute;

		public ushort wSecond;

		public ushort wMilliseconds;
	}

	[Flags]
	public enum BDIF : uint
	{
		Address = 1u,
		Cod = 2u,
		Name = 4u,
		Paired = 8u,
		Personal = 0x10u,
		Connected = 0x20u,
		ShortName = 0x40u,
		Visible = 0x80u,
		SspSupported = 0x100u,
		SspPaired = 0x200u,
		SspMitmProtected = 0x400u,
		Rssi = 0x1000u,
		Eir = 0x2000u,
		Br = 0x4000u,
		Le = 0x8000u,
		LePaired = 0x10000u,
		LePersonal = 0x20000u,
		LeMitmProtected = 0x40000u,
		LePrivacyEnabled = 0x80000u,
		LeRandomAddress = 0x100000u,
		LeDiscoverable = 0x200000u,
		LeName = 0x400000u,
		Unknown1 = 0x1000000u,
		Unknown2 = 0x2000000u
	}

	public delegate IntPtr WindowProc(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

	public delegate void EnumerateDeviceInterfacesCallback(HDEVINFO deviceInfoSet, SP_DEVINFO_DATA deviceInfoData, SP_DEVICE_INTERFACE_DATA deviceInterfaceData, string deviceID, string devicePath);

	public delegate void EnumerateDevicesCallback(HDEVINFO deviceInfoSet, SP_DEVINFO_DATA deviceInfoData, string deviceID);

	public const int DICS_FLAG_GLOBAL = 1;

	public const int DIREG_DEV = 1;

	public const int ERROR_GEN_FAILURE = 31;

	public const int ERROR_HANDLE_EOF = 38;

	public const int ERROR_INSUFFICIENT_BUFFER = 122;

	public const int ERROR_NO_MORE_ITEMS = 259;

	public const int ERROR_OPERATION_ABORTED = 995;

	public const int ERROR_IO_PENDING = 997;

	public const int ERROR_INVALID_PARAMETER = -2147024809;

	public const int ERROR_MORE_DATA = -2147024662;

	public const int ERROR_NOT_FOUND = -2147023728;

	public const uint FILE_ANY_ACCESS = 0u;

	public const uint FILE_DEVICE_KEYBOARD = 11u;

	public const uint FILE_DEVICE_UNKNOWN = 34u;

	public const uint HKEY_CURRENT_USER = 2147483649u;

	public const uint HKEY_LOCAL_MACHINE = 2147483650u;

	public const uint KEY_ALL_ACCESS = 983103u;

	public const uint KEY_NOTIFY = 16u;

	public const uint KEY_READ = 131097u;

	public const uint KEY_WRITE = 131078u;

	public const uint METHOD_BUFFERED = 0u;

	public const uint METHOD_NEITHER = 3u;

	public const uint REG_NOTIFY_CHANGE_NAME = 1u;

	public const uint REG_NOTIFY_CHANGE_LAST_SET = 4u;

	public const uint REG_SZ = 1u;

	public const uint SPDRP_DEVICEDESC = 0u;

	public const uint SPDRP_HARDWAREID = 1u;

	public const uint SPDRP_FRIENDLYNAME = 12u;

	public const uint SPDRP_ADDRESS = 28u;

	public const uint WAIT_OBJECT_0 = 0u;

	public const uint WAIT_OBJECT_1 = 1u;

	public const uint WAIT_TIMEOUT = 258u;

	public const uint WM_DEVICECHANGE = 537u;

	public const uint RTS_CONTROL_DISABLE = 0u;

	public const uint RTS_CONTROL_ENABLE = 1u;

	public const uint RTS_CONTROL_HANDSHAKE = 2u;

	public const uint RTS_CONTROL_TOGGLE = 3u;

	public const byte NOPARITY = 0;

	public const byte ODDPARITY = 1;

	public const byte EVENPARITY = 2;

	public const byte MARKPARITY = 3;

	public const byte SPACEPARITY = 4;

	public const byte ONESTOPBIT = 0;

	public const byte ONE5STOPBITS = 1;

	public const byte TWOSTOPBITS = 2;

	public const uint PURGE_TXABORT = 1u;

	public const uint PURGE_RXABORT = 2u;

	public const uint PURGE_TXCLEAR = 4u;

	public const uint PURGE_RXCLEAR = 8u;

	public const int DN_DEVICE_DISCONNECTED = 33554432;

	public const uint CM_DRP_DRIVER = 10u;

	public const int BLUETOOTH_MAX_NAME_SIZE = 248;

	public const int CW_USEDEFAULT = int.MinValue;

	public const int DBT_DEVTYP_DEVICEINTERFACE = 5;

	public const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;

	public const int DBT_DEVTYP_HANDLE = 6;

	public static readonly Guid GuidForBluetoothLEDevice = new Guid("{781AEE18-7733-4CE4-ADD0-91F41C67B592}");

	public static readonly Guid GuidForBluetoothHciEvent = new Guid("{FC240062-1541-49BE-B463-84C4DCD7BF7F}");

	public static readonly Guid GuidForBluetoothRadioInRange = new Guid("{EA3B5B82-26EE-450E-B0D8-D26FE30A3869}");

	public static readonly Guid GuidForBluetoothRadioOutOfRange = new Guid("{E28867C9-C2AA-4CED-B969-4570866037C4}");

	public static readonly Guid GuidForComPort = new Guid("{86E0D1E0-8089-11D0-9CE4-08003E301F73}");

	public static readonly Guid GuidForPortsClass = new Guid("{4D36E978-E325-11CE-BFC1-08002BE10318}");

	public static readonly Guid GuidForUsbHub = new Guid("{F18A0E88-C30C-11D0-8815-00A0C906BED8}");

	public static readonly int HIDP_STATUS_SUCCESS = HIDP_ERROR_CODES(0, 0);

	public static readonly int HIDP_STATUS_INVALID_PREPARSED_DATA = HIDP_ERROR_CODES(12, 1);

	public static readonly int HIDP_STATUS_USAGE_NOT_FOUND = HIDP_ERROR_CODES(12, 4);

	public static readonly int HIDP_STATUS_IS_VALUE_ARRAY = HIDP_ERROR_CODES(12, 12);

	public static readonly uint IOCTL_HID_GET_REPORT_DESCRIPTOR = HID_CTL_CODE(1u);

	public static readonly uint IOCTL_USB_GET_NODE_INFORMATION = CTL_CODE(34u, 258u, 0u, 0u);

	public static readonly uint IOCTL_USB_GET_NODE_CONNECTION_INFORMATION = CTL_CODE(34u, 259u, 0u, 0u);

	public static readonly uint IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION = CTL_CODE(34u, 260u, 0u, 0u);

	public static readonly uint IOCTL_USB_GET_NODE_CONNECTION_NAME = CTL_CODE(34u, 261u, 0u, 0u);

	public static readonly uint IOCTL_USB_GET_NODE_CONNECTION_DRIVERKEY_NAME = CTL_CODE(34u, 264u, 0u, 0u);

	public static readonly IntPtr HWND_MESSAGE = (IntPtr)(-3);

	public static uint CTL_CODE(uint devType, uint func, uint method, uint access)
	{
		return (devType << 16) | (access << 14) | (func << 2) | method;
	}

	public static uint HID_CTL_CODE(uint id)
	{
		return CTL_CODE(11u, id, 3u, 0u);
	}

	public static int HIDP_ERROR_CODES(int sev, ushort code)
	{
		return (sev << 28) | 0x110000 | code;
	}

	public static IntPtr CreateAutoResetEventOrThrow()
	{
		return CreateResetEventOrThrow(manualReset: false);
	}

	public static IntPtr CreateManualResetEventOrThrow()
	{
		return CreateResetEventOrThrow(manualReset: true);
	}

	private static IntPtr CreateResetEventOrThrow(bool manualReset)
	{
		IntPtr intPtr = CreateEvent(IntPtr.Zero, manualReset, initialState: false, IntPtr.Zero);
		if (intPtr == IntPtr.Zero)
		{
			throw new IOException("Event creation failed.");
		}
		return intPtr;
	}

	public unsafe static void OverlappedOperation(IntPtr ioHandle, IntPtr eventHandle, int eventTimeout, IntPtr closeEventHandle, bool overlapResult, NativeOverlapped* overlapped, out uint bytesTransferred)
	{
		bool flag = false;
		if (!overlapResult)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 997)
			{
				Win32Exception ex = new Win32Exception();
				throw new IOException($"Operation failed early: {ex.Message}", ex);
			}
			IntPtr* ptr = stackalloc IntPtr[2];
			*ptr = eventHandle;
			ptr[1] = closeEventHandle;
			switch (WaitForMultipleObjects(2u, ptr, waitAll: false, WaitForMultipleObjectsGetTimeout(eventTimeout)))
			{
			case 1u:
				flag = true;
				goto default;
			default:
				CancelIo(ioHandle);
				break;
			case 0u:
				break;
			}
		}
		if (GetOverlappedResult(ioHandle, overlapped, out bytesTransferred, wait: true))
		{
			return;
		}
		int lastWin32Error2 = Marshal.GetLastWin32Error();
		if (lastWin32Error2 != 38)
		{
			if (flag)
			{
				throw CommonException.CreateClosedException();
			}
			if (lastWin32Error2 == 995)
			{
				throw new TimeoutException("Operation timed out.");
			}
			throw new IOException("Operation failed after some time.", new Win32Exception());
		}
		bytesTransferred = 0u;
	}

	[DllImport("cfgmgr32.dll")]
	public static extern int CM_Get_Child(out uint childDevInst, uint devInst, int flags = 0);

	public static int CM_Get_Device_ID(uint devInst, out string deviceID)
	{
		deviceID = null;
		int num = CM_Get_Device_ID_Size(out var length, devInst);
		if (num != 0)
		{
			return num;
		}
		char[] array = new char[length + 1];
		num = CM_Get_Device_ID(devInst, array, array.Length);
		if (num != 0)
		{
			return num;
		}
		deviceID = new string(array, 0, length);
		return 0;
	}

	[DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
	public static extern int CM_Get_Device_ID(uint devInst, char[] buffer, int length, int flags = 0);

	[DllImport("cfgmgr32.dll")]
	public static extern int CM_Get_Device_ID_Size(out int length, uint devInst, int flags = 0);

	[DllImport("cfgmgr32.dll")]
	public static extern int CM_Get_Parent(out uint parentDevInst, uint devInst, int flags = 0);

	[DllImport("cfgmgr32.dll")]
	public static extern int CM_Get_Sibling(out uint siblingDevInst, uint devInst, int flags = 0);

	[DllImport("cfgmgr32.dll")]
	public static extern int CM_Get_DevNode_Status(out uint status, out uint problemNumber, uint devInst, int flags = 0);

	[DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
	public static extern int CM_Locate_DevNode(out uint devInst, string deviceID, int flags = 0);

	[DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
	public unsafe static extern int CM_Get_DevNode_Registry_Property(uint devInst, uint property, uint* dataType, void* buffer, ref uint length, uint flags);

	[DllImport("cfgmgr32.dll")]
	public static extern uint CMP_WaitNoPendingInstallEvents(uint timeout);

	[DllImport("user32.dll")]
	public static extern ushort RegisterClass(ref WNDCLASS windowClass);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr CreateWindowEx(uint exStyle, string className, string windowName, uint style, int x, int y, int width, int height, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr parameter);

	[DllImport("user32.dll")]
	public static extern IntPtr DefWindowProc(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll")]
	public static extern int GetMessage(out MSG message, IntPtr window, uint messageMin, uint messageMax);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool TranslateMessage(ref MSG message);

	[DllImport("user32.dll")]
	public static extern IntPtr DispatchMessage(ref MSG message);

	[DllImport("user32.dll")]
	public static extern IntPtr RegisterDeviceNotification(IntPtr recipient, ref DEV_BROADCAST_DEVICEINTERFACE notificationFilter, int flags);

	[DllImport("user32.dll")]
	public static extern IntPtr RegisterDeviceNotification(IntPtr recipient, ref DEV_BROADCAST_HANDLE notificationFilter, int flags);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool UnregisterDeviceNotification(IntPtr handle);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool DestroyWindow(IntPtr window);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool UnregisterClass(string className, IntPtr instance);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetVersionEx(ref OSVERSIONINFO version);

	[DllImport("hid.dll")]
	public static extern void HidD_GetHidGuid(out Guid hidGuid);

	public static Guid HidD_GetHidGuid()
	{
		HidD_GetHidGuid(out var hidGuid);
		return hidGuid;
	}

	[DllImport("hid.dll")]
	[return: MarshalAs(UnmanagedType.U1)]
	public static extern bool HidD_GetAttributes(IntPtr handle, ref HIDD_ATTRIBUTES attributes);

	[DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static extern bool HidD_GetManufacturerString(IntPtr handle, char[] buffer, int bufferLengthInBytes);

	[DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static extern bool HidD_GetProductString(IntPtr handle, char[] buffer, int bufferLengthInBytes);

	[DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static extern bool HidD_GetSerialNumberString(IntPtr handle, char[] buffer, int bufferLengthInBytes);

	[DllImport("hid.dll", CharSet = CharSet.Auto)]
	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe static extern bool HidD_GetFeature(IntPtr handle, byte* buffer, int bufferLength);

	[DllImport("hid.dll", CharSet = CharSet.Auto)]
	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe static extern bool HidD_SetFeature(IntPtr handle, byte* buffer, int bufferLength);

	[DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static extern bool HidD_SetNumInputBuffers(IntPtr handle, int count);

	[DllImport("hid.dll", CharSet = CharSet.Auto)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static extern bool HidD_GetPreparsedData(IntPtr handle, out IntPtr preparsed);

	[DllImport("hid.dll", CharSet = CharSet.Auto)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static extern bool HidD_FreePreparsedData(IntPtr preparsed);

	[DllImport("hid.dll", CharSet = CharSet.Auto)]
	public static extern int HidP_GetCaps(IntPtr preparsed, out HIDP_CAPS caps);

	[DllImport("hid.dll", CharSet = CharSet.Auto)]
	public static extern int HidP_SetData(HIDP_REPORT_TYPE reportType, ref HIDP_DATA dataList, ref int dataCount, IntPtr preparsed, byte[] report, int reportLength);

	[DllImport("hid.dll", CharSet = CharSet.Auto)]
	public static extern int HidP_SetUsages(HIDP_REPORT_TYPE reportType, ushort usagePage, ushort linkCollection, ref ushort usage, ref int usageCount, IntPtr preparsed, byte[] report, int reportLength);

	[DllImport("hid.dll", CharSet = CharSet.Auto)]
	public static extern int HidP_SetUsageValueArray(HIDP_REPORT_TYPE reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] usageValue, ushort usageValueLength, IntPtr preparsed, byte[] report, int reportLength);

	[DllImport("hid.dll", CharSet = CharSet.Auto)]
	public static extern int HidP_GetLinkCollectionNodes([Out] HIDP_LINK_COLLECTION_NODE[] nodes, ref int count, IntPtr preparsed);

	[DllImport("hid.dll", CharSet = CharSet.Auto)]
	public static extern int HidP_GetButtonCaps(HIDP_REPORT_TYPE reportType, [Out] HIDP_DATA_CAPS[] buttons, ref ushort count, IntPtr preparsed);

	[DllImport("hid.dll", CharSet = CharSet.Auto)]
	public static extern int HidP_GetValueCaps(HIDP_REPORT_TYPE reportType, [Out] HIDP_DATA_CAPS[] values, ref ushort count, IntPtr preparsed);

	[DllImport("advapi32.dll", SetLastError = true)]
	public static extern int RegOpenKeyEx(IntPtr parentHandle, string subkey, uint options, uint access, out IntPtr handle);

	[DllImport("advapi32.dll", SetLastError = true)]
	public static extern int RegNotifyChangeKeyValue(IntPtr handle, bool watchSubtree, uint notifyFilter, IntPtr @event, bool asynchronous);

	[DllImport("advapi32.dll", SetLastError = true)]
	public static extern int RegCloseKey(IntPtr handle);

	[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern int RegQueryValueEx(IntPtr handle, string valueName, uint reserved, IntPtr type, char[] buffer, ref int lengthInBytes);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern HDEVINFO SetupDiGetClassDevs(IntPtr classGuid, string enumerator, IntPtr hwndParent, DIGCF flags);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern HDEVINFO SetupDiGetClassDevs([MarshalAs(UnmanagedType.LPStruct)] Guid classGuid, string enumerator, IntPtr hwndParent, DIGCF flags);

	[DllImport("setupapi.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetupDiDestroyDeviceInfoList(HDEVINFO deviceInfoSet);

	[DllImport("setupapi.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetupDiEnumDeviceInfo(HDEVINFO deviceInfoSet, int memberIndex, ref SP_DEVINFO_DATA deviceInfoData);

	[DllImport("setupapi.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetupDiEnumDeviceInterfaces(HDEVINFO deviceInfoSet, IntPtr deviceInfoData, [MarshalAs(UnmanagedType.LPStruct)] Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

	[DllImport("setupapi.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetupDiEnumDeviceInterfaces(HDEVINFO deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, [MarshalAs(UnmanagedType.LPStruct)] Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetupDiGetDeviceInterfaceDetail(HDEVINFO deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, IntPtr requiredSize, IntPtr deviceInfoData);

	public static bool SetupDiGetDeviceInterfaceDetail(HDEVINFO deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, out SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData)
	{
		deviceInterfaceDetailData = default(SP_DEVICE_INTERFACE_DETAIL_DATA);
		deviceInterfaceDetailData.Size = ((IntPtr.Size == 8) ? 8 : (4 + Marshal.SystemDefaultCharSize));
		if (SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, ref deviceInterfaceDetailData, Marshal.SizeOf((object)deviceInterfaceDetailData) - 4, IntPtr.Zero, IntPtr.Zero))
		{
			return true;
		}
		deviceInterfaceDetailData = default(SP_DEVICE_INTERFACE_DETAIL_DATA);
		return false;
	}

	public static bool SetupDiGetDeviceInterfaceDevicePath(HDEVINFO deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, out string devicePath)
	{
		if (SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, out var deviceInterfaceDetailData))
		{
			devicePath = deviceInterfaceDetailData.DevicePath;
			return true;
		}
		devicePath = null;
		return false;
	}

	[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetupDiGetDeviceRegistryProperty(HDEVINFO deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, out uint propertyDataType, char[] buffer, int lengthInBytes, IntPtr lengthInBytesRequired);

	public static bool TryGetDeviceRegistryProperty(HDEVINFO deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, out string value)
	{
		value = null;
		char[] buffer = new char[64];
		int lengthInBytes = 126;
		if (SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref deviceInfoData, property, out var propertyDataType, buffer, lengthInBytes, IntPtr.Zero) && propertyDataType == 1)
		{
			value = NTString(buffer);
		}
		return value != null;
	}

	public static bool TryGetSerialPortFriendlyName(HDEVINFO deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, out string friendlyName)
	{
		if (!TryGetDeviceRegistryProperty(deviceInfoSet, ref deviceInfoData, 12u, out friendlyName) && !TryGetDeviceRegistryProperty(deviceInfoSet, ref deviceInfoData, 0u, out friendlyName))
		{
			friendlyName = null;
		}
		return !string.IsNullOrEmpty(friendlyName);
	}

	public static bool TryGetSerialPortName(HDEVINFO deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, out string portName)
	{
		portName = null;
		IntPtr intPtr = SetupDiOpenDevRegKey(deviceInfoSet, ref deviceInfoData);
		if (intPtr != (IntPtr)(-1))
		{
			try
			{
				char[] array = new char[64];
				int lengthInBytes = 126;
				if (RegQueryValueEx(intPtr, "PortName", 0u, IntPtr.Zero, array, ref lengthInBytes) == 0)
				{
					Array.Resize(ref array, lengthInBytes / 2);
					string text = NTString(array);
					if (text.Length >= 4 && text.StartsWith("COM") && int.TryParse(text.Substring(3), NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) && text == "COM" + result.ToString(CultureInfo.InvariantCulture))
					{
						portName = text;
					}
				}
			}
			finally
			{
				RegCloseKey(intPtr);
			}
		}
		return portName != null;
	}

	public static void EnumerateDeviceInterfaces(Guid guid, EnumerateDeviceInterfacesCallback callback)
	{
		EnumerateDeviceInterfaces(guid, null, callback);
	}

	public static void EnumerateDeviceInterfaces(Guid guid, string deviceIDToFilterTo, EnumerateDeviceInterfacesCallback callback)
	{
		EnumerateDevicesCore(SetupDiGetClassDevs(guid, deviceIDToFilterTo, IntPtr.Zero, DIGCF.Present | DIGCF.DeviceInterface), delegate(HDEVINFO devInfo, SP_DEVINFO_DATA dvi, string deviceID)
		{
			SP_DEVICE_INTERFACE_DATA deviceInterfaceData = default(SP_DEVICE_INTERFACE_DATA);
			deviceInterfaceData.Size = Marshal.SizeOf((object)deviceInterfaceData);
			for (int i = 0; SetupDiEnumDeviceInterfaces(devInfo, ref dvi, guid, i, ref deviceInterfaceData); i++)
			{
				if (SetupDiGetDeviceInterfaceDevicePath(devInfo, ref deviceInterfaceData, out var devicePath))
				{
					callback(devInfo, dvi, deviceInterfaceData, deviceID, devicePath);
				}
			}
		});
	}

	public static void EnumerateDevices(Guid guid, EnumerateDevicesCallback callback)
	{
		EnumerateDevicesCore(SetupDiGetClassDevs(guid, null, IntPtr.Zero, DIGCF.Present), callback);
	}

	private static void EnumerateDevicesCore(HDEVINFO devInfo, EnumerateDevicesCallback callback)
	{
		if (!devInfo.IsValid)
		{
			return;
		}
		try
		{
			SP_DEVINFO_DATA deviceInfoData = default(SP_DEVINFO_DATA);
			deviceInfoData.Size = Marshal.SizeOf((object)deviceInfoData);
			for (int i = 0; SetupDiEnumDeviceInfo(devInfo, i, ref deviceInfoData); i++)
			{
				if (CM_Get_Device_ID(deviceInfoData.DevInst, out var deviceID) == 0)
				{
					callback(devInfo, deviceInfoData, deviceID);
				}
			}
		}
		finally
		{
			SetupDiDestroyDeviceInfoList(devInfo);
		}
	}

	[DllImport("setupapi.dll", SetLastError = true)]
	public static extern IntPtr SetupDiOpenDevRegKey(HDEVINFO deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, int scope = 1, int profile = 0, int keyType = 1, uint desiredAccess = 131097u);

	[DllImport("setupapi.dll", SetLastError = true)]
	public static extern IntPtr SetupDiOpenDeviceInterfaceRegKey(HDEVINFO deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, uint reserved = 0u, uint desiredAccess = 131097u);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern IntPtr CreateEvent(IntPtr eventAttributes, [MarshalAs(UnmanagedType.Bool)] bool manualReset, [MarshalAs(UnmanagedType.Bool)] bool initialState, IntPtr name);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern IntPtr CreateFile(string filename, EFileAccess desiredAccess, EFileShare shareMode, IntPtr securityAttributes, ECreationDisposition creationDisposition, EFileAttributes attributes, IntPtr template);

	public static IntPtr CreateFileFromDevice(string filename, EFileAccess desiredAccess, EFileShare shareMode)
	{
		return CreateFile(filename, desiredAccess, shareMode, IntPtr.Zero, ECreationDisposition.OpenExisting, EFileAttributes.Device | EFileAttributes.Overlapped, IntPtr.Zero);
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CloseHandle(IntPtr handle);

	public static bool CloseHandle(ref IntPtr handle)
	{
		if (!CloseHandle(handle))
		{
			return false;
		}
		handle = IntPtr.Zero;
		return true;
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public unsafe static extern bool ReadFile(IntPtr handle, byte* buffer, int bytesToRead, IntPtr bytesRead, NativeOverlapped* overlapped);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public unsafe static extern bool WriteFile(IntPtr handle, byte* buffer, int bytesToWrite, IntPtr bytesWritten, NativeOverlapped* overlapped);

	public static string NTString(char[] buffer)
	{
		int num = Array.IndexOf(buffer, '\0');
		return new string(buffer, 0, (num >= 0) ? num : buffer.Length);
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CancelIo(IntPtr handle);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public unsafe static extern bool DeviceIoControl(IntPtr handle, uint ioControlCode, void* inBuffer, uint inBufferSize, void* outBuffer, uint outBufferSize, out uint bytesReturned, NativeOverlapped* overlapped);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetCommState(IntPtr handle, ref DCB dcb);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetCommState(IntPtr handle, ref DCB dcb);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetCommTimeouts(IntPtr handle, ref COMMTIMEOUTS timeouts);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetCommTimeouts(IntPtr handle, out COMMTIMEOUTS timeouts);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool PurgeComm(IntPtr handle, uint flags);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool FlushFileBuffers(IntPtr handle);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public unsafe static extern bool GetOverlappedResult(IntPtr handle, NativeOverlapped* overlapped, out uint bytesTransferred, [MarshalAs(UnmanagedType.Bool)] bool wait);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ResetEvent(IntPtr handle);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetEvent(IntPtr handle);

	[DllImport("kernel32.dll", SetLastError = true)]
	public unsafe static extern uint WaitForMultipleObjects(uint count, IntPtr* handles, [MarshalAs(UnmanagedType.Bool)] bool waitAll, uint milliseconds);

	public static uint WaitForMultipleObjectsGetTimeout(int eventTimeout)
	{
		if (eventTimeout >= 0)
		{
			return (uint)eventTimeout;
		}
		return uint.MaxValue;
	}

	[DllImport("BluetoothAPIs.dll", SetLastError = true)]
	public static extern int BluetoothGATTGetServices(IntPtr handle, ushort allocatedCount, [Out] BTH_LE_GATT_SERVICE[] services, out ushort returnedCount, uint flags = 0u);

	public static BTH_LE_GATT_SERVICE[] BluetoothGATTGetServices(IntPtr handle)
	{
		ushort returnedCount;
		int num = BluetoothGATTGetServices(handle, 0, null, out returnedCount);
		if (num != -2147024662)
		{
			return null;
		}
		BTH_LE_GATT_SERVICE[] array = new BTH_LE_GATT_SERVICE[returnedCount];
		if (returnedCount > 0)
		{
			if (BluetoothGATTGetServices(handle, returnedCount, array, out var returnedCount2) != 0)
			{
				return null;
			}
			if (returnedCount != returnedCount2)
			{
				return null;
			}
		}
		return array;
	}

	[DllImport("BluetoothAPIs.dll", SetLastError = true)]
	public static extern int BluetoothGATTGetCharacteristics(IntPtr handle, [In] ref BTH_LE_GATT_SERVICE service, ushort allocatedCount, [Out] BTH_LE_GATT_CHARACTERISTIC[] characteristics, out ushort returnedCount, uint flags = 0u);

	public static BTH_LE_GATT_CHARACTERISTIC[] BluetoothGATTGetCharacteristics(IntPtr handle, ref BTH_LE_GATT_SERVICE service)
	{
		ushort returnedCount;
		switch (BluetoothGATTGetCharacteristics(handle, ref service, 0, null, out returnedCount))
		{
		case -2147023728:
			returnedCount = 0;
			break;
		default:
			return null;
		case -2147024662:
			break;
		}
		BTH_LE_GATT_CHARACTERISTIC[] array = new BTH_LE_GATT_CHARACTERISTIC[returnedCount];
		if (returnedCount > 0)
		{
			if (BluetoothGATTGetCharacteristics(handle, ref service, returnedCount, array, out var returnedCount2) != 0)
			{
				return null;
			}
			if (returnedCount != returnedCount2)
			{
				return null;
			}
		}
		return array;
	}

	[DllImport("BluetoothAPIs.dll", SetLastError = true)]
	public static extern int BluetoothGATTGetDescriptors(IntPtr handle, [In] ref BTH_LE_GATT_CHARACTERISTIC characteristic, ushort allocatedCount, [Out] BTH_LE_GATT_DESCRIPTOR[] descriptors, out ushort returnedCount, uint flags = 0u);

	public static BTH_LE_GATT_DESCRIPTOR[] BluetoothGATTGetDescriptors(IntPtr handle, ref BTH_LE_GATT_CHARACTERISTIC characteristic)
	{
		ushort returnedCount;
		switch (BluetoothGATTGetDescriptors(handle, ref characteristic, 0, null, out returnedCount))
		{
		case -2147023728:
			returnedCount = 0;
			break;
		default:
			return null;
		case -2147024662:
			break;
		}
		BTH_LE_GATT_DESCRIPTOR[] array = new BTH_LE_GATT_DESCRIPTOR[returnedCount];
		if (returnedCount > 0)
		{
			if (BluetoothGATTGetDescriptors(handle, ref characteristic, returnedCount, array, out var returnedCount2) != 0)
			{
				return null;
			}
			if (returnedCount != returnedCount2)
			{
				return null;
			}
		}
		return array;
	}

	[DllImport("BluetoothAPIs.dll", SetLastError = true)]
	public unsafe static extern int BluetoothGATTGetCharacteristicValue(IntPtr handle, [In] ref BTH_LE_GATT_CHARACTERISTIC characteristic, uint valueDataSize, BTH_LE_GATT_CHARACTERISTIC_VALUE* value, out ushort valueSizeRequired, BLUETOOTH_GATT_FLAGS flags);

	[DllImport("BluetoothAPIs.dll", SetLastError = true)]
	public unsafe static extern int BluetoothGATTSetCharacteristicValue(IntPtr handle, [In] ref BTH_LE_GATT_CHARACTERISTIC characteristic, BTH_LE_GATT_CHARACTERISTIC_VALUE* value, ulong reliableWriteContext, BLUETOOTH_GATT_FLAGS flags);

	[DllImport("BluetoothAPIs.dll", SetLastError = true)]
	public unsafe static extern int BluetoothGATTGetDescriptorValue(IntPtr handle, [In] ref BTH_LE_GATT_DESCRIPTOR descriptor, uint valueDataSize, BTH_LE_GATT_DESCRIPTOR_VALUE* value, out ushort valueSizeRequired, BLUETOOTH_GATT_FLAGS flags);

	[DllImport("BluetoothAPIs.dll", SetLastError = true)]
	public unsafe static extern int BluetoothGATTSetDescriptorValue(IntPtr handle, [In] ref BTH_LE_GATT_DESCRIPTOR descriptor, BTH_LE_GATT_DESCRIPTOR_VALUE* value, BLUETOOTH_GATT_FLAGS flags);

	[DllImport("BluetoothAPIs.dll", SetLastError = true)]
	public unsafe static extern int BluetoothGATTRegisterEvent(IntPtr handle, BTH_LE_GATT_EVENT_TYPE eventType, BLUETOOTH_GATT_VALUE_CHANGED_EVENT_REGISTRATION* eventParameter, BLUETOOTH_GATT_EVENT_CALLBACK callback, IntPtr context, out IntPtr eventHandle, int flags = 0);

	[DllImport("BluetoothAPIs.dll", SetLastError = true)]
	public static extern int BluetoothGATTUnregisterEvent(IntPtr eventHandle, int flags = 0);

	public static bool TryOpenToGetInfo(string path, Func<IntPtr, bool> action)
	{
		IntPtr intPtr = CreateFileFromDevice(path, EFileAccess.None, EFileShare.Read | EFileShare.Write);
		if (intPtr == (IntPtr)(-1))
		{
			return false;
		}
		try
		{
			return action(intPtr);
		}
		catch (Exception arg)
		{
			HidSharpDiagnostics.Trace("CreateFileFromDevice failed: {0}", arg);
		}
		finally
		{
			CloseHandle(intPtr);
		}
		return false;
	}

	[DllImport("bthprops.cpl", SetLastError = true)]
	public static extern IntPtr BluetoothFindFirstRadio(ref BLUETOOTH_FIND_RADIO_PARAMS @params, out IntPtr radioHandle);

	[DllImport("bthprops.cpl", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BluetoothFindNextRadio(IntPtr searchHandle, out IntPtr radioHandle);

	[DllImport("bthprops.cpl", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BluetoothFindRadioClose(IntPtr searchHandle);

	[DllImport("bthprops.cpl", SetLastError = true)]
	public static extern IntPtr BluetoothFindFirstDevice(ref BLUETOOTH_DEVICE_SEARCH_PARAMS @params, ref BLUETOOTH_DEVICE_INFO info);

	[DllImport("bthprops.cpl", SetLastError = true)]
	public static extern bool BluetoothFindNextDevice(IntPtr searchHandle, ref BLUETOOTH_DEVICE_INFO info);

	[DllImport("bthprops.cpl", SetLastError = true)]
	public static extern bool BluetoothFindDeviceClose(IntPtr searchHandle);
}
