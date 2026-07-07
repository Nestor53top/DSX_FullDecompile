using System;
using Microsoft.Win32;

namespace NuGet;

public static class EnvironmentUtility
{
	private static bool _runningFromCommandLine;

	private static readonly bool _isMonoRuntime = Type.GetType("Mono.Runtime") != null;

	public static bool IsMonoRuntime => _isMonoRuntime;

	public static bool RunningFromCommandLine => _runningFromCommandLine;

	public static bool IsNet45Installed
	{
		get
		{
			RegistryKey val = RegistryKey.OpenBaseKey((RegistryHive)(-2147483646), (RegistryView)512);
			try
			{
				RegistryKey val2 = val.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\");
				try
				{
					if (val2 == null)
					{
						return false;
					}
					object value = val2.GetValue("Release");
					return value is int && (int)value >= 378389;
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	public static void SetRunningFromCommandLine()
	{
		_runningFromCommandLine = true;
	}
}
