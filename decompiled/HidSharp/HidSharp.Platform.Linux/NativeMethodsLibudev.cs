using System;

namespace HidSharp.Platform.Linux;

internal abstract class NativeMethodsLibudev
{
	public static NativeMethodsLibudev Instance { get; private set; }

	public abstract string FriendlyName { get; }

	static NativeMethodsLibudev()
	{
		NativeMethodsLibudev[] array = new NativeMethodsLibudev[2]
		{
			new NativeMethodsLibudev1(),
			new NativeMethodsLibudev0()
		};
		foreach (NativeMethodsLibudev nativeMethodsLibudev in array)
		{
			try
			{
				IntPtr intPtr = nativeMethodsLibudev.udev_new();
				if (IntPtr.Zero != intPtr)
				{
					nativeMethodsLibudev.udev_unref(intPtr);
					Instance = nativeMethodsLibudev;
					break;
				}
			}
			catch
			{
			}
		}
	}

	public abstract IntPtr udev_new();

	public abstract IntPtr udev_ref(IntPtr udev);

	public abstract void udev_unref(IntPtr udev);

	public abstract IntPtr udev_monitor_new_from_netlink(IntPtr udev, string name);

	public abstract void udev_monitor_unref(IntPtr monitor);

	public abstract int udev_monitor_filter_add_match_subsystem_devtype(IntPtr monitor, string subsystem, string devtype);

	public abstract int udev_monitor_enable_receiving(IntPtr monitor);

	public abstract int udev_monitor_get_fd(IntPtr monitor);

	public abstract IntPtr udev_monitor_receive_device(IntPtr monitor);

	public abstract IntPtr udev_enumerate_new(IntPtr udev);

	public abstract IntPtr udev_enumerate_ref(IntPtr enumerate);

	public abstract void udev_enumerate_unref(IntPtr enumerate);

	public abstract int udev_enumerate_add_match_subsystem(IntPtr enumerate, string subsystem);

	public abstract int udev_enumerate_scan_devices(IntPtr enumerate);

	public abstract IntPtr udev_enumerate_get_list_entry(IntPtr enumerate);

	public abstract IntPtr udev_list_entry_get_next(IntPtr entry);

	public abstract string udev_list_entry_get_name(IntPtr entry);

	public abstract IntPtr udev_device_new_from_syspath(IntPtr udev, string syspath);

	public abstract IntPtr udev_device_ref(IntPtr device);

	public abstract void udev_device_unref(IntPtr device);

	public abstract string udev_device_get_devnode(IntPtr device);

	public abstract IntPtr udev_device_get_parent_with_subsystem_devtype(IntPtr device, string subsystem, string devtype);

	public abstract string udev_device_get_sysattr_value(IntPtr device, string sysattr);

	public abstract int udev_device_get_is_initialized(IntPtr device);
}
