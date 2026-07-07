using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal;

internal static class Helpers
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public readonly struct TaskConversion
	{
		public TaskConversion<TTo> To<TTo>()
		{
			throw new InvalidOperationException();
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public readonly struct TaskConversion<TTo>
	{
	}

	public readonly struct NonThrowingAwaitable<TTask>(TTask task) : ICriticalNotifyCompletion, INotifyCompletion where TTask : Task
	{
		private readonly TTask _task = task;

		private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _taskAwaiter = task.ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();

		public bool IsCompleted
		{
			get
			{
				ConfiguredTaskAwaitable.ConfiguredTaskAwaiter taskAwaiter = _taskAwaiter;
				return taskAwaiter.IsCompleted;
			}
		}

		public NonThrowingAwaitable<TTask> GetAwaiter()
		{
			return this;
		}

		public TTask GetResult()
		{
			return _task;
		}

		public void OnCompleted(Action continuation)
		{
			ConfiguredTaskAwaitable.ConfiguredTaskAwaiter taskAwaiter = _taskAwaiter;
			taskAwaiter.OnCompleted(continuation);
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			ConfiguredTaskAwaitable.ConfiguredTaskAwaiter taskAwaiter = _taskAwaiter;
			taskAwaiter.UnsafeOnCompleted(continuation);
		}
	}

	public static T As<T>(this T @this)
	{
		return @this;
	}

	public static async ValueTask<TBase> Convert<TDerived, TBase>(this ValueTask<TDerived> task, To<TBase>.ValueTaskConversion _) where TDerived : TBase
	{
		return (TBase)(object)(await task.ConfigureAwait(continueOnCapturedContext: false));
	}

	internal static async ValueTask ConvertToVoid<TResult>(this ValueTask<TResult> task)
	{
		await task.ConfigureAwait(continueOnCapturedContext: false);
	}

	public static ValueTask<T> AsValueTask<T>(this Task<T> task)
	{
		return new ValueTask<T>(task);
	}

	public static ValueTask AsValueTask(this Task task)
	{
		return new ValueTask(task);
	}

	public static ValueTask<T> AsValueTask<T>(this T value)
	{
		return new ValueTask<T>(value);
	}

	public static Task<TResult> SafeCreateTask<TState, TResult>(Func<TState, Task<TResult>> taskFactory, TState state)
	{
		return InternalSafeCreateTask<TState, Task<TResult>, TResult>(taskFactory, state);
	}

	public static Task SafeCreateTask<TState>(Func<TState, Task> taskFactory, TState state)
	{
		return InternalSafeCreateTask<TState, Task, bool>(taskFactory, state);
	}

	private static TTask InternalSafeCreateTask<TState, TTask, TResult>(Func<TState, TTask> taskFactory, TState state) where TTask : Task
	{
		try
		{
			return taskFactory(state);
		}
		catch (OperationCanceledException)
		{
			TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
			taskCompletionSource.SetCanceled();
			return (TTask)((object)taskCompletionSource.Task).As();
		}
		catch (Exception exception)
		{
			return (TTask)((object)Task.FromException<TResult>(exception)).As();
		}
	}

	public static ObjectDisposedException ObjectDisposed<T>(this T _) where T : IAsyncDisposable
	{
		throw new ObjectDisposedException(typeof(T).ToString());
	}

	public static NonThrowingAwaitable<TTask> TryAwait<TTask>(this TTask task) where TTask : Task
	{
		return new NonThrowingAwaitable<TTask>(task);
	}

	public static bool TryGetValue<T>(this T? nullable, out T value) where T : struct
	{
		value = nullable.GetValueOrDefault();
		return nullable.HasValue;
	}
}
