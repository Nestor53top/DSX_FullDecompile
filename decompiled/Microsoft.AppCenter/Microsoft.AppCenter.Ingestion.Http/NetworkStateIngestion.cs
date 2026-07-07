using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Ingestion.Http;

internal sealed class NetworkStateIngestion : IngestionDecorator
{
	private readonly ISet<ServiceCall> _calls = new HashSet<ServiceCall>();

	private readonly INetworkStateAdapter _networkStateAdapter;

	public NetworkStateIngestion(IIngestion decoratedApi, INetworkStateAdapter networkStateAdapter)
		: base(decoratedApi)
	{
		_networkStateAdapter = networkStateAdapter;
		_networkStateAdapter.NetworkStatusChanged += OnNetworkStateChange;
	}

	private void OnNetworkStateChange(object sender, EventArgs e)
	{
		if (!_networkStateAdapter.IsConnected)
		{
			return;
		}
		List<ServiceCall> list = new List<ServiceCall>();
		lock (_calls)
		{
			list.AddRange(_calls);
			_calls.Clear();
		}
		foreach (ServiceCall item in list)
		{
			RetryCall(item);
		}
	}

	private void RetryCall(ServiceCall call)
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
		result.ContinueWith(call.CopyState);
	}

	public override IServiceCall Call(string appSecret, Guid installId, IList<Log> logs)
	{
		if (_networkStateAdapter.IsConnected)
		{
			return base.Call(appSecret, installId, logs);
		}
		ServiceCall serviceCall = new ServiceCall(appSecret, installId, logs);
		lock (_calls)
		{
			_calls.Add(serviceCall);
			return serviceCall;
		}
	}

	public override void Close()
	{
		CancelAllCalls();
		base.Close();
	}

	public override void Dispose()
	{
		_networkStateAdapter.NetworkStatusChanged -= OnNetworkStateChange;
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
			list = new List<ServiceCall>(_calls);
			_calls.Clear();
		}
		foreach (ServiceCall item in list)
		{
			item.Cancel();
		}
	}
}
