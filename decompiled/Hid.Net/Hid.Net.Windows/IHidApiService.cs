using System;
using System.IO;
using Device.Net;
using Device.Net.Windows;
using Microsoft.Win32.SafeHandles;

namespace Hid.Net.Windows;

public interface IHidApiService : IApiService
{
	ConnectedDeviceDefinition GetDeviceDefinition(string deviceId, SafeFileHandle safeFileHandle);

	HidAttributes GetHidAttributes(SafeFileHandle safeFileHandle);

	HidCollectionCapabilities GetHidCapabilities(SafeFileHandle readSafeFileHandle);

	Guid GetHidGuid();

	string GetManufacturer(SafeFileHandle safeFileHandle);

	string GetProduct(SafeFileHandle safeFileHandle);

	string GetSerialNumber(SafeFileHandle safeFileHandle);

	Stream OpenRead(SafeFileHandle readSafeFileHandle, ushort readBufferSize);

	Stream OpenWrite(SafeFileHandle writeSafeFileHandle, ushort writeBufferSize);
}
