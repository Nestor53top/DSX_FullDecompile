using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Device.Net.Exceptions;

namespace Device.Net.Windows;

public abstract class WindowsDeviceBase : DeviceBase
{
	protected virtual string LogSection => "WindowsDeviceBase";

	protected WindowsDeviceBase(string deviceId, ILogger logger, ITracer tracer)
		: base(deviceId, logger, tracer)
	{
	}

	public abstract Task InitializeAsync();

	public static void HandleError(bool isSuccess, string message)
	{
		if (!isSuccess)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 0)
			{
				throw new ApiException($"{message}. Error code: {lastWin32Error}");
			}
		}
	}
}
