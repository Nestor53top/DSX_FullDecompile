using System;
using System.Threading;

namespace HidSharp.Platform.SystemEvents;

internal abstract class SystemEvent : IDisposable
{
	public abstract bool CreatedNew { get; }

	public string Name { get; private set; }

	public abstract WaitHandle WaitHandle { get; }

	protected SystemEvent(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException();
		}
		Name = name;
	}

	public abstract void Dispose();

	public abstract void Reset();

	public abstract void Set();

	public bool Wait(int timeout)
	{
		try
		{
			return WaitHandle.WaitOne(timeout);
		}
		catch (Exception)
		{
			return false;
		}
	}
}
