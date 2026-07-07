using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal;

internal static class BusyWaitHelper
{
	public static async ValueTask<TResult?> WaitAsync<TState, TResult>(TState state, Func<TState, CancellationToken, ValueTask<TResult?>> tryGetValue, TimeoutValue timeout, TimeoutValue minSleepTime, TimeoutValue maxSleepTime, CancellationToken cancellationToken) where TResult : class
	{
		TResult val = await tryGetValue(state, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (val != null || timeout.IsZero)
		{
			return val;
		}
		CancellationToken mergedCancellationToken;
		using (CreateMergedCancellationTokenSourceSource(timeout, cancellationToken, out mergedCancellationToken))
		{
			Random random = new Random(Guid.NewGuid().GetHashCode());
			int sleepRangeMillis = maxSleepTime.InMilliseconds - minSleepTime.InMilliseconds;
			while (true)
			{
				TimeSpan value = minSleepTime.TimeSpan + TimeSpan.FromMilliseconds(random.NextDouble() * (double)sleepRangeMillis);
				try
				{
					await SyncViaAsync.Delay(value, mergedCancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (OperationCanceledException) when (IsTimedOut())
				{
					return await tryGetValue(state, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
				try
				{
					TResult val2 = await tryGetValue(state, mergedCancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					if (val2 != null)
					{
						return val2;
					}
				}
				catch (OperationCanceledException) when (IsTimedOut())
				{
					return null;
				}
			}
		}
		bool IsTimedOut()
		{
			if (mergedCancellationToken.IsCancellationRequested)
			{
				return !cancellationToken.IsCancellationRequested;
			}
			return false;
		}
	}

	private static IDisposable? CreateMergedCancellationTokenSourceSource(TimeoutValue timeout, CancellationToken cancellationToken, out CancellationToken mergedCancellationToken)
	{
		if (timeout.IsInfinite)
		{
			mergedCancellationToken = cancellationToken;
			return null;
		}
		if (!cancellationToken.CanBeCanceled)
		{
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(timeout.InMilliseconds);
			mergedCancellationToken = cancellationTokenSource.Token;
			return cancellationTokenSource;
		}
		CancellationTokenSource cancellationTokenSource2 = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken[1] { cancellationToken });
		cancellationTokenSource2.CancelAfter(timeout.InMilliseconds);
		mergedCancellationToken = cancellationTokenSource2.Token;
		return cancellationTokenSource2;
	}
}
