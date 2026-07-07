using System;
using System.IO.Pipes;
using System.Threading;
using NamedPipeWrapper.IO;
using NamedPipeWrapper.Threading;

namespace NamedPipeWrapper;

public class NamedPipeClient<TReadWrite> : NamedPipeClient<TReadWrite, TReadWrite> where TReadWrite : class
{
	public NamedPipeClient(string pipeName)
		: base(pipeName)
	{
	}
}
public class NamedPipeClient<TRead, TWrite> where TRead : class where TWrite : class
{
	private readonly string _pipeName;

	private NamedPipeConnection<TRead, TWrite> _connection;

	private readonly AutoResetEvent _connected = new AutoResetEvent(initialState: false);

	private readonly AutoResetEvent _disconnected = new AutoResetEvent(initialState: false);

	private volatile bool _closedExplicitly;

	public bool AutoReconnect { get; set; }

	public event ConnectionMessageEventHandler<TRead, TWrite> ServerMessage;

	public event ConnectionEventHandler<TRead, TWrite> Disconnected;

	public event PipeExceptionEventHandler Error;

	public NamedPipeClient(string pipeName)
	{
		_pipeName = pipeName;
		AutoReconnect = true;
	}

	public void Start()
	{
		_closedExplicitly = false;
		Worker worker = new Worker();
		worker.Error += OnError;
		worker.DoWork(ListenSync);
	}

	public void PushMessage(TWrite message)
	{
		if (_connection != null)
		{
			_connection.PushMessage(message);
		}
	}

	public void Stop()
	{
		_closedExplicitly = true;
		if (_connection != null)
		{
			_connection.Close();
		}
	}

	public void WaitForConnection()
	{
		_connected.WaitOne();
	}

	public void WaitForConnection(int millisecondsTimeout)
	{
		_connected.WaitOne(millisecondsTimeout);
	}

	public void WaitForConnection(TimeSpan timeout)
	{
		_connected.WaitOne(timeout);
	}

	public void WaitForDisconnection()
	{
		_disconnected.WaitOne();
	}

	public void WaitForDisconnection(int millisecondsTimeout)
	{
		_disconnected.WaitOne(millisecondsTimeout);
	}

	public void WaitForDisconnection(TimeSpan timeout)
	{
		_disconnected.WaitOne(timeout);
	}

	private void ListenSync()
	{
		PipeStreamWrapper<string, string> pipeStreamWrapper = PipeClientFactory.Connect<string, string>(_pipeName);
		string pipeName = pipeStreamWrapper.ReadObject();
		pipeStreamWrapper.Close();
		NamedPipeClientStream pipeStream = PipeClientFactory.CreateAndConnectPipe(pipeName);
		_connection = ConnectionFactory.CreateConnection<TRead, TWrite>(pipeStream);
		_connection.Disconnected += OnDisconnected;
		_connection.ReceiveMessage += OnReceiveMessage;
		_connection.Error += ConnectionOnError;
		_connection.Open();
		_connected.Set();
	}

	private void OnDisconnected(NamedPipeConnection<TRead, TWrite> connection)
	{
		if (this.Disconnected != null)
		{
			this.Disconnected(connection);
		}
		_disconnected.Set();
		if (AutoReconnect && !_closedExplicitly)
		{
			Start();
		}
	}

	private void OnReceiveMessage(NamedPipeConnection<TRead, TWrite> connection, TRead message)
	{
		if (this.ServerMessage != null)
		{
			this.ServerMessage(connection, message);
		}
	}

	private void ConnectionOnError(NamedPipeConnection<TRead, TWrite> connection, Exception exception)
	{
		OnError(exception);
	}

	private void OnError(Exception exception)
	{
		if (this.Error != null)
		{
			this.Error(exception);
		}
	}
}
