using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HidSharp.Platform.SystemEvents;

namespace HidSharp.Platform.Linux;

internal sealed class LinuxHidManager : HidManager
{
	public override string FriendlyName
	{
		get
		{
			NativeMethodsLibudev instance = NativeMethodsLibudev.Instance;
			return "Linux hidraw (" + ((instance != null) ? instance.FriendlyName : "?") + ")";
		}
	}

	public override bool IsSupported
	{
		get
		{
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				try
				{
					if (NativeMethods.uname(out string sysname, out Version release) && sysname == "Linux" && release >= new Version(2, 6, 36) && NativeMethodsLibudev.Instance != null)
					{
						return true;
					}
				}
				catch (Exception)
				{
				}
			}
			return false;
		}
	}

	protected override EventManager CreateEventManager()
	{
		return new LinuxEventManager();
	}

	protected override void Run(Action readyCallback)
	{
		IntPtr intPtr = NativeMethodsLibudev.Instance.udev_new();
		HidManager.RunAssert(intPtr != IntPtr.Zero, "HidSharp udev_new failed.");
		try
		{
			IntPtr intPtr2 = NativeMethodsLibudev.Instance.udev_monitor_new_from_netlink(intPtr, "udev");
			HidManager.RunAssert(intPtr2 != IntPtr.Zero, "HidSharp udev_monitor_new_from_netlink failed.");
			try
			{
				int num = NativeMethodsLibudev.Instance.udev_monitor_filter_add_match_subsystem_devtype(intPtr2, "hid", null);
				HidManager.RunAssert(num >= 0, "HidSharp udev_monitor_failed_add_match_subsystem_devtype failed.");
				num = NativeMethodsLibudev.Instance.udev_monitor_enable_receiving(intPtr2);
				HidManager.RunAssert(num >= 0, "HidSharp udev_monitor_enable_receiving failed.");
				int num2 = NativeMethodsLibudev.Instance.udev_monitor_get_fd(intPtr2);
				HidManager.RunAssert(num2 >= 0, "HidSharp udev_monitor_get_fd failed.");
				NativeMethods.pollfd[] fds = new NativeMethods.pollfd[1];
				fds[0].fd = num2;
				fds[0].events = NativeMethods.pollev.IN;
				readyCallback();
				while (true)
				{
					num = NativeMethods.retry(() => NativeMethods.poll(fds, (IntPtr)1, -1));
					if (num < 0)
					{
						break;
					}
					if (num == 1)
					{
						if ((fds[0].revents & (NativeMethods.pollev.ERR | NativeMethods.pollev.HUP | NativeMethods.pollev.NVAL)) != 0)
						{
							break;
						}
						if ((fds[0].revents & NativeMethods.pollev.IN) != 0)
						{
							IntPtr device = NativeMethodsLibudev.Instance.udev_monitor_receive_device(intPtr2);
							NativeMethodsLibudev.Instance.udev_device_unref(device);
							DeviceList.Local.RaiseChanged();
						}
					}
				}
			}
			finally
			{
				NativeMethodsLibudev.Instance.udev_monitor_unref(intPtr2);
			}
		}
		finally
		{
			NativeMethodsLibudev.Instance.udev_unref(intPtr);
		}
	}

	protected override object[] GetBleDeviceKeys()
	{
		return new object[0];
	}

	protected override object[] GetHidDeviceKeys()
	{
		return GetDeviceKeys("hidraw");
	}

	protected override object[] GetSerialDeviceKeys()
	{
		try
		{
			return (from name in Directory.GetFiles("/dev/")
				where name.StartsWith("/dev/ttyACM") || name.StartsWith("/dev/ttyUSB")
				select name).Cast<object>().ToArray();
		}
		catch
		{
			return new object[0];
		}
	}

	private object[] GetDeviceKeys(string subsystem)
	{
		List<string> list = new List<string>();
		IntPtr intPtr = NativeMethodsLibudev.Instance.udev_new();
		if (IntPtr.Zero != intPtr)
		{
			try
			{
				IntPtr intPtr2 = NativeMethodsLibudev.Instance.udev_enumerate_new(intPtr);
				if (IntPtr.Zero != intPtr2)
				{
					try
					{
						if (NativeMethodsLibudev.Instance.udev_enumerate_add_match_subsystem(intPtr2, subsystem) == 0 && NativeMethodsLibudev.Instance.udev_enumerate_scan_devices(intPtr2) == 0)
						{
							IntPtr intPtr3 = NativeMethodsLibudev.Instance.udev_enumerate_get_list_entry(intPtr2);
							while (intPtr3 != IntPtr.Zero)
							{
								string text = NativeMethodsLibudev.Instance.udev_list_entry_get_name(intPtr3);
								if (text != null)
								{
									list.Add(text);
								}
								intPtr3 = NativeMethodsLibudev.Instance.udev_list_entry_get_next(intPtr3);
							}
						}
					}
					finally
					{
						NativeMethodsLibudev.Instance.udev_enumerate_unref(intPtr2);
					}
				}
			}
			finally
			{
				NativeMethodsLibudev.Instance.udev_unref(intPtr);
			}
		}
		return list.Cast<object>().ToArray();
	}

	protected override bool TryCreateBleDevice(object key, out Device device)
	{
		throw new NotImplementedException();
	}

	protected override bool TryCreateHidDevice(object key, out Device device)
	{
		device = LinuxHidDevice.TryCreate((string)key);
		return device != null;
	}

	protected override bool TryCreateSerialDevice(object key, out Device device)
	{
		device = LinuxSerialDevice.TryCreate((string)key);
		return true;
	}
}
