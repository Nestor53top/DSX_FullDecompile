using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Ingestion;

internal class ServiceCall : IServiceCall, IDisposable
{
	private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

	private Action<IServiceCall> _continuationAction;

	private readonly object _lock = new object();

	private bool _disposed;

	public bool IsCanceled => CancellationToken.IsCancellationRequested;

	public bool IsCompleted { get; private set; }

	public bool IsFaulted => Exception != null;

	public string Result { get; private set; }

	public Exception Exception { get; private set; }

	public CancellationToken CancellationToken => _tokenSource.Token;

	public string AppSecret { get; }

	public Guid InstallId { get; }

	public IList<Log> Logs { get; }

	public ServiceCall()
	{
	}

	public ServiceCall(string appSecret, Guid installId, IList<Log> logs)
	{
		AppSecret = appSecret;
		InstallId = installId;
		Logs = logs;
	}

	public void ContinueWith(Action<IServiceCall> continuationAction)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("ServiceCall");
		}
		lock (_lock)
		{
			if (!IsCompleted && !IsCanceled)
			{
				_continuationAction = (Action<IServiceCall>)Delegate.Combine(_continuationAction, continuationAction);
				return;
			}
		}
		continuationAction(this);
	}

	public void CopyState(IServiceCall source)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("ServiceCall");
		}
		if (source.IsCanceled)
		{
			Cancel();
		}
		else if (source.IsFaulted)
		{
			SetException(source.Exception);
		}
		else
		{
			SetResult(source.Result);
		}
	}

	public void SetResult(string result)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("ServiceCall");
		}
		Action<IServiceCall> continuationAction;
		lock (_lock)
		{
			if (IsCompleted || IsCanceled)
			{
				return;
			}
			IsCompleted = true;
			Result = result;
			continuationAction = _continuationAction;
			_continuationAction = null;
		}
		continuationAction?.Invoke(this);
	}

	public void SetException(Exception exception)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("ServiceCall");
		}
		Action<IServiceCall> continuationAction;
		lock (_lock)
		{
			if (IsCompleted || IsCanceled)
			{
				return;
			}
			IsCompleted = true;
			Exception = exception;
			continuationAction = _continuationAction;
			_continuationAction = null;
		}
		continuationAction?.Invoke(this);
	}

	public void Cancel()
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("ServiceCall");
		}
		Action<IServiceCall> continuationAction;
		lock (_lock)
		{
			if (IsCompleted || IsCanceled)
			{
				return;
			}
			_tokenSource.Cancel();
			continuationAction = _continuationAction;
			_continuationAction = null;
		}
		continuationAction?.Invoke(this);
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
			_tokenSource.Dispose();
		}
	}
}
