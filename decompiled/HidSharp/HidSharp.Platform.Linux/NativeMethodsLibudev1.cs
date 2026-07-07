using System;
using System.Runtime.InteropServices;

namespace HidSharp.Platform.Linux;

internal sealed class NativeMethodsLibudev1 : NativeMethodsLibudev
{
	private const string libudev = "libudev.so.1";

	public override string FriendlyName => "libudev.so.1";

	[DllImport("libudev.so.1", EntryPoint = "udev_new")]
	private static extern IntPtr native_udev_new();

	public override IntPtr udev_new()
	{
		return native_udev_new();
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_ref")]
	private static extern IntPtr native_udev_ref(IntPtr udev);

	public override IntPtr udev_ref(IntPtr udev)
	{
		return native_udev_ref(udev);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_unref")]
	private static extern void native_udev_unref(IntPtr udev);

	public override void udev_unref(IntPtr udev)
	{
		native_udev_unref(udev);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_monitor_new_from_netlink")]
	private static extern IntPtr native_udev_monitor_new_from_netlink(IntPtr udev, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "HidSharp.Platform.Utf8Marshaler")] string name);

	public override IntPtr udev_monitor_new_from_netlink(IntPtr udev, string name)
	{
		return native_udev_monitor_new_from_netlink(udev, name);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_monitor_unref")]
	private static extern void native_udev_monitor_unref(IntPtr monitor);

	public override void udev_monitor_unref(IntPtr monitor)
	{
		native_udev_monitor_unref(monitor);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_monitor_filter_add_match_subsystem_devtype")]
	private static extern int native_udev_monitor_filter_add_match_subsystem_devtype(IntPtr monitor, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "HidSharp.Platform.Utf8Marshaler")] string subsystem, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "HidSharp.Platform.Utf8Marshaler")] string devtype);

	public override int udev_monitor_filter_add_match_subsystem_devtype(IntPtr monitor, string subsystem, string devtype)
	{
		return native_udev_monitor_filter_add_match_subsystem_devtype(monitor, subsystem, devtype);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_monitor_enable_receiving")]
	private static extern int native_udev_monitor_enable_receiving(IntPtr monitor);

	public override int udev_monitor_enable_receiving(IntPtr monitor)
	{
		return native_udev_monitor_enable_receiving(monitor);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_monitor_get_fd")]
	private static extern int native_udev_monitor_get_fd(IntPtr monitor);

	public override int udev_monitor_get_fd(IntPtr monitor)
	{
		return native_udev_monitor_get_fd(monitor);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_monitor_receive_device")]
	private static extern IntPtr native_udev_monitor_receive_device(IntPtr monitor);

	public override IntPtr udev_monitor_receive_device(IntPtr monitor)
	{
		return native_udev_monitor_receive_device(monitor);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_enumerate_new")]
	private static extern IntPtr native_udev_enumerate_new(IntPtr udev);

	public override IntPtr udev_enumerate_new(IntPtr udev)
	{
		return native_udev_enumerate_new(udev);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_enumerate_ref")]
	private static extern IntPtr native_udev_enumerate_ref(IntPtr enumerate);

	public override IntPtr udev_enumerate_ref(IntPtr enumerate)
	{
		return native_udev_enumerate_ref(enumerate);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_enumerate_unref")]
	private static extern void native_udev_enumerate_unref(IntPtr enumerate);

	public override void udev_enumerate_unref(IntPtr enumerate)
	{
		native_udev_enumerate_unref(enumerate);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_enumerate_add_match_subsystem")]
	private static extern int native_udev_enumerate_add_match_subsystem(IntPtr enumerate, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "HidSharp.Platform.Utf8Marshaler")] string subsystem);

	public override int udev_enumerate_add_match_subsystem(IntPtr enumerate, string subsystem)
	{
		return native_udev_enumerate_add_match_subsystem(enumerate, subsystem);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_enumerate_scan_devices")]
	private static extern int native_udev_enumerate_scan_devices(IntPtr enumerate);

	public override int udev_enumerate_scan_devices(IntPtr enumerate)
	{
		return native_udev_enumerate_scan_devices(enumerate);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_enumerate_get_list_entry")]
	private static extern IntPtr native_udev_enumerate_get_list_entry(IntPtr enumerate);

	public override IntPtr udev_enumerate_get_list_entry(IntPtr enumerate)
	{
		return native_udev_enumerate_get_list_entry(enumerate);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_list_entry_get_next")]
	private static extern IntPtr native_udev_list_entry_get_next(IntPtr entry);

	public override IntPtr udev_list_entry_get_next(IntPtr entry)
	{
		return native_udev_list_entry_get_next(entry);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_list_entry_get_name")]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "HidSharp.Platform.Utf8Marshaler")]
	private static extern string native_udev_list_entry_get_name(IntPtr entry);

	public override string udev_list_entry_get_name(IntPtr entry)
	{
		return native_udev_list_entry_get_name(entry);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_device_new_from_syspath")]
	private static extern IntPtr native_udev_device_new_from_syspath(IntPtr udev, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "HidSharp.Platform.Utf8Marshaler")] string syspath);

	public override IntPtr udev_device_new_from_syspath(IntPtr udev, string syspath)
	{
		return native_udev_device_new_from_syspath(udev, syspath);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_device_ref")]
	private static extern IntPtr native_udev_device_ref(IntPtr device);

	public override IntPtr udev_device_ref(IntPtr device)
	{
		return native_udev_device_ref(device);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_device_unref")]
	private static extern void native_udev_device_unref(IntPtr device);

	public override void udev_device_unref(IntPtr device)
	{
		native_udev_device_unref(device);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_device_get_devnode")]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "HidSharp.Platform.Utf8Marshaler")]
	private static extern string native_udev_device_get_devnode(IntPtr device);

	public override string udev_device_get_devnode(IntPtr device)
	{
		return native_udev_device_get_devnode(device);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_device_get_parent_with_subsystem_devtype")]
	private static extern IntPtr native_udev_device_get_parent_with_subsystem_devtype(IntPtr device, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "HidSharp.Platform.Utf8Marshaler")] string subsystem, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "HidSharp.Platform.Utf8Marshaler")] string devtype);

	public override IntPtr udev_device_get_parent_with_subsystem_devtype(IntPtr device, string subsystem, string devtype)
	{
		return native_udev_device_get_parent_with_subsystem_devtype(device, subsystem, devtype);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_device_get_sysattr_value")]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "HidSharp.Platform.Utf8Marshaler")]
	private static extern string native_udev_device_get_sysattr_value(IntPtr device, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "HidSharp.Platform.Utf8Marshaler")] string sysattr);

	public override string udev_device_get_sysattr_value(IntPtr device, string sysattr)
	{
		return native_udev_device_get_sysattr_value(device, sysattr);
	}

	[DllImport("libudev.so.1", EntryPoint = "udev_device_get_is_initialized")]
	private static extern int native_udev_device_get_is_initialized(IntPtr device);

	public override int udev_device_get_is_initialized(IntPtr device)
	{
		return native_udev_device_get_is_initialized(device);
	}
}
