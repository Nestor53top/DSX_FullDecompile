using System;
using System.Collections.Generic;
using System.IO.Pipes;
using NamedPipeWrapper.IO;
using NamedPipeWrapper.Threading;

namespace NamedPipeWrapper;

public class Server<TRead, TWrite> where TRead : class where TWrite : class
{
	private readonly string _pipeName;

	public readonly PipeSecurity _pipeSecurity;

	public readonly List<NamedPipeConnection<TRead, TWrite>> _connections = new List<NamedPipeConnection<TRead, TWrite>>();

	private int _nextPipeId;

	private volatile bool _shouldKeepRunning;

	private volatile bool _isRunning;

	public event ConnectionEventHandler<TRead, TWrite> ClientConnected;

	public event ConnectionEventHandler<TRead, TWrite> ClientDisconnected;

	public event ConnectionMessageEventHandler<TRead, TWrite> ClientMessage;

	public event PipeExceptionEventHandler Error;

	public Server(string pipeName, PipeSecurity pipeSecurity)
	{
		_pipeName = pipeName;
		_pipeSecurity = pipeSecurity;
	}

	public void Start()
	{
		_shouldKeepRunning = true;
		Worker worker = new Worker();
		worker.Error += OnError;
		worker.DoWork(ListenSync);
	}

	public void PushMessage(TWrite message)
	{
		lock (_connections)
		{
			foreach (NamedPipeConnection<TRead, TWrite> connection in _connections)
			{
				connection.PushMessage(message);
			}
		}
	}

	public void Stop()
	{
		_shouldKeepRunning = false;
		lock (_connections)
		{
			NamedPipeConnection<TRead, TWrite>[] array = _connections.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Close();
			}
		}
		NamedPipeClient<TRead, TWrite> namedPipeClient = new NamedPipeClient<TRead, TWrite>(_pipeName);
		namedPipeClient.Start();
		namedPipeClient.WaitForConnection(TimeSpan.FromSeconds(2.0));
		namedPipeClient.Stop();
		namedPipeClient.WaitForDisconnection(TimeSpan.FromSeconds(2.0));
	}

	private void ListenSync()
	{
		_isRunning = true;
		while (_shouldKeepRunning)
		{
			WaitForConnection(_pipeName, _pipeSecurity);
		}
		_isRunning = false;
	}

	private void WaitForConnection(string pipeName, PipeSecurity pipeSecurity)
	{
		NamedPipeServerStream namedPipeServerStream = null;
		NamedPipeServerStream namedPipeServerStream2 = null;
		NamedPipeConnection<TRead, TWrite> namedPipeConnection = null;
		string nextConnectionPipeName = GetNextConnectionPipeName(pipeName);
		try
		{
			namedPipeServerStream = PipeServerFactory.CreateAndConnectPipe(pipeName, pipeSecurity);
			PipeStreamWrapper<string, string> pipeStreamWrapper = new PipeStreamWrapper<string, string>(namedPipeServerStream);
			pipeStreamWrapper.WriteObject(nextConnectionPipeName);
			pipeStreamWrapper.WaitForPipeDrain();
			pipeStreamWrapper.Close();
			namedPipeServerStream2 = PipeServerFactory.CreatePipe(nextConnectionPipeName, pipeSecurity);
			namedPipeServerStream2.WaitForConnection();
			namedPipeConnection = ConnectionFactory.CreateConnection<TRead, TWrite>(namedPipeServerStream2);
			namedPipeConnection.ReceiveMessage += ClientOnReceiveMessage;
			namedPipeConnection.Disconnected += ClientOnDisconnected;
			namedPipeConnection.Error += ConnectionOnError;
			namedPipeConnection.Open();
			lock (_connections)
			{
				_connections.Add(namedPipeConnection);
			}
			ClientOnConnected(namedPipeConnection);
		}
		catch (Exception arg)
		{
			Console.Error.WriteLine("Named pipe is broken or disconnected: {0}", arg);
			Cleanup(namedPipeServerStream);
			Cleanup(namedPipeServerStream2);
			ClientOnDisconnected(namedPipeConnection);
		}
	}

	private void ClientOnConnected(NamedPipeConnection<TRead, TWrite> connection)
	{
		if (this.ClientConnected != null)
		{
			this.ClientConnected(connection);
		}
	}

	private void ClientOnReceiveMessage(NamedPipeConnection<TRead, TWrite> connection, TRead message)
	{
		if (this.ClientMessage != null)
		{
			this.ClientMessage(connection, message);
		}
	}

	private void ClientOnDisconnected(NamedPipeConnection<TRead, TWrite> connection)
	{
		if (connection != null)
		{
			lock (_connections)
			{
				_connections.Remove(connection);
			}
			if (this.ClientDisconnected != null)
			{
				this.ClientDisconnected(connection);
			}
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

	private string GetNextConnectionPipeName(string pipeName)
	{
		return $"{pipeName}_{++_nextPipeId}";
	}

	private static void Cleanup(NamedPipeServerStream pipe)
	{
		if (pipe == null)
		{
			return;
		}
		using NamedPipeServerStream namedPipeServerStream = pipe;
		namedPipeServerStream.Close();
	}
}
