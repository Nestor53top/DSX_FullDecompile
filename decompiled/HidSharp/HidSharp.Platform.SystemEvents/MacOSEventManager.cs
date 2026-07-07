using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace HidSharp.Platform.SystemEvents;

internal sealed class MacOSEventManager : PosixEventManager
{
	private sealed class JobHandle
	{
		internal int? NotifyToken;
	}

	private uint _notifyFD;

	private Dictionary<int, JobHandle> _notifyTokens;

	private Thread _notifyThread;

	internal override void Start()
	{
		_notifyTokens = new Dictionary<int, JobHandle>();
		base.Start();
	}

	protected override PosixNativeMethods CreateNativeMethods()
	{
		return new MacOSNativeMethods();
	}

	protected override object CreateJobObject()
	{
		return new JobHandle();
	}

	protected override void RegisterJobObjectNotify(object jobObject, string eventName, string shmName)
	{
		JobHandle jobHandle = (JobHandle)jobObject;
		if (shmName != null)
		{
			bool flag = _notifyThread != null;
			if (!flag && MacOSNativeMethods.mach_port_allocate(MacOSNativeMethods.GetMachTaskSelf(), 1u, out _notifyFD) != 0)
			{
				throw new InvalidOperationException("Failed to create Mach port!");
			}
			int token;
			uint num = MacOSNativeMethods.notify_register_mach_port(shmName, ref _notifyFD, 1, out token);
			if (num != 0)
			{
				throw new InvalidOperationException("Failed to register notify file descriptor! " + num);
			}
			_notifyTokens.Add(token, jobHandle);
			jobHandle.NotifyToken = token;
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
	}

	protected override void UnregisterJobObjectNotify(object jobObject)
	{
		JobHandle jobHandle = (JobHandle)jobObject;
		if (jobHandle.NotifyToken.HasValue)
		{
			int value = jobHandle.NotifyToken.Value;
			if (MacOSNativeMethods.notify_cancel(value) != 0)
			{
				throw new InvalidOperationException("Failed to cancel notify token!");
			}
			jobHandle.NotifyToken = null;
			_notifyTokens.Remove(value);
		}
	}

	protected override void RunNotify(FileStream eventStream, string eventName, string shmName)
	{
		MacOSNativeMethods.notify_post(shmName);
	}

	private unsafe void RunNotifyThread()
	{
		uint notifyFD = _notifyFD;
		int recv_size = Marshal.SizeOf(typeof(MacOSNativeMethods.mach_msg_t));
		while (true)
		{
			MacOSNativeMethods.mach_msg_t mach_msg_t = default(MacOSNativeMethods.mach_msg_t);
			if (MacOSNativeMethods.mach_msg_overwrite(IntPtr.Zero, 2, 0u, (uint)recv_size, notifyFD, 0u, 0u, (IntPtr)(&mach_msg_t), 0u) != 0)
			{
				continue;
			}
			int msgh_id = mach_msg_t.header.msgh_id;
			lock (base.SyncRoot)
			{
				if (_notifyTokens.TryGetValue(msgh_id, out var value))
				{
					RunJobObject(value);
				}
			}
		}
	}
}
