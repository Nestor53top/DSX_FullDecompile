using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace HidSharp.Platform.SystemEvents;

internal class DefaultEventManager : EventManager
{
	private sealed class DefaultEvent : SystemEvent
	{
		private bool _createdNew;

		private EventWaitHandle _event;

		public override bool CreatedNew => _createdNew;

		public override WaitHandle WaitHandle => _event;

		public DefaultEvent(string name)
			: base(name)
		{
			_event = new EventWaitHandle(initialState: false, EventResetMode.ManualReset, GetGlobalName(name), out _createdNew);
		}

		public override void Dispose()
		{
			try
			{
				if (_event != null)
				{
					_event.Close();
					_event = null;
				}
			}
			catch
			{
			}
		}

		public override void Reset()
		{
			try
			{
				_event.Reset();
			}
			catch
			{
			}
		}

		public override void Set()
		{
			try
			{
				_event.Set();
			}
			catch
			{
			}
		}
	}

	private sealed class DefaultMutex : SystemMutex
	{
		private bool _createdNew;

		private Mutex _mutex;

		public override bool CreatedNew => _createdNew;

		public DefaultMutex(string name)
			: base(name)
		{
			_mutex = new Mutex(initiallyOwned: false, GetGlobalName(name), out _createdNew);
		}

		public override void Dispose()
		{
			try
			{
				if (_mutex != null)
				{
					_mutex.Close();
					_mutex = null;
				}
			}
			catch (Exception)
			{
			}
		}

		protected override bool WaitOne(int timeout)
		{
			if (!_mutex.WaitOne(timeout))
			{
				return false;
			}
			return true;
		}

		protected override void ReleaseMutex()
		{
			if (_mutex != null)
			{
				_mutex.ReleaseMutex();
			}
		}
	}

	private static string GetGlobalName(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException();
		}
		if (name.Length > 240)
		{
			name = "HIDSharp Global (" + Convert.ToBase64String(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(name))) + ")";
		}
		return "Global\\" + name;
	}

	internal override void Start()
	{
	}

	public override SystemEvent CreateEvent(string name)
	{
		return new DefaultEvent(name);
	}

	public override SystemMutex CreateMutex(string name)
	{
		return new DefaultMutex(name);
	}
}
