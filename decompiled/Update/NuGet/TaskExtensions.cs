using System;
using System.Threading;
using System.Threading.Tasks;

namespace NuGet;

internal static class TaskExtensions
{
	public static TResult WhenAny<TResult>(this Task<TResult>[] tasks, Predicate<TResult> predicate)
	{
		int numTasksRemaining = tasks.Length;
		TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
		for (int i = 0; i < tasks.Length; i++)
		{
			tasks[i].ContinueWith(delegate(Task<TResult> innerTask)
			{
				if (innerTask.Status == TaskStatus.RanToCompletion && predicate(innerTask.Result))
				{
					tcs.TrySetResult(innerTask.Result);
				}
				if (Interlocked.Decrement(ref numTasksRemaining) == 0)
				{
					tcs.TrySetResult(default(TResult));
				}
			}, TaskContinuationOptions.ExecuteSynchronously);
		}
		return tcs.Task.Result;
	}
}
