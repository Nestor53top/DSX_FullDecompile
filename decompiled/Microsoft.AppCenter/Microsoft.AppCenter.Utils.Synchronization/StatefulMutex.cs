using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Utils.Synchronization;

public class StatefulMutex : IDisposable
{
	public class LockHolder : IDisposable
	{
		private readonly StatefulMutex _parent;

		internal LockHolder(StatefulMutex parent)
		{
			_parent = parent;
		}

		public void Dispose()
		{
			_parent._mutex.Release();
		}
	}

	private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

	private State _state = new State();

	public State State => _state;

	public State InvalidateState()
	{
		_state = _state.GetNextState();
		return _state;
	}

	public bool IsCurrent(State state)
	{
		return _state.Equals(state);
	}

	public LockHolder GetLock()
	{
		_mutex.Wait();
		return new LockHolder(this);
	}

	public LockHolder GetLock(State state)
	{
		_mutex.Wait();
		if (IsCurrent(state))
		{
			return new LockHolder(this);
		}
		_mutex.Release();
		throw new StatefulMutexException("Cannot lock mutex with expired state");
	}

	public async Task<LockHolder> GetLockAsync()
	{
		await _mutex.WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
		return new LockHolder(this);
	}

	public async Task<LockHolder> GetLockAsync(State state)
	{
		await _mutex.WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
		if (IsCurrent(state))
		{
			return new LockHolder(this);
		}
		_mutex.Release();
		throw new StatefulMutexException("Cannot lock mutex with expired state");
	}

	public void Dispose()
	{
		_mutex.Dispose();
	}
}
