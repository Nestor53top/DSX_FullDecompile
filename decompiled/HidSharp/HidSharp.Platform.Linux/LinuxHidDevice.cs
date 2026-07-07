using System;
using System.IO;
using HidSharp.Reports;

namespace HidSharp.Platform.Linux;

internal sealed class LinuxHidDevice : HidDevice
{
	private object _getInfoLock;

	private string _manufacturer;

	private string _productName;

	private string _serialNumber;

	private byte[] _reportDescriptor;

	private int _vid;

	private int _pid;

	private int _version;

	private int _maxInput;

	private int _maxOutput;

	private int _maxFeature;

	private bool _reportsUseID;

	private string _path;

	private string _fileSystemName;

	public override string DevicePath => _path;

	public override int VendorID => _vid;

	public override int ProductID => _pid;

	public override int ReleaseNumberBcd => _version;

	internal bool ReportsUseID => _reportsUseID;

	private LinuxHidDevice()
	{
		_getInfoLock = new object();
	}

	internal static LinuxHidDevice TryCreate(string path)
	{
		LinuxHidDevice linuxHidDevice = new LinuxHidDevice();
		linuxHidDevice._path = path;
		LinuxHidDevice linuxHidDevice2 = linuxHidDevice;
		IntPtr intPtr = NativeMethodsLibudev.Instance.udev_new();
		if (IntPtr.Zero != intPtr)
		{
			try
			{
				IntPtr intPtr2 = NativeMethodsLibudev.Instance.udev_device_new_from_syspath(intPtr, linuxHidDevice2._path);
				if (intPtr2 != IntPtr.Zero)
				{
					try
					{
						string text = NativeMethodsLibudev.Instance.udev_device_get_devnode(intPtr2);
						if (text != null)
						{
							linuxHidDevice2._fileSystemName = text;
							IntPtr intPtr3 = NativeMethodsLibudev.Instance.udev_device_get_parent_with_subsystem_devtype(intPtr2, "usb", "usb_device");
							if (IntPtr.Zero != intPtr3)
							{
								string manufacturer = NativeMethodsLibudev.Instance.udev_device_get_sysattr_value(intPtr3, "manufacturer");
								string productName = NativeMethodsLibudev.Instance.udev_device_get_sysattr_value(intPtr3, "product");
								string serialNumber = NativeMethodsLibudev.Instance.udev_device_get_sysattr_value(intPtr3, "serial");
								string hex = NativeMethodsLibudev.Instance.udev_device_get_sysattr_value(intPtr3, "idVendor");
								string hex2 = NativeMethodsLibudev.Instance.udev_device_get_sysattr_value(intPtr3, "idProduct");
								string hex3 = NativeMethodsLibudev.Instance.udev_device_get_sysattr_value(intPtr3, "bcdDevice");
								if (NativeMethods.TryParseHex(hex, out var result) && NativeMethods.TryParseHex(hex2, out var result2) && NativeMethods.TryParseHex(hex3, out var result3))
								{
									linuxHidDevice2._vid = result;
									linuxHidDevice2._pid = result2;
									linuxHidDevice2._version = result3;
									linuxHidDevice2._manufacturer = manufacturer;
									linuxHidDevice2._productName = productName;
									linuxHidDevice2._serialNumber = serialNumber;
									return linuxHidDevice2;
								}
							}
						}
					}
					finally
					{
						NativeMethodsLibudev.Instance.udev_device_unref(intPtr2);
					}
				}
			}
			finally
			{
				NativeMethodsLibudev.Instance.udev_unref(intPtr);
			}
		}
		return null;
	}

	protected override DeviceStream OpenDeviceDirectly(OpenConfiguration openConfig)
	{
		RequiresGetInfo();
		LinuxHidStream linuxHidStream = new LinuxHidStream(this);
		try
		{
			linuxHidStream.Init(_path);
			return linuxHidStream;
		}
		catch
		{
			linuxHidStream.Close();
			throw;
		}
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

	public override int GetMaxInputReportLength()
	{
		RequiresGetInfo();
		return _maxInput;
	}

	public override int GetMaxOutputReportLength()
	{
		RequiresGetInfo();
		return _maxOutput;
	}

	public override int GetMaxFeatureReportLength()
	{
		RequiresGetInfo();
		return _maxFeature;
	}

	public override byte[] GetRawReportDescriptor()
	{
		RequiresGetInfo();
		return (byte[])_reportDescriptor.Clone();
	}

	private bool TryParseReportDescriptor(out ReportDescriptor parser, out byte[] reportDescriptor)
	{
		parser = null;
		reportDescriptor = null;
		int handle;
		try
		{
			handle = LinuxHidStream.DeviceHandleFromPath(_path, this, NativeMethods.oflag.NONBLOCK);
		}
		catch (FileNotFoundException)
		{
			throw DeviceException.CreateIOException(this, "Failed to read report descriptor.");
		}
		try
		{
			if (NativeMethods.ioctl(handle, NativeMethods.HIDIOCGRDESCSIZE, out var value) < 0)
			{
				return false;
			}
			if (value > 4096)
			{
				return false;
			}
			NativeMethods.hidraw_report_descriptor value2 = new NativeMethods.hidraw_report_descriptor
			{
				size = value
			};
			if (NativeMethods.ioctl(handle, NativeMethods.HIDIOCGRDESC, ref value2) < 0)
			{
				return false;
			}
			Array.Resize(ref value2.value, (int)value);
			parser = new ReportDescriptor(value2.value);
			reportDescriptor = value2.value;
			return true;
		}
		finally
		{
			NativeMethods.retry(() => NativeMethods.close(handle));
		}
	}

	private void RequiresGetInfo()
	{
		lock (_getInfoLock)
		{
			if (_reportDescriptor == null)
			{
				if (!TryParseReportDescriptor(out var parser, out var reportDescriptor))
				{
					throw DeviceException.CreateIOException(this, "Failed to read report descriptor.");
				}
				_maxInput = parser.MaxInputReportLength;
				_maxOutput = parser.MaxOutputReportLength;
				_maxFeature = parser.MaxFeatureReportLength;
				_reportsUseID = parser.ReportsUseID;
				_reportDescriptor = reportDescriptor;
			}
		}
	}

	public override string GetFileSystemName()
	{
		return _fileSystemName;
	}

	public override bool HasImplementationDetail(Guid detail)
	{
		if (!base.HasImplementationDetail(detail) && !(detail == ImplementationDetail.Linux))
		{
			return detail == ImplementationDetail.HidrawApi;
		}
		return true;
	}
}
