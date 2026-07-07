using System;
using System.Text;
using System.Threading;
using HidSharp.Platform;
using HidSharp.Platform.SystemEvents;
using HidSharp.Utility;

namespace HidSharp;

internal sealed class DeviceOpenUtility
{
	private Device _device;

	private string _resourcePrefix;

	private object _syncRoot;

	private Thread _thread;

	private ManualResetEvent _threadStartEvent;

	private Exception _threadStartError;

	private AutoResetEvent _closeEvent;

	private OpenPriority _priority;

	private bool _interruptible;

	private bool _transient;

	private int _timeoutIfInterruptible;

	private int _timeoutIfTransient;

	public event EventHandler InterruptRequested;

	public DeviceOpenUtility(Device device, string streamPath, OpenConfiguration openConfig)
	{
		_device = device;
		_syncRoot = new object();
		_resourcePrefix = GetResourcePrefix(streamPath);
		_priority = (OpenPriority)openConfig.GetOption(OpenOption.Priority);
		_interruptible = (bool)openConfig.GetOption(OpenOption.Interruptible);
		_transient = (bool)openConfig.GetOption(OpenOption.Transient);
		_timeoutIfInterruptible = (int)openConfig.GetOption(OpenOption.TimeoutIfInterruptible);
		_timeoutIfTransient = (int)openConfig.GetOption(OpenOption.TimeoutIfTransient);
		HidSharpDiagnostics.Trace("Opening a device. Our priority is {0}, our interruptible state is {1}, and our transient state is {2}.", _priority, _interruptible, _transient);
	}

	public void Open()
	{
		Close();
		lock (_syncRoot)
		{
			_closeEvent = new AutoResetEvent(initialState: false);
			_threadStartEvent = new ManualResetEvent(initialState: false);
			_threadStartError = null;
			Thread thread = new Thread(Run);
			thread.IsBackground = true;
			thread.Name = "HID Sharing Monitor";
			Thread thread2 = (_thread = thread);
			thread2.Start();
			try
			{
				_threadStartEvent.WaitOne();
				if (_threadStartError != null)
				{
					thread2.Join();
					throw _threadStartError;
				}
			}
			finally
			{
				_threadStartEvent = null;
				_threadStartError = null;
			}
		}
	}

	private void Run()
	{
		SystemEvent obj = null;
		SystemMutex obj2 = null;
		IDisposable @lock = null;
		SystemMutex obj3 = null;
		SystemMutex obj4 = null;
		try
		{
			EventManager eventManager = HidSelector.Instance.EventManager;
			try
			{
				string resourceName = GetResourceName("Transient");
				obj = eventManager.CreateEvent(GetResourceName("Event"));
				obj2 = eventManager.CreateMutex(GetResourceName("Lock"));
				if (!obj2.TryLock(0, out @lock))
				{
					bool flag = false;
					for (int i = -2; i < (int)_priority; i++)
					{
						if (eventManager.MutexMayExist(GetResourceNameForPriority((OpenPriority)i)))
						{
							flag = true;
							break;
						}
					}
					bool flag2 = eventManager.MutexMayExist(resourceName);
					using (flag ? eventManager.CreateMutex(GetResourceNameForPriorityRequest()) : null)
					{
						obj.Set();
						int num;
						if (flag2)
						{
							num = Math.Max(0, _timeoutIfTransient);
							HidSharpDiagnostics.Trace("Failed to open the device. Luckily, it is in use by a transient process. We will wait {0} ms.", num);
						}
						else if (flag)
						{
							num = Math.Max(0, _timeoutIfInterruptible);
							HidSharpDiagnostics.Trace("Failed to open the device. Luckily, it is in use by an interruptible process. We will wait {0} ms.", num);
						}
						else
						{
							num = 0;
						}
						if (!obj2.TryLock(num, out @lock))
						{
							throw DeviceException.CreateIOException(_device, "The device is in use.", -2147024864);
						}
					}
				}
				if (_transient)
				{
					obj4 = eventManager.CreateMutex(resourceName);
				}
				if (_interruptible)
				{
					obj3 = eventManager.CreateMutex(GetResourceNameForPriority(_priority));
				}
			}
			catch (Exception threadStartError)
			{
				_threadStartError = threadStartError;
				return;
			}
			finally
			{
				_threadStartEvent.Set();
			}
			WaitHandle[] waitHandles = new WaitHandle[2] { _closeEvent, obj.WaitHandle };
			Exception ex = null;
			HidSharpDiagnostics.Trace("Started the sharing monitor thread ({0}).", Thread.CurrentThread.ManagedThreadId);
			while (true)
			{
				try
				{
					if (WaitHandle.WaitAny(waitHandles) == 0)
					{
						break;
					}
				}
				catch (Exception ex2)
				{
					ex = ex2;
					break;
				}
				lock (_syncRoot)
				{
					obj.Reset();
					HidSharpDiagnostics.Trace("Received an interrupt request ({0}).", Thread.CurrentThread.ManagedThreadId);
					if (eventManager.MutexMayExist(GetResourceNameForPriorityRequest()))
					{
						ThreadPool.QueueUserWorkItem(delegate
						{
							this.InterruptRequested?.Invoke(this, EventArgs.Empty);
						});
					}
				}
			}
			HidSharpDiagnostics.Trace("Exited its sharing monitor thread ({0}).{1}", Thread.CurrentThread.ManagedThreadId, (ex != null) ? (" " + ex.ToString()) : "");
		}
		finally
		{
			Close(ref obj3);
			Close(ref obj4);
			Close(ref @lock);
			Close(ref obj2);
			Close(ref obj);
			_closeEvent.Close();
			_closeEvent = null;
			_thread = null;
		}
	}

	public void Close()
	{
		Thread thread = _thread;
		if (thread != null)
		{
			_closeEvent?.Set();
			thread.Join();
		}
	}

	private static void Close<T>(ref T obj) where T : class, IDisposable
	{
		if (obj != null)
		{
			obj.Dispose();
			obj = null;
		}
	}

	private static string GetResourcePrefix(string devicePath)
	{
		string text = "Device Resource : ";
		text += Convert.ToBase64String(Encoding.UTF8.GetBytes(devicePath));
		return text + " : ";
	}

	private string GetResourceName(string property)
	{
		return _resourcePrefix + property;
	}

	private string GetResourceNameForPriority(OpenPriority priority)
	{
		int num = (int)priority;
		return GetResourceName("Priority : " + num);
	}

	private string GetResourceNameForPriorityRequest()
	{
		return GetResourceName("Priority Request");
	}
}
