using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Ingestion.Http;

internal sealed class RetryableIngestion : IngestionDecorator
{
	private static readonly TimeSpan[] DefaultIntervals = new TimeSpan[3]
	{
		TimeSpan.FromSeconds(10.0),
		TimeSpan.FromSeconds(30.0),
		TimeSpan.FromMinutes(20.0)
	};

	private readonly IDictionary<ServiceCall, Timer> _calls = new Dictionary<ServiceCall, Timer>();

	private readonly TimeSpan[] _retryIntervals;

	public RetryableIngestion(IIngestion decoratedApi)
		: base(decoratedApi)
	{
		Random random = new Random();
		_retryIntervals = DefaultIntervals.Select(delegate(TimeSpan defaultInterval)
		{
			int num = (int)(defaultInterval.TotalMilliseconds / 2.0);
			num += random.Next(num);
			return TimeSpan.FromMilliseconds((double)num);
		}).ToArray();
	}

	public RetryableIngestion(IIngestion decoratedApi, TimeSpan[] retryIntervals)
		: base(decoratedApi)
	{
		_retryIntervals = retryIntervals ?? throw new ArgumentNullException("retryIntervals");
	}

	private Timer IntervalCall(int retry, Action action)
	{
		int num = (int)_retryIntervals[retry - 1].TotalMilliseconds;
		AppCenterLog.Warn(AppCenterLog.LogTag, $"Try #{retry} failed and will be retried in {num} ms");
		return new Timer(delegate
		{
			action();
		}, null, num, -1);
	}

	private void RetryCall(ServiceCall call, int retry)
	{
		if (call.IsCanceled)
		{
			return;
		}
		IServiceCall result = base.Call(call.AppSecret, call.InstallId, call.Logs);
		call.ContinueWith(delegate
		{
			if (call.IsCanceled && !result.IsCanceled)
			{
				result.Cancel();
			}
		});
		result.ContinueWith(delegate
		{
			RetryCallContinuation(call, result, retry + 1);
		});
	}

	private void RetryCallContinuation(ServiceCall call, IServiceCall result, int retry)
	{
		if (call.IsCanceled)
		{
			return;
		}
		if (result.IsCanceled)
		{
			call.Cancel();
		}
		else if (result.IsFaulted)
		{
			if (result.Exception is IngestionException { IsRecoverable: not false } && retry - 1 < _retryIntervals.Length)
			{
				Timer value = IntervalCall(retry, delegate
				{
					lock (_calls)
					{
						if (!_calls.Remove(call))
						{
							return;
						}
					}
					RetryCall(call, retry);
				});
				lock (_calls)
				{
					_calls.Add(call, value);
					return;
				}
			}
			call.SetException(result.Exception);
		}
		else
		{
			call.SetResult(result.Result);
		}
	}

	public override IServiceCall Call(string appSecret, Guid installId, IList<Log> logs)
	{
		ServiceCall serviceCall = new ServiceCall(appSecret, installId, logs);
		RetryCall(serviceCall, 0);
		return serviceCall;
	}

	public override void Close()
	{
		CancelAllCalls();
		base.Close();
	}

	public override void Dispose()
	{
		CancelAllCalls();
		base.Dispose();
	}

	private void CancelAllCalls()
	{
		IList<ServiceCall> list;
		lock (_calls)
		{
			if (_calls.Count == 0)
			{
				return;
			}
			list = _calls.Select((KeyValuePair<ServiceCall, Timer> i) => i.Key).ToList();
			foreach (Timer value in _calls.Values)
			{
				value.Dispose();
			}
			_calls.Clear();
		}
		foreach (ServiceCall item in list)
		{
			item.Cancel();
		}
	}
}
