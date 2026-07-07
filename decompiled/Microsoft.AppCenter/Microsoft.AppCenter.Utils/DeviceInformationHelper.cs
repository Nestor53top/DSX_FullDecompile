using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Microsoft.AppCenter.Utils;

public class DeviceInformationHelper : AbstractDeviceInformationHelper
{
	private IManagmentClassFactory _managmentClassFactory;

	private static string DeploymentVersion
	{
		get
		{
			if (ApplicationDeployment.IsNetworkDeployed)
			{
				return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
			}
			return null;
		}
	}

	private static string FileVersion
	{
		get
		{
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			if (entryAssembly != null)
			{
				string text = entryAssembly.Location;
				if (string.IsNullOrWhiteSpace(text))
				{
					text = Environment.GetCommandLineArgs()[0];
				}
				return FileVersionInfo.GetVersionInfo(text).FileVersion;
			}
			return Application.ProductVersion;
		}
	}

	public DeviceInformationHelper()
	{
		_managmentClassFactory = ManagmentClassFactory.Instance;
	}

	internal void SetManagmentClassFactory(IManagmentClassFactory factory)
	{
		_managmentClassFactory = factory;
	}

	protected override string GetSdkName()
	{
		if (!WpfHelper.IsRunningOnWpf)
		{
			return "appcenter.winforms";
		}
		return "appcenter.wpf";
	}

	protected override string GetDeviceModel()
	{
		try
		{
			ManagementObjectEnumerator enumerator = _managmentClassFactory.GetComputerSystemClass().GetInstances().GetEnumerator();
			try
			{
				if (enumerator.MoveNext())
				{
					string text = (string)enumerator.Current["Model"];
					return (string.IsNullOrEmpty(text) || AbstractDeviceInformationHelper.DefaultSystemProductName == text) ? null : text;
				}
			}
			finally
			{
				((IDisposable)enumerator)?.Dispose();
			}
		}
		catch (UnauthorizedAccessException exception)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device model with error: ", exception);
			return string.Empty;
		}
		return string.Empty;
	}

	protected override string GetAppNamespace()
	{
		return Assembly.GetEntryAssembly()?.EntryPoint.DeclaringType?.Namespace;
	}

	protected override string GetDeviceOemName()
	{
		try
		{
			ManagementObjectEnumerator enumerator = _managmentClassFactory.GetComputerSystemClass().GetInstances().GetEnumerator();
			try
			{
				if (enumerator.MoveNext())
				{
					string text = (string)enumerator.Current["Manufacturer"];
					return (string.IsNullOrEmpty(text) || AbstractDeviceInformationHelper.DefaultSystemManufacturer == text) ? null : text;
				}
			}
			finally
			{
				((IDisposable)enumerator)?.Dispose();
			}
		}
		catch (UnauthorizedAccessException exception)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device OEM name with error: ", exception);
			return string.Empty;
		}
		return string.Empty;
	}

	protected override string GetOsName()
	{
		return "WINDOWS";
	}

	protected override string GetOsBuild()
	{
		RegistryKey localMachine = Registry.LocalMachine;
		try
		{
			RegistryKey val = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
			try
			{
				object value = val.GetValue("CurrentMajorVersionNumber");
				if (value != null)
				{
					object value2 = val.GetValue("CurrentMinorVersionNumber", (object)"0");
					object value3 = val.GetValue("CurrentBuildNumber", (object)"0");
					object value4 = val.GetValue("UBR", (object)"0");
					return string.Format("{0}.{1}.{2}.{3}", new object[4] { value, value2, value3, value4 });
				}
				object value5 = val.GetValue("CurrentVersion", (object)"0.0");
				object value6 = val.GetValue("CurrentBuild", (object)"0");
				string[] array = val.GetValue("BuildLabEx")?.ToString().Split(new char[1] { '.' });
				string arg = ((array != null && array.Length >= 2) ? array[1] : "0");
				return $"{value5}.{value6}.{arg}";
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)localMachine)?.Dispose();
		}
	}

	protected override string GetOsVersion()
	{
		try
		{
			ManagementObjectEnumerator enumerator = _managmentClassFactory.GetOperatingSystemClass().GetInstances().GetEnumerator();
			try
			{
				if (enumerator.MoveNext())
				{
					return (string)enumerator.Current["Version"];
				}
			}
			finally
			{
				((IDisposable)enumerator)?.Dispose();
			}
		}
		catch (UnauthorizedAccessException exception)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device OS version with error: ", exception);
			return string.Empty;
		}
		return string.Empty;
	}

	protected override string GetAppVersion()
	{
		return DeploymentVersion ?? Application.ProductVersion;
	}

	protected override string GetAppBuild()
	{
		return DeploymentVersion ?? FileVersion;
	}

	protected override string GetScreenSize()
	{
		Graphics val = Graphics.FromHwnd(IntPtr.Zero);
		try
		{
			IntPtr hdc = val.GetHdc();
			int deviceCaps = GetDeviceCaps(hdc, 117);
			int deviceCaps2 = GetDeviceCaps(hdc, 118);
			return $"{deviceCaps2}x{deviceCaps}";
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	[DllImport("gdi32.dll")]
	private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
}
