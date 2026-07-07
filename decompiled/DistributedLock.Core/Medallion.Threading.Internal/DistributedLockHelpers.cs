using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal;

internal static class DistributedLockHelpers
{
	public static string ToSafeName(string name, int maxNameLength, Func<string, string> convertToValidName)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		string text = convertToValidName(name);
		if (text == name && text.Length <= maxNameLength)
		{
			return name;
		}
		using SHA512 sHA = SHA512.Create();
		string text2 = Convert.ToBase64String(sHA.ComputeHash(Encoding.UTF8.GetBytes(name)));
		if (text2.Length >= maxNameLength)
		{
			return text2.Substring(0, maxNameLength);
		}
		return text.Substring(0, Math.Min(text.Length, maxNameLength - text2.Length)) + text2;
	}

	public static async ValueTask<THandle?> Wrap<THandle>(this ValueTask<IDistributedSynchronizationHandle?> handleTask, Func<IDistributedSynchronizationHandle, THandle> factory) where THandle : class
	{
		IDistributedSynchronizationHandle distributedSynchronizationHandle = await handleTask.ConfigureAwait(continueOnCapturedContext: false);
		return (distributedSynchronizationHandle != null) ? factory(distributedSynchronizationHandle) : null;
	}

	public static ValueTask<THandle> AcquireAsync<THandle>(IInternalDistributedLock<THandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken) where THandle : class, IDistributedSynchronizationHandle
	{
		return @lock.InternalTryAcquireAsync(timeout, cancellationToken).ThrowTimeoutIfNull();
	}

	public static THandle Acquire<THandle>(IInternalDistributedLock<THandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken) where THandle : class, IDistributedSynchronizationHandle
	{
		return SyncViaAsync.Run(((IInternalDistributedLock<THandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken) state) => AcquireAsync(state.@lock, state.timeout, state.cancellationToken), (@lock, timeout, cancellationToken));
	}

	public static THandle? TryAcquire<THandle>(IInternalDistributedLock<THandle> @lock, TimeSpan timeout, CancellationToken cancellationToken) where THandle : class, IDistributedSynchronizationHandle
	{
		return SyncViaAsync.Run(((IInternalDistributedLock<THandle> @lock, TimeSpan timeout, CancellationToken cancellationToken) state) => state.@lock.InternalTryAcquireAsync(state.timeout, state.cancellationToken), (@lock, timeout, cancellationToken));
	}

	public static ValueTask<THandle> AcquireAsync<THandle>(IInternalDistributedReaderWriterLock<THandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken, bool isWrite) where THandle : class, IDistributedSynchronizationHandle
	{
		return @lock.InternalTryAcquireAsync(timeout, cancellationToken, isWrite).ThrowTimeoutIfNull();
	}

	public static THandle Acquire<THandle>(IInternalDistributedReaderWriterLock<THandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken, bool isWrite) where THandle : class, IDistributedSynchronizationHandle
	{
		return SyncViaAsync.Run(((IInternalDistributedReaderWriterLock<THandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken, bool isWrite) state) => AcquireAsync(state.@lock, state.timeout, state.cancellationToken, state.isWrite), (@lock, timeout, cancellationToken, isWrite));
	}

	public static THandle? TryAcquire<THandle>(IInternalDistributedReaderWriterLock<THandle> @lock, TimeSpan timeout, CancellationToken cancellationToken, bool isWrite) where THandle : class, IDistributedSynchronizationHandle
	{
		return SyncViaAsync.Run(((IInternalDistributedReaderWriterLock<THandle> @lock, TimeSpan timeout, CancellationToken cancellationToken, bool isWrite) state) => state.@lock.InternalTryAcquireAsync(state.timeout, state.cancellationToken, state.isWrite), (@lock, timeout, cancellationToken, isWrite));
	}

	public static ValueTask<TUpgradeableHandle> AcquireUpgradeableReadLockAsync<THandle, TUpgradeableHandle>(IInternalDistributedUpgradeableReaderWriterLock<THandle, TUpgradeableHandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken) where THandle : class, IDistributedSynchronizationHandle where TUpgradeableHandle : class, IDistributedLockUpgradeableHandle
	{
		return @lock.InternalTryAcquireUpgradeableReadLockAsync(timeout, cancellationToken).ThrowTimeoutIfNull();
	}

	public static TUpgradeableHandle AcquireUpgradeableReadLock<THandle, TUpgradeableHandle>(IInternalDistributedUpgradeableReaderWriterLock<THandle, TUpgradeableHandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken) where THandle : class, IDistributedSynchronizationHandle where TUpgradeableHandle : class, IDistributedLockUpgradeableHandle
	{
		return SyncViaAsync.Run(((IInternalDistributedUpgradeableReaderWriterLock<THandle, TUpgradeableHandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken) state) => AcquireUpgradeableReadLockAsync(state.@lock, state.timeout, state.cancellationToken), (@lock, timeout, cancellationToken));
	}

	public static TUpgradeableHandle? TryAcquireUpgradeableReadLock<THandle, TUpgradeableHandle>(IInternalDistributedUpgradeableReaderWriterLock<THandle, TUpgradeableHandle> @lock, TimeSpan timeout, CancellationToken cancellationToken) where THandle : class, IDistributedSynchronizationHandle where TUpgradeableHandle : class, IDistributedLockUpgradeableHandle
	{
		return SyncViaAsync.Run(((IInternalDistributedUpgradeableReaderWriterLock<THandle, TUpgradeableHandle> @lock, TimeSpan timeout, CancellationToken cancellationToken) state) => state.@lock.InternalTryAcquireUpgradeableReadLockAsync(state.timeout, state.cancellationToken), (@lock, timeout, cancellationToken));
	}

	public static ValueTask<THandle> AcquireAsync<THandle>(IInternalDistributedSemaphore<THandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken) where THandle : class, IDistributedSynchronizationHandle
	{
		return @lock.InternalTryAcquireAsync(timeout, cancellationToken).ThrowTimeoutIfNull("semaphore");
	}

	public static THandle Acquire<THandle>(IInternalDistributedSemaphore<THandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken) where THandle : class, IDistributedSynchronizationHandle
	{
		return SyncViaAsync.Run(((IInternalDistributedSemaphore<THandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken) state) => AcquireAsync(state.@lock, state.timeout, state.cancellationToken), (@lock, timeout, cancellationToken));
	}

	public static THandle? TryAcquire<THandle>(IInternalDistributedSemaphore<THandle> @lock, TimeSpan timeout, CancellationToken cancellationToken) where THandle : class, IDistributedSynchronizationHandle
	{
		return SyncViaAsync.Run(((IInternalDistributedSemaphore<THandle> @lock, TimeSpan timeout, CancellationToken cancellationToken) state) => state.@lock.InternalTryAcquireAsync(state.timeout, state.cancellationToken), (@lock, timeout, cancellationToken));
	}

	public static ValueTask UpgradeToWriteLockAsync(IInternalDistributedLockUpgradeableHandle handle, TimeSpan? timeout, CancellationToken cancellationToken)
	{
		return handle.InternalTryUpgradeToWriteLockAsync(timeout, cancellationToken).ThrowTimeoutIfFalse();
	}

	public static void UpgradeToWriteLock(IDistributedLockUpgradeableHandle handle, TimeSpan? timeout, CancellationToken cancellationToken)
	{
		SyncViaAsync.Run(((IDistributedLockUpgradeableHandle handle, TimeSpan? timeout, CancellationToken cancellationToken) t) => t.handle.UpgradeToWriteLockAsync(t.timeout, t.cancellationToken), (handle, timeout, cancellationToken));
	}

	public static bool TryUpgradeToWriteLock(IDistributedLockUpgradeableHandle handle, TimeSpan timeout, CancellationToken cancellationToken)
	{
		return SyncViaAsync.Run(((IDistributedLockUpgradeableHandle handle, TimeSpan timeout, CancellationToken cancellationToken) t) => t.handle.TryUpgradeToWriteLockAsync(t.timeout, t.cancellationToken), (handle, timeout, cancellationToken));
	}

	private static Exception LockTimeout(string? @object = null)
	{
		return new TimeoutException("Timeout exceeded when trying to acquire the " + (@object ?? "lock"));
	}

	public static async ValueTask<T> ThrowTimeoutIfNull<T>(this ValueTask<T?> task, string? @object = null) where T : class
	{
		return (await task.ConfigureAwait(continueOnCapturedContext: false)) ?? throw LockTimeout(@object);
	}

	private static async ValueTask ThrowTimeoutIfFalse(this ValueTask<bool> task)
	{
		if (!(await task.ConfigureAwait(continueOnCapturedContext: false)))
		{
			throw LockTimeout();
		}
	}
}
