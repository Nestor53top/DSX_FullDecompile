using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal;

internal static class SyncViaAsync
{
	[ThreadStatic]
	private static bool _isSynchronous;

	public static bool IsSynchronous => _isSynchronous;

	public static void Run<TState>(Func<TState, ValueTask> action, TState state)
	{
		Run<(Func<TState, ValueTask>, TState), bool>(async delegate((Func<TState, ValueTask> action, TState state) s)
		{
			await s.action(s.state).ConfigureAwait(continueOnCapturedContext: false);
			return true;
		}, (action, state));
	}

	public static TResult Run<TState, TResult>(Func<TState, ValueTask<TResult>> action, TState state)
	{
		try
		{
			_isSynchronous = true;
			ValueTask<TResult> valueTask = action(state);
			if (!valueTask.IsCompleted)
			{
				return valueTask.AsTask().GetAwaiter().GetResult();
			}
			return valueTask.GetAwaiter().GetResult();
		}
		finally
		{
			_isSynchronous = false;
		}
	}

	public static ValueTask Delay(TimeoutValue timeout, CancellationToken cancellationToken)
	{
		if (!IsSynchronous)
		{
			return Task.Delay(timeout.InMilliseconds, cancellationToken).AsValueTask();
		}
		if (cancellationToken.CanBeCanceled)
		{
			if (cancellationToken.WaitHandle.WaitOne(timeout.InMilliseconds))
			{
				throw new OperationCanceledException("delay was canceled", cancellationToken);
			}
		}
		else
		{
			Thread.Sleep(timeout.InMilliseconds);
		}
		return default(ValueTask);
	}

	public static void DisposeSyncViaAsync<TDisposable>(this TDisposable disposable) where TDisposable : IAsyncDisposable, IDisposable
	{
		Run((TDisposable @this) => @this.DisposeAsync(), disposable);
	}

	public static ValueTask<TResult> AwaitSyncOverAsync<TResult>(this Task<TResult> task)
	{
		if (!IsSynchronous)
		{
			return task.AsValueTask();
		}
		return task.GetAwaiter().GetResult().AsValueTask();
	}

	public static ValueTask AwaitSyncOverAsync(this Task task)
	{
		if (IsSynchronous)
		{
			task.GetAwaiter().GetResult();
			return default(ValueTask);
		}
		return task.AsValueTask();
	}
}
