using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal;

internal sealed class LeaseMonitor : IDisposable, IAsyncDisposable
{
	public interface ILeaseHandle
	{
		TimeoutValue LeaseDuration { get; }

		TimeoutValue MonitoringCadence { get; }

		Task<LeaseState> RenewOrValidateLeaseAsync(CancellationToken cancellationToken);
	}

	public enum LeaseState
	{
		Held,
		Renewed,
		Lost,
		Unknown
	}

	private readonly CancellationTokenSource _disposalSource = new CancellationTokenSource();

	private readonly CancellationTokenSource _handleLostSource = new CancellationTokenSource();

	private readonly ILeaseHandle _leaseHandle;

	private readonly Task _monitoringTask;

	private Task? _cancellationTask;

	public CancellationToken HandleLostToken => _handleLostSource.Token;

	public LeaseMonitor(ILeaseHandle leaseHandle)
	{
		_leaseHandle = leaseHandle;
		_monitoringTask = CreateMonitoringLoopTask(new WeakReference<LeaseMonitor>(this), leaseHandle.MonitoringCadence, _disposalSource.Token);
	}

	public void Dispose()
	{
		this.DisposeSyncViaAsync();
	}

	public async ValueTask DisposeAsync()
	{
		try
		{
			if (!_disposalSource.IsCancellationRequested)
			{
				_disposalSource.Cancel();
			}
			await _monitoringTask.AwaitSyncOverAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
		finally
		{
			if (_cancellationTask != null)
			{
				_cancellationTask.ContinueWith(delegate(Task _, object state)
				{
					((CancellationTokenSource)state).Dispose();
				}, _handleLostSource);
			}
			else
			{
				_handleLostSource.Dispose();
			}
			_disposalSource.Dispose();
		}
	}

	private static Task CreateMonitoringLoopTask(WeakReference<LeaseMonitor> weakMonitor, TimeoutValue monitoringCadence, CancellationToken disposalToken)
	{
		return Task.Run(() => MonitoringLoop());
		async Task MonitoringLoop()
		{
			Stopwatch leaseLifetime = Stopwatch.StartNew();
			bool flag;
			do
			{
				await Task.Delay(monitoringCadence.InMilliseconds, disposalToken).TryAwait();
				flag = !disposalToken.IsCancellationRequested;
				if (flag)
				{
					flag = await RunMonitoringLoopIterationAsync(weakMonitor, leaseLifetime).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			while (flag);
		}
	}

	private static async Task<bool> RunMonitoringLoopIterationAsync(WeakReference<LeaseMonitor> weakMonitor, Stopwatch leaseLifetime)
	{
		if (!weakMonitor.TryGetTarget(out LeaseMonitor monitor))
		{
			return false;
		}
		if (monitor._leaseHandle.LeaseDuration.CompareTo(leaseLifetime.Elapsed) < 0)
		{
			OnHandleLost();
			return false;
		}
		switch (await monitor.CheckLeaseAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
		case LeaseState.Lost:
			OnHandleLost();
			return false;
		case LeaseState.Renewed:
			leaseLifetime.Restart();
			return true;
		case LeaseState.Held:
		case LeaseState.Unknown:
			return true;
		default:
			throw new InvalidOperationException("should never get here");
		}
		void OnHandleLost()
		{
			monitor._cancellationTask = Task.Run(delegate
			{
				monitor._handleLostSource.Cancel();
			});
		}
	}

	private async Task<LeaseState> CheckLeaseAsync()
	{
		Task<LeaseState> renewOrValidateTask = Helpers.SafeCreateTask(((ILeaseHandle leaseHandle, CancellationToken Token) state) => state.leaseHandle.RenewOrValidateLeaseAsync(state.Token), (_leaseHandle, _disposalSource.Token));
		await renewOrValidateTask.TryAwait();
		return (_disposalSource.IsCancellationRequested || renewOrValidateTask.Status != TaskStatus.RanToCompletion) ? LeaseState.Unknown : renewOrValidateTask.Result;
	}
}
