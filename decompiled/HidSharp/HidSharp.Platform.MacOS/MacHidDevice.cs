using System;

namespace HidSharp.Platform.MacOS;

internal sealed class MacHidDevice : HidDevice
{
	private string _manufacturer;

	private string _productName;

	private string _serialNumber;

	private int _vid;

	private int _pid;

	private int _version;

	private int _maxInput;

	private int _maxOutput;

	private int _maxFeature;

	private bool _reportsUseID;

	private byte[] _reportDescriptor;

	private NativeMethods.io_string_t _path;

	public override string DevicePath => _path.ToString();

	public override int VendorID => _vid;

	public override int ProductID => _pid;

	public override int ReleaseNumberBcd => _version;

	internal bool ReportsUseID => _reportsUseID;

	private MacHidDevice()
	{
	}

	internal static MacHidDevice TryCreate(NativeMethods.io_string_t path)
	{
		MacHidDevice macHidDevice = new MacHidDevice();
		macHidDevice._path = path;
		MacHidDevice macHidDevice2 = macHidDevice;
		NativeMethods.IOObject iOObject = NativeMethods.IORegistryEntryFromPath(0, ref path).ToIOObject();
		if (!iOObject.IsSet)
		{
			return null;
		}
		using (iOObject)
		{
			int? num = NativeMethods.IORegistryEntryGetCFProperty_Int(iOObject, NativeMethods.kIOHIDVendorIDKey);
			int? num2 = NativeMethods.IORegistryEntryGetCFProperty_Int(iOObject, NativeMethods.kIOHIDProductIDKey);
			int? num3 = NativeMethods.IORegistryEntryGetCFProperty_Int(iOObject, NativeMethods.kIOHIDVersionNumberKey);
			if (!num.HasValue || !num2.HasValue || !num3.HasValue)
			{
				return null;
			}
			macHidDevice2._vid = num.Value;
			macHidDevice2._pid = num2.Value;
			macHidDevice2._version = num3.Value;
			macHidDevice2._maxInput = NativeMethods.IORegistryEntryGetCFProperty_Int(iOObject, NativeMethods.kIOHIDMaxInputReportSizeKey) ?? 0;
			macHidDevice2._maxOutput = NativeMethods.IORegistryEntryGetCFProperty_Int(iOObject, NativeMethods.kIOHIDMaxOutputReportSizeKey) ?? 0;
			macHidDevice2._maxFeature = NativeMethods.IORegistryEntryGetCFProperty_Int(iOObject, NativeMethods.kIOHIDMaxFeatureReportSizeKey) ?? 0;
			macHidDevice2._manufacturer = NativeMethods.IORegistryEntryGetCFProperty_String(iOObject, NativeMethods.kIOHIDManufacturerKey);
			macHidDevice2._productName = NativeMethods.IORegistryEntryGetCFProperty_String(iOObject, NativeMethods.kIOHIDProductKey);
			macHidDevice2._serialNumber = NativeMethods.IORegistryEntryGetCFProperty_String(iOObject, NativeMethods.kIOHIDSerialNumberKey);
			macHidDevice2._reportDescriptor = NativeMethods.IORegistryEntryGetCFProperty_Data(iOObject, NativeMethods.kIOHIDReportDescriptorKey);
			if (macHidDevice2._maxInput == 0 && macHidDevice2._maxOutput == 0 && macHidDevice2._maxFeature == 0)
			{
				return null;
			}
			macHidDevice2._reportsUseID = false;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			using (NativeMethods.CFType cFType = NativeMethods.IOHIDDeviceCreate(IntPtr.Zero, iOObject).ToCFType())
			{
				if (!cFType.IsSet)
				{
					return null;
				}
				using NativeMethods.CFType cFType2 = NativeMethods.IOHIDDeviceCopyMatchingElements(cFType, IntPtr.Zero).ToCFType();
				if (!cFType2.IsSet)
				{
					return null;
				}
				int num4 = (int)NativeMethods.CFArrayGetCount(cFType2);
				for (int i = 0; i < num4; i++)
				{
					IntPtr intPtr = NativeMethods.CFArrayGetValueAtIndex(cFType2, (IntPtr)i);
					if (!(intPtr == IntPtr.Zero))
					{
						switch (NativeMethods.IOHIDElementGetType(intPtr))
						{
						case NativeMethods.IOHIDElementType.InputMisc:
						case NativeMethods.IOHIDElementType.InputButton:
						case NativeMethods.IOHIDElementType.InputAxis:
						case NativeMethods.IOHIDElementType.InputScanCodes:
							flag = true;
							break;
						case NativeMethods.IOHIDElementType.Output:
							flag2 = true;
							break;
						case NativeMethods.IOHIDElementType.Feature:
							flag3 = true;
							break;
						}
						if (NativeMethods.IOHIDElementGetReportID(intPtr) != 0)
						{
							macHidDevice2._reportsUseID = true;
						}
					}
				}
			}
			if (!macHidDevice2._reportsUseID)
			{
				if (macHidDevice2._maxInput != 0)
				{
					macHidDevice2._maxInput++;
				}
				if (macHidDevice2._maxOutput != 0)
				{
					macHidDevice2._maxOutput++;
				}
				if (macHidDevice2._maxFeature != 0)
				{
					macHidDevice2._maxFeature++;
				}
			}
			if (!flag)
			{
				macHidDevice2._maxInput = 0;
			}
			if (!flag2)
			{
				macHidDevice2._maxOutput = 0;
			}
			if (!flag3)
			{
				macHidDevice2._maxFeature = 0;
			}
		}
		return macHidDevice2;
	}

	protected override DeviceStream OpenDeviceDirectly(OpenConfiguration openConfig)
	{
		MacHidStream macHidStream = new MacHidStream(this);
		try
		{
			macHidStream.Init(_path);
			return macHidStream;
		}
		catch
		{
			macHidStream.Close();
			throw;
		}
	}

	public override int GetMaxInputReportLength()
	{
		return _maxInput;
	}

	public override int GetMaxOutputReportLength()
	{
		return _maxOutput;
	}

	public override int GetMaxFeatureReportLength()
	{
		return _maxFeature;
	}

	public override string GetManufacturer()
	{
		if (_manufacturer == null)
		{
			throw DeviceException.CreateIOException(this, "Unnamed manufacturer.");
		}
		return _manufacturer;
	}

	public override string GetProductName()
	{
		if (_productName == null)
		{
			throw DeviceException.CreateIOException(this, "Unnamed product.");
		}
		return _productName;
	}

	public override string GetSerialNumber()
	{
		if (_serialNumber == null)
		{
			throw DeviceException.CreateIOException(this, "No serial number.");
		}
		return _serialNumber;
	}

	public override string GetFileSystemName()
	{
		throw new NotSupportedException();
	}

	public override byte[] GetRawReportDescriptor()
	{
		byte[] reportDescriptor = _reportDescriptor;
		if (reportDescriptor == null)
		{
			throw new NotSupportedException("Report descriptors are only available on OS X 10.8+.");
		}
		return (byte[])reportDescriptor.Clone();
	}

	public override bool HasImplementationDetail(Guid detail)
	{
		if (!base.HasImplementationDetail(detail))
		{
			return detail == ImplementationDetail.MacOS;
		}
		return true;
	}
}
