using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal;

internal readonly struct AsyncLock
{
	private sealed class Handle : IDisposable
	{
		private SemaphoreSlim? _semaphore;

		public Handle(SemaphoreSlim semaphore)
		{
			_semaphore = semaphore;
		}

		public void Dispose()
		{
			Interlocked.Exchange(ref _semaphore, null)?.Release();
		}
	}

	private readonly SemaphoreSlim _semaphore;

	private AsyncLock(SemaphoreSlim semaphore)
	{
		_semaphore = semaphore;
	}

	public static AsyncLock Create()
	{
		return new AsyncLock(new SemaphoreSlim(1, 1));
	}

	public async ValueTask<IDisposable> AcquireAsync(CancellationToken cancellationToken)
	{
		return await TryAcquireAsync(Timeout.InfiniteTimeSpan, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async ValueTask<IDisposable?> TryAcquireAsync(TimeoutValue timeout, CancellationToken cancellationToken)
	{
		bool flag = ((!SyncViaAsync.IsSynchronous) ? (await _semaphore.WaitAsync(timeout.InMilliseconds, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) : _semaphore.Wait(timeout.InMilliseconds, cancellationToken));
		return flag ? new Handle(_semaphore) : null;
	}
}
