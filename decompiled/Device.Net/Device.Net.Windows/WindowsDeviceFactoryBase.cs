using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Device.Net.Windows;

public abstract class WindowsDeviceFactoryBase
{
	public ILogger Logger { get; }

	public ITracer Tracer { get; }

	public abstract DeviceType DeviceType { get; }

	protected abstract ConnectedDeviceDefinition GetDeviceDefinition(string deviceId);

	protected abstract Guid GetClassGuid();

	protected WindowsDeviceFactoryBase(ILogger logger, ITracer tracer)
	{
		Logger = logger;
		Tracer = tracer;
	}

	public async Task<IEnumerable<ConnectedDeviceDefinition>> GetConnectedDeviceDefinitionsAsync(FilterDeviceDefinition filterDeviceDefinition)
	{
		return await Task.Run((Func<IEnumerable<ConnectedDeviceDefinition>>)delegate
		{
			Collection<ConnectedDeviceDefinition> collection = new Collection<ConnectedDeviceDefinition>();
			SpDeviceInterfaceData deviceInterfaceData = default(SpDeviceInterfaceData);
			SpDeviceInfoData deviceInfoData = default(SpDeviceInfoData);
			SpDeviceInterfaceDetailData deviceInterfaceDetailData = default(SpDeviceInterfaceDetailData);
			deviceInterfaceData.CbSize = (uint)Marshal.SizeOf((object)deviceInterfaceData);
			deviceInfoData.CbSize = (uint)Marshal.SizeOf((object)deviceInfoData);
			string word = null;
			string word2 = null;
			string text = GetClassGuid().ToString();
			Guid classGuid = new Guid(text);
			Log(string.Format("About to call {0} for class Guid {1}. Flags: {2}", "SetupDiGetClassDevs", text, 18), null, LogLevel.Information, "GetConnectedDeviceDefinitionsAsync");
			IntPtr intPtr = APICalls.SetupDiGetClassDevs(ref classGuid, IntPtr.Zero, IntPtr.Zero, 18u);
			deviceInterfaceDetailData.CbSize = ((IntPtr.Size == 8) ? 8 : (4 + Marshal.SystemDefaultCharSize));
			int num = -1;
			if (filterDeviceDefinition != null)
			{
				if (filterDeviceDefinition.ProductId.HasValue)
				{
					word = Helpers.GetHex(filterDeviceDefinition.ProductId);
				}
				if (filterDeviceDefinition.VendorId.HasValue)
				{
					word2 = Helpers.GetHex(filterDeviceDefinition.VendorId);
				}
			}
			while (true)
			{
				try
				{
					num++;
					if (!APICalls.SetupDiEnumDeviceInterfaces(intPtr, IntPtr.Zero, ref classGuid, (uint)num, ref deviceInterfaceData))
					{
						int lastWin32Error = Marshal.GetLastWin32Error();
						if (lastWin32Error == 259)
						{
							Log("The call to SetupDiEnumDeviceInterfaces returned ERROR_NO_MORE_ITEMS", null, LogLevel.Information, "GetConnectedDeviceDefinitionsAsync");
							break;
						}
						if (lastWin32Error > 0)
						{
							Log(string.Format("{0} called successfully but a device was skipped while enumerating because something went wrong. The device was at index {1}. The error code was {2}.", "SetupDiEnumDeviceInterfaces", num, lastWin32Error), null, LogLevel.Warning, "GetConnectedDeviceDefinitionsAsync");
						}
					}
					if (!APICalls.SetupDiGetDeviceInterfaceDetail(intPtr, ref deviceInterfaceData, ref deviceInterfaceDetailData, 256u, out var _, ref deviceInfoData))
					{
						int lastWin32Error2 = Marshal.GetLastWin32Error();
						if (lastWin32Error2 == 259)
						{
							Log("The call to SetupDiEnumDeviceInterfaces returned ERROR_NO_MORE_ITEMS", null, LogLevel.Information, "GetConnectedDeviceDefinitionsAsync");
							break;
						}
						if (lastWin32Error2 > 0)
						{
							Log(string.Format("{0} called successfully but a device was skipped while enumerating because something went wrong. The device was at index {1}. The error code was {2}.", "SetupDiGetDeviceInterfaceDetail", num, lastWin32Error2), null, LogLevel.Warning, "GetConnectedDeviceDefinitionsAsync");
						}
					}
					if (filterDeviceDefinition == null || ((!filterDeviceDefinition.VendorId.HasValue || deviceInterfaceDetailData.DevicePath.ContainsIgnoreCase(word2)) && (!filterDeviceDefinition.ProductId.HasValue || deviceInterfaceDetailData.DevicePath.ContainsIgnoreCase(word))))
					{
						ConnectedDeviceDefinition deviceDefinition = GetDeviceDefinition(deviceInterfaceDetailData.DevicePath);
						if (deviceDefinition == null)
						{
							Logger.Log("Device with path " + deviceInterfaceDetailData.DevicePath + " was skipped. See previous logs.", GetType().Name, null, LogLevel.Warning);
						}
						else if (DeviceManager.IsDefinitionMatch(filterDeviceDefinition, deviceDefinition))
						{
							collection.Add(deviceDefinition);
						}
					}
				}
				catch (Exception ex)
				{
					Log(ex, "GetConnectedDeviceDefinitionsAsync");
				}
			}
			APICalls.SetupDiDestroyDeviceInfoList(intPtr);
			return collection;
		});
	}

	protected void Log(Exception ex, [CallerMemberName] string callMemberName = null)
	{
		Log(null, GetType().Name + " - " + callMemberName, ex, LogLevel.Error);
	}

	protected void Log(string message, Exception ex, LogLevel logLevel, [CallerMemberName] string callMemberName = null)
	{
		Log(message, GetType().Name + " - " + callMemberName, ex, logLevel);
	}

	protected void Log(string message, string region, Exception ex, LogLevel logLevel)
	{
		Logger?.Log(message, region, ex, logLevel);
	}

	private static uint GetNumberFromDeviceId(string deviceId, string searchString)
	{
		if (deviceId == null)
		{
			throw new ArgumentNullException("deviceId");
		}
		int num = deviceId.IndexOf(searchString, StringComparison.OrdinalIgnoreCase);
		string s = null;
		if (num > -1)
		{
			s = deviceId.Substring(num + searchString.Length, 4);
		}
		return uint.Parse(s, NumberStyles.HexNumber);
	}

	public static ConnectedDeviceDefinition GetDeviceDefinitionFromWindowsDeviceId(string deviceId, DeviceType deviceType, ILogger logger)
	{
		uint? vendorId = null;
		uint? productId = null;
		try
		{
			vendorId = GetNumberFromDeviceId(deviceId, "vid_");
			productId = GetNumberFromDeviceId(deviceId, "pid_");
		}
		catch (Exception ex)
		{
			logger?.Log("Error " + ex.Message, "GetDeviceDefinitionFromWindowsDeviceId", ex, LogLevel.Error);
		}
		return new ConnectedDeviceDefinition(deviceId)
		{
			DeviceType = deviceType,
			VendorId = vendorId,
			ProductId = productId
		};
	}
}
