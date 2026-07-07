using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading;
using NamedPipeWrapper.IO;
using NamedPipeWrapper.Threading;

namespace NamedPipeWrapper;

public class NamedPipeConnection<TRead, TWrite> where TRead : class where TWrite : class
{
	public readonly int Id;

	public readonly string Name;

	private readonly PipeStreamWrapper<TRead, TWrite> _streamWrapper;

	private readonly AutoResetEvent _writeSignal = new AutoResetEvent(initialState: false);

	private readonly Queue<TWrite> _writeQueue = new Queue<TWrite>();

	private bool _notifiedSucceeded;

	public bool IsConnected => _streamWrapper.IsConnected;

	public event ConnectionEventHandler<TRead, TWrite> Disconnected;

	public event ConnectionMessageEventHandler<TRead, TWrite> ReceiveMessage;

	public event ConnectionExceptionEventHandler<TRead, TWrite> Error;

	internal NamedPipeConnection(int id, string name, PipeStream serverStream)
	{
		Id = id;
		Name = name;
		_streamWrapper = new PipeStreamWrapper<TRead, TWrite>(serverStream);
	}

	public void Open()
	{
		Worker worker = new Worker();
		worker.Succeeded += OnSucceeded;
		worker.Error += OnError;
		worker.DoWork(ReadPipe);
		Worker worker2 = new Worker();
		worker2.Succeeded += OnSucceeded;
		worker2.Error += OnError;
		worker2.DoWork(WritePipe);
	}

	public void PushMessage(TWrite message)
	{
		_writeQueue.Enqueue(message);
		_writeSignal.Set();
	}

	public void Close()
	{
		CloseImpl();
	}

	private void CloseImpl()
	{
		_streamWrapper.Close();
		_writeSignal.Set();
	}

	private void OnSucceeded()
	{
		if (!_notifiedSucceeded)
		{
			_notifiedSucceeded = true;
			if (this.Disconnected != null)
			{
				this.Disconnected(this);
			}
		}
	}

	private void OnError(Exception exception)
	{
		if (this.Error != null)
		{
			this.Error(this, exception);
		}
	}

	private void ReadPipe()
	{
		while (IsConnected && _streamWrapper.CanRead)
		{
			TRead val = _streamWrapper.ReadObject();
			if (val == null)
			{
				CloseImpl();
				break;
			}
			if (this.ReceiveMessage != null)
			{
				this.ReceiveMessage(this, val);
			}
		}
	}

	private void WritePipe()
	{
		while (IsConnected && _streamWrapper.CanWrite)
		{
			_writeSignal.WaitOne();
			while (_writeQueue.Count > 0)
			{
				_streamWrapper.WriteObject(_writeQueue.Dequeue());
				_streamWrapper.WaitForPipeDrain();
			}
		}
	}
}
