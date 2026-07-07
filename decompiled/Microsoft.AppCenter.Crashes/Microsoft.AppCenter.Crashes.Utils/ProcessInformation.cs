using System;
using System.Diagnostics;

namespace Microsoft.AppCenter.Crashes.Utils;

public class ProcessInformation : IProcessInformation
{
	public DateTime? ProcessStartTime
	{
		get
		{
			try
			{
				return Process.GetCurrentProcess().StartTime;
			}
			catch (Exception exception)
			{
				AppCenterLog.Warn("AppCenterCrashes", "Unable to get process start time.", exception);
				return null;
			}
		}
	}

	public int? ProcessId
	{
		get
		{
			try
			{
				return Process.GetCurrentProcess().Id;
			}
			catch (Exception exception)
			{
				AppCenterLog.Warn("AppCenterCrashes", "Unable to get process ID.", exception);
				return null;
			}
		}
	}

	public string ProcessName
	{
		get
		{
			try
			{
				return Process.GetCurrentProcess().ProcessName;
			}
			catch (Exception exception)
			{
				AppCenterLog.Warn("AppCenterCrashes", "Unable to get process name.", exception);
				return null;
			}
		}
	}

	public int? ParentProcessId => null;

	public string ParentProcessName => null;

	public string ProcessArchitecture
	{
		get
		{
			try
			{
				return Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
			}
			catch (Exception exception)
			{
				AppCenterLog.Warn("AppCenterCrashes", "Unable to get process architecture.", exception);
				return null;
			}
		}
	}
}
