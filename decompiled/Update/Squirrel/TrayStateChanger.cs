using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Squirrel.SimpleSplat;

namespace Squirrel;

internal class TrayStateChanger : IEnableLogger
{
	private class NotificationCb : INotificationCb
	{
		public readonly List<NOTIFYITEM> items = new List<NOTIFYITEM>();

		public void Notify([In] uint nEvent, [In] ref NOTIFYITEM notifyItem)
		{
			items.Add(notifyItem);
		}
	}

	public List<NOTIFYITEM> GetTrayItems()
	{
		TrayNotify trayNotify = new TrayNotify();
		try
		{
			if (useLegacyInterface())
			{
				return getTrayItemsWin7(trayNotify);
			}
			return getTrayItems(trayNotify);
		}
		finally
		{
			Marshal.ReleaseComObject(trayNotify);
		}
	}

	public void PromoteTrayItem(string exeToPromote)
	{
		TrayNotify trayNotify = new TrayNotify();
		try
		{
			List<NOTIFYITEM> list = null;
			bool flag = useLegacyInterface();
			list = ((!flag) ? getTrayItems(trayNotify) : getTrayItemsWin7(trayNotify));
			exeToPromote = exeToPromote.ToLowerInvariant();
			for (int i = 0; i < list.Count; i++)
			{
				NOTIFYITEM item = list[i];
				if (item.exe_name.ToLowerInvariant().Contains(exeToPromote) && item.preference == NOTIFYITEM_PREFERENCE.PREFERENCE_SHOW_WHEN_ACTIVE)
				{
					item.preference = NOTIFYITEM_PREFERENCE.PREFERENCE_SHOW_ALWAYS;
					NOTIFYITEM_Writable notifyItem = NOTIFYITEM_Writable.fromNotifyItem(item);
					if (flag)
					{
						((ITrayNotifyWin7)trayNotify).SetPreference(ref notifyItem);
					}
					else
					{
						((ITrayNotify)trayNotify).SetPreference(ref notifyItem);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Failed to promote Tray icon: " + ex.ToString());
		}
		finally
		{
			Marshal.ReleaseComObject(trayNotify);
		}
	}

	public unsafe void RemoveDeadEntries(List<string> executablesInPackage, string rootAppDirectory, string currentAppVersion)
	{
		byte[] array = null;
		try
		{
			array = (byte[])Registry.GetValue("HKEY_CURRENT_USER\\Software\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\TrayNotify", "IconStreams", new byte[1]);
		}
		catch (Exception ex)
		{
			Console.WriteLine("Couldn't load IconStreams key, bailing: " + ex.ToString());
			return;
		}
		if (array == null || array.Length < 20)
		{
			return;
		}
		List<byte[]> list = new List<byte[]>();
		IconStreamsHeader iconStreamsHeader = default(IconStreamsHeader);
		fixed (byte* ptr = array)
		{
			iconStreamsHeader = (IconStreamsHeader)Marshal.PtrToStructure((IntPtr)ptr, typeof(IconStreamsHeader));
			if (iconStreamsHeader.count <= 1)
			{
				return;
			}
			byte* ptr2;
			for (int i = 0; i < iconStreamsHeader.count; i++)
			{
				int num = Marshal.SizeOf(typeof(IconStreamsHeader)) + i * Marshal.SizeOf(typeof(IconStreamsItem));
				if (num > array.Length)
				{
					this.Log().Error("Corrupted IconStreams regkey, bailing");
					return;
				}
				ptr2 = ptr + num;
				IconStreamsItem iconStreamsItem = (IconStreamsItem)Marshal.PtrToStructure((IntPtr)ptr2, typeof(IconStreamsItem));
				try
				{
					string path = iconStreamsItem.ExePath.ToLowerInvariant();
					if (!executablesInPackage.Any((string exe) => path.Contains(exe)) || !path.StartsWith(rootAppDirectory, StringComparison.Ordinal) || path.Contains("app-" + currentAppVersion))
					{
						byte[] array2 = new byte[Marshal.SizeOf(typeof(IconStreamsItem))];
						Array.Copy(array, num, array2, 0, array2.Length);
						list.Add(array2);
					}
				}
				catch (Exception exception)
				{
					this.Log().ErrorException("Failed to parse IconStreams regkey", exception);
					return;
				}
			}
			if (iconStreamsHeader.count == list.Count)
			{
				return;
			}
			iconStreamsHeader.count = (uint)list.Count;
			Marshal.StructureToPtr((object)iconStreamsHeader, (IntPtr)ptr, false);
			ptr2 = ptr + Marshal.SizeOf(typeof(IconStreamsHeader));
			for (int num2 = 0; num2 < list.Count; num2++)
			{
				Marshal.Copy(list[num2], 0, (IntPtr)ptr2, list[num2].Length);
				ptr2 += list[num2].Length;
			}
		}
		try
		{
			int num3 = Marshal.SizeOf(typeof(IconStreamsHeader)) + list.Count * Marshal.SizeOf(typeof(IconStreamsItem));
			byte[] array3 = new byte[num3];
			Array.Copy(array, array3, num3);
			Registry.SetValue("HKEY_CURRENT_USER\\Software\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\TrayNotify", "IconStreams", array3);
		}
		catch (Exception ex2)
		{
			Console.WriteLine("Failed to write new IconStreams regkey: " + ex2.ToString());
		}
	}

	private static List<NOTIFYITEM> getTrayItems(TrayNotify instance)
	{
		ITrayNotify obj = (ITrayNotify)instance;
		NotificationCb notificationCb = new NotificationCb();
		ulong handle = 0uL;
		obj.RegisterCallback(notificationCb, out handle);
		obj.UnregisterCallback(handle);
		return notificationCb.items;
	}

	private static List<NOTIFYITEM> getTrayItemsWin7(TrayNotify instance)
	{
		ITrayNotifyWin7 obj = (ITrayNotifyWin7)instance;
		NotificationCb notificationCb = new NotificationCb();
		obj.RegisterCallback(notificationCb);
		obj.RegisterCallback(null);
		return notificationCb.items;
	}

	private static bool useLegacyInterface()
	{
		Version version = Environment.OSVersion.Version;
		if (version.Major < 6)
		{
			return true;
		}
		if (version.Major > 6)
		{
			return false;
		}
		return version.Minor <= 1;
	}
}
