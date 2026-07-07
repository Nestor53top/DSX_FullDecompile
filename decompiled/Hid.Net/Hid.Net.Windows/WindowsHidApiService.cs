using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Device.Net;
using Device.Net.Exceptions;
using Device.Net.Windows;
using Microsoft.Win32.SafeHandles;

namespace Hid.Net.Windows;

public class WindowsHidApiService : ApiService, IHidApiService, IApiService
{
	private delegate bool GetString(SafeFileHandle hidDeviceObject, IntPtr pointerToBuffer, uint bufferLength);

	private static Guid? _HidGuid;

	private const int HIDP_STATUS_SUCCESS = 1114112;

	public WindowsHidApiService(ILogger logger)
		: base(logger)
	{
	}

	[DllImport("hid.dll", SetLastError = true)]
	private static extern bool HidD_GetPreparsedData(SafeFileHandle hidDeviceObject, out IntPtr pointerToPreparsedData);

	[DllImport("hid.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
	private static extern bool HidD_GetManufacturerString(SafeFileHandle hidDeviceObject, IntPtr pointerToBuffer, uint bufferLength);

	[DllImport("hid.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
	private static extern bool HidD_GetProductString(SafeFileHandle hidDeviceObject, IntPtr pointerToBuffer, uint bufferLength);

	[DllImport("hid.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
	private static extern bool HidD_GetSerialNumberString(SafeFileHandle hidDeviceObject, IntPtr pointerToBuffer, uint bufferLength);

	[DllImport("hid.dll", SetLastError = true)]
	private static extern int HidP_GetCaps(IntPtr pointerToPreparsedData, out HidCollectionCapabilities hidCollectionCapabilities);

	[DllImport("hid.dll", SetLastError = true)]
	private static extern bool HidD_GetAttributes(SafeFileHandle hidDeviceObject, out HidAttributes attributes);

	[DllImport("hid.dll", SetLastError = true)]
	private static extern void HidD_GetHidGuid(out Guid hidGuid);

	[DllImport("hid.dll", SetLastError = true)]
	private static extern bool HidD_FreePreparsedData(ref IntPtr pointerToPreparsedData);

	public ConnectedDeviceDefinition GetDeviceDefinition(string deviceId, SafeFileHandle safeFileHandle)
	{
		HidAttributes hidAttributes = GetHidAttributes(safeFileHandle);
		HidCollectionCapabilities hidCapabilities = GetHidCapabilities(safeFileHandle);
		string manufacturer = GetManufacturer(safeFileHandle);
		string serialNumber = GetSerialNumber(safeFileHandle);
		string product = GetProduct(safeFileHandle);
		return new ConnectedDeviceDefinition(deviceId)
		{
			WriteBufferSize = hidCapabilities.OutputReportByteLength,
			ReadBufferSize = hidCapabilities.InputReportByteLength,
			Manufacturer = manufacturer,
			ProductName = product,
			ProductId = (ushort)hidAttributes.ProductId,
			SerialNumber = serialNumber,
			Usage = hidCapabilities.Usage,
			UsagePage = hidCapabilities.UsagePage,
			VendorId = (ushort)hidAttributes.VendorId,
			VersionNumber = (ushort)hidAttributes.VersionNumber,
			DeviceType = DeviceType.Hid
		};
	}

	public string GetManufacturer(SafeFileHandle safeFileHandle)
	{
		return GetHidString(safeFileHandle, HidD_GetManufacturerString, base.Logger, "GetManufacturer");
	}

	public string GetProduct(SafeFileHandle safeFileHandle)
	{
		return GetHidString(safeFileHandle, HidD_GetProductString, base.Logger, "GetProduct");
	}

	public string GetSerialNumber(SafeFileHandle safeFileHandle)
	{
		return GetHidString(safeFileHandle, HidD_GetSerialNumberString, base.Logger, "GetSerialNumber");
	}

	public HidAttributes GetHidAttributes(SafeFileHandle safeFileHandle)
	{
		WindowsDeviceBase.HandleError(HidD_GetAttributes(safeFileHandle, out var attributes), "Could not get Hid Attributes (Call HidD_GetAttributes)");
		return attributes;
	}

	public HidCollectionCapabilities GetHidCapabilities(SafeFileHandle readSafeFileHandle)
	{
		WindowsDeviceBase.HandleError(HidD_GetPreparsedData(readSafeFileHandle, out var pointerToPreparsedData), "Could not get pre parsed data");
		HidCollectionCapabilities hidCollectionCapabilities;
		int num = HidP_GetCaps(pointerToPreparsedData, out hidCollectionCapabilities);
		if (num != 1114112)
		{
			throw new ApiException($"Could not get Hid capabilities. Return code: {num}");
		}
		WindowsDeviceBase.HandleError(HidD_FreePreparsedData(ref pointerToPreparsedData), "Could not release handle for getting Hid capabilities");
		return hidCollectionCapabilities;
	}

	public Guid GetHidGuid()
	{
		if (_HidGuid.HasValue)
		{
			return _HidGuid.Value;
		}
		HidD_GetHidGuid(out var hidGuid);
		_HidGuid = hidGuid;
		return hidGuid;
	}

	public Stream OpenRead(SafeFileHandle readSafeFileHandle, ushort readBufferSize)
	{
		return new FileStream(readSafeFileHandle, FileAccess.Read, readBufferSize, isAsync: false);
	}

	public Stream OpenWrite(SafeFileHandle writeSafeFileHandle, ushort writeBufferSize)
	{
		return new FileStream(writeSafeFileHandle, FileAccess.ReadWrite, writeBufferSize, isAsync: false);
	}

	private static string GetHidString(SafeFileHandle safeFileHandle, GetString getString, ILogger logger, [CallerMemberName] string callMemberName = null)
	{
		try
		{
			IntPtr intPtr = Marshal.AllocHGlobal(126);
			bool num = getString(safeFileHandle, intPtr, 126u);
			Marshal.FreeHGlobal(intPtr);
			if (!num)
			{
				logger?.Log("Could not get Hid string. Caller: " + callMemberName, "WindowsHidApiService", null, LogLevel.Warning);
			}
			return Marshal.PtrToStringUni(intPtr);
		}
		catch (Exception ex)
		{
			logger?.Log("Could not get Hid string. Message: " + ex.Message, "WindowsHidApiService", ex, LogLevel.Error);
			return null;
		}
	}
}
