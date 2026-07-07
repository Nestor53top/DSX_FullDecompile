using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace HidSharp.Platform.SystemEvents;

internal sealed class LinuxEventManager : PosixEventManager
{
	private sealed class JobHandle
	{
		internal int? WatchDescriptor;
	}

	private int _notifyFD;

	private Dictionary<int, JobHandle> _watchDescriptors;

	private Thread _notifyThread;

	internal override void Start()
	{
		_watchDescriptors = new Dictionary<int, JobHandle>();
		base.Start();
	}

	protected override object CreateJobObject()
	{
		return new JobHandle();
	}

	protected override void RegisterJobObjectNotify(object jobObject, string eventName, string shmName)
	{
		JobHandle jobHandle = (JobHandle)jobObject;
		if (eventName == null)
		{
			return;
		}
		bool flag = _notifyThread != null;
		if (!flag)
		{
			_notifyFD = LinuxNativeMethods.inotify_init();
			if (_notifyFD < 0)
			{
				throw new InvalidOperationException("Failed to create inotify file descriptor!");
			}
		}
		int num = LinuxNativeMethods.inotify_add_watch(_notifyFD, eventName, 4);
		if (num < 0)
		{
			throw new InvalidOperationException("Failed to add inotify watch descriptor: " + base.NativeMethods.GetLastError());
		}
		_watchDescriptors.Add(num, jobHandle);
		jobHandle.WatchDescriptor = num;
		if (!flag)
		{
			_notifyThread = new Thread(RunNotifyThread)
			{
				IsBackground = true,
				Name = "HID System Events Notification Monitor"
			};
			_notifyThread.Start();
		}
	}

	protected override void UnregisterJobObjectNotify(object jobObject)
	{
		JobHandle jobHandle = (JobHandle)jobObject;
		if (jobHandle.WatchDescriptor.HasValue)
		{
			int value = jobHandle.WatchDescriptor.Value;
			if (LinuxNativeMethods.inotify_rm_watch(_notifyFD, value) < 0)
			{
				throw new InvalidOperationException("Failed to remove inotify watch descriptor.");
			}
			jobHandle.WatchDescriptor = null;
			_watchDescriptors.Remove(value);
		}
	}

	protected override void RunNotify(FileStream eventStream, string eventName, string shmName)
	{
		int num = base.NativeMethods.retry(() => LinuxNativeMethods.utime(eventName, IntPtr.Zero));
		_ = 0;
	}

	private unsafe void RunNotifyThread()
	{
		int notifyFD = _notifyFD;
		int num = Marshal.SizeOf(typeof(LinuxNativeMethods.inotify_event));
		int num2 = num + 255 + 1;
		byte* ptr = stackalloc byte[(int)(uint)num2];
		while (true)
		{
			int num3 = (int)(long)LinuxNativeMethods.read(notifyFD, (IntPtr)ptr, (UIntPtr)(ulong)num2);
			if (num3 < 1)
			{
				break;
			}
			int num4 = 0;
			while (num4 <= num3 - num)
			{
				LinuxNativeMethods.inotify_event inotify_event = (LinuxNativeMethods.inotify_event)Marshal.PtrToStructure((IntPtr)(ptr + num4), typeof(LinuxNativeMethods.inotify_event));
				num4 += num + checked((int)inotify_event.len);
				if ((inotify_event.mask & 4) == 0)
				{
					continue;
				}
				lock (base.SyncRoot)
				{
					if (_watchDescriptors.TryGetValue(inotify_event.wd, out var value))
					{
						RunJobObject(value);
					}
				}
			}
		}
	}

	protected override PosixNativeMethods CreateNativeMethods()
	{
		return new LinuxNativeMethods();
	}
}
