using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal;

internal sealed class ManagedFinalizerQueue
{
	private sealed class Registration : IDisposable
	{
		private readonly ManagedFinalizerQueue _queue;

		private IAsyncDisposable? _key;

		public Registration(ManagedFinalizerQueue queue, IAsyncDisposable key)
		{
			_queue = queue;
			_key = key;
		}

		public void Dispose()
		{
			IAsyncDisposable asyncDisposable = Interlocked.Exchange(ref _key, null);
			if (asyncDisposable != null)
			{
				_queue.TryRemove(asyncDisposable, disposeKey: false);
			}
		}
	}

	internal static readonly TimeSpan FinalizerCadence = TimeSpan.FromSeconds(30.0);

	public static readonly ManagedFinalizerQueue Instance = new ManagedFinalizerQueue();

	private readonly ConcurrentDictionary<IAsyncDisposable, WeakReference> _items = new ConcurrentDictionary<IAsyncDisposable, WeakReference>();

	private long _count;

	private Task _finalizerTask = Task.CompletedTask;

	private int _finalizerTaskIsInitializing;

	private ManagedFinalizerQueue()
	{
	}

	public IDisposable Register(object resource, IAsyncDisposable finalizer)
	{
		((IDictionary<IAsyncDisposable, WeakReference>)_items).As().Add(finalizer, new WeakReference(resource));
		if (Interlocked.Increment(ref _count) == 1)
		{
			StartFinalizerTask();
		}
		return new Registration(this, finalizer);
	}

	private void StartFinalizerTask()
	{
		if (Interlocked.Exchange(ref _finalizerTaskIsInitializing, 1) != 0)
		{
			return;
		}
		lock (_items)
		{
			_finalizerTask = _finalizerTask.ContinueWith((Task _, object @this) => ((ManagedFinalizerQueue)@this).FinalizerLoop(), this, CancellationToken.None).Unwrap();
		}
	}

	private async Task FinalizerLoop()
	{
		await Task.Delay(FinalizerCadence).ConfigureAwait(continueOnCapturedContext: false);
		Interlocked.Exchange(ref _finalizerTaskIsInitializing, 0);
		while (Volatile.Read(in _count) != 0L)
		{
			await FinalizeAsync(waitForItemFinalization: false).ConfigureAwait(continueOnCapturedContext: false);
			await Task.Delay(FinalizerCadence).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private Task FinalizeAsync(bool waitForItemFinalization)
	{
		List<Task> list = null;
		foreach (KeyValuePair<IAsyncDisposable, WeakReference> item2 in _items)
		{
			if (!item2.Value.IsAlive)
			{
				Task item = TryRemove(item2.Key, disposeKey: true);
				if (waitForItemFinalization)
				{
					(list ?? (list = new List<Task>())).Add(item);
				}
			}
		}
		if (!waitForItemFinalization)
		{
			return Task.CompletedTask;
		}
		IEnumerable<Task> enumerable = list;
		return Task.WhenAll(enumerable ?? Enumerable.Empty<Task>());
	}

	internal Task FinalizeAsync()
	{
		return FinalizeAsync(waitForItemFinalization: true);
	}

	private Task TryRemove(IAsyncDisposable key, bool disposeKey)
	{
		if (_items.TryRemove(key, out WeakReference _))
		{
			Interlocked.Decrement(ref _count);
			if (disposeKey)
			{
				return Task.Run(() => key.DisposeAsync().AsTask());
			}
		}
		return Task.CompletedTask;
	}
}
