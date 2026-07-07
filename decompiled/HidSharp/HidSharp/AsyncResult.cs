using System;
using System.Threading;

namespace HidSharp;

internal sealed class AsyncResult<T> : IAsyncResult
{
	internal delegate T OperationCallback();

	private volatile bool _isCompleted;

	private ManualResetEvent _waitHandle;

	public AsyncCallback AsyncCallback { get; private set; }

	public object AsyncState { get; private set; }

	public WaitHandle AsyncWaitHandle
	{
		get
		{
			lock (this)
			{
				if (_waitHandle == null)
				{
					_waitHandle = new ManualResetEvent(_isCompleted);
				}
			}
			return _waitHandle;
		}
	}

	public bool CompletedSynchronously => false;

	public bool IsCompleted => _isCompleted;

	private Exception Exception { get; set; }

	private T Result { get; set; }

	private AsyncResult(AsyncCallback callback, object state)
	{
		AsyncCallback = callback;
		AsyncState = state;
	}

	private void Complete()
	{
		lock (this)
		{
			if (_isCompleted)
			{
				return;
			}
			_isCompleted = true;
			if (_waitHandle != null)
			{
				_waitHandle.Set();
			}
		}
		if (AsyncCallback != null)
		{
			AsyncCallback(this);
		}
	}

	internal static IAsyncResult BeginOperation(OperationCallback operation, AsyncCallback callback, object state)
	{
		AsyncResult<T> ar = new AsyncResult<T>(callback, state);
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				ar.Result = operation();
			}
			catch (Exception exception)
			{
				ar.Exception = exception;
			}
			ar.Complete();
		}, ar);
		return ar;
	}

	internal T EndOperation()
	{
		while (!IsCompleted)
		{
			AsyncWaitHandle.WaitOne();
		}
		if (Exception != null)
		{
			throw Exception;
		}
		return Result;
	}

	internal static T EndOperation(IAsyncResult asyncResult)
	{
		Throw.If.Null(asyncResult);
		return ((AsyncResult<T>)asyncResult).EndOperation();
	}
}
