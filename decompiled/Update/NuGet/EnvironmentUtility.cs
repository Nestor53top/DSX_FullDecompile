using System;
using Microsoft.Win32;

namespace NuGet;

internal static class EnvironmentUtility
{
	private static bool _runningFromCommandLine;

	private static readonly bool _isMonoRuntime = Type.GetType("Mono.Runtime") != null;

	public static bool IsMonoRuntime => _isMonoRuntime;

	public static bool RunningFromCommandLine => _runningFromCommandLine;

	public static bool IsNet45Installed
	{
		get
		{
			using RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
			using RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\");
			if (registryKey2 == null)
			{
				return false;
			}
			object value = registryKey2.GetValue("Release");
			return value is int && (int)value >= 378389;
		}
	}

	public static void SetRunningFromCommandLine()
	{
		_runningFromCommandLine = true;
	}
}
