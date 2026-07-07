using System;
using System.Collections.Generic;
using System.Threading;

namespace HidSharp.Platform.SystemEvents;

internal abstract class SystemMutex : IDisposable
{
	private sealed class ResourceLock : IDisposable
	{
		private int _disposed;

		internal SystemMutex M;

		public void Dispose()
		{
			if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
			{
				return;
			}
			try
			{
				M.ReleaseMutexOuter();
			}
			catch (Exception)
			{
			}
		}
	}

	private static HashSet<string> _antirecursionList = new HashSet<string>();

	private Thread _lockThread;

	public abstract bool CreatedNew { get; }

	public string Name { get; private set; }

	protected SystemMutex(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException();
		}
		Name = name;
	}

	public abstract void Dispose();

	protected abstract bool WaitOne(int timeout);

	protected abstract void ReleaseMutex();

	public bool TryLock(out IDisposable @lock)
	{
		return TryLock(-1, out @lock);
	}

	public bool TryLock(int timeout, out IDisposable @lock)
	{
		@lock = null;
		try
		{
			if (!WaitOneOuter(timeout))
			{
				return false;
			}
		}
		catch (AbandonedMutexException)
		{
			return false;
		}
		@lock = new ResourceLock
		{
			M = this
		};
		return true;
	}

	private bool WaitOneOuter(int timeout)
	{
		if (!WaitOneInner(timeout))
		{
			return false;
		}
		lock (_antirecursionList)
		{
			if (_antirecursionList.Contains(Name))
			{
				ReleaseMutexInner();
				return false;
			}
			_antirecursionList.Add(Name);
			return true;
		}
	}

	private bool WaitOneInner(int timeout)
	{
		try
		{
			if (!WaitOne(timeout))
			{
				return false;
			}
		}
		catch (Exception)
		{
			return false;
		}
		if (_lockThread != null)
		{
			throw new InvalidOperationException();
		}
		_lockThread = Thread.CurrentThread;
		return true;
	}

	private void ReleaseMutexOuter()
	{
		lock (_antirecursionList)
		{
			_antirecursionList.Remove(Name);
			ReleaseMutexInner();
		}
	}

	private void ReleaseMutexInner()
	{
		if (_lockThread != Thread.CurrentThread)
		{
			throw new InvalidOperationException();
		}
		ReleaseMutex();
		_lockThread = null;
	}
}
