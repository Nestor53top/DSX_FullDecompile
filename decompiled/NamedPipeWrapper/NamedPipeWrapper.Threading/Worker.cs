using System;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipeWrapper.Threading;

internal class Worker
{
	private readonly TaskScheduler _callbackThread;

	private static TaskScheduler CurrentTaskScheduler
	{
		get
		{
			if (SynchronizationContext.Current == null)
			{
				return TaskScheduler.Default;
			}
			return TaskScheduler.FromCurrentSynchronizationContext();
		}
	}

	public event WorkerSucceededEventHandler Succeeded;

	public event WorkerExceptionEventHandler Error;

	public Worker()
		: this(CurrentTaskScheduler)
	{
	}

	public Worker(TaskScheduler callbackThread)
	{
		_callbackThread = callbackThread;
	}

	public void DoWork(Action action)
	{
		new Task(DoWorkImpl, action, CancellationToken.None, TaskCreationOptions.LongRunning).Start();
	}

	private void DoWorkImpl(object oAction)
	{
		Action action = (Action)oAction;
		try
		{
			action();
			Callback(Succeed);
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			Exception e = ex2;
			Callback(delegate
			{
				Fail(e);
			});
		}
	}

	private void Succeed()
	{
		if (this.Succeeded != null)
		{
			this.Succeeded();
		}
	}

	private void Fail(Exception exception)
	{
		if (this.Error != null)
		{
			this.Error(exception);
		}
	}

	private void Callback(Action action)
	{
		Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, _callbackThread);
	}
}
