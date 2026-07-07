using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal.Data;

internal sealed class DedicatedConnectionOrTransactionDbDistributedLock : IDbDistributedLock
{
	private sealed class Handle<TLockCookie> : IDistributedSynchronizationHandle, IDisposable, IAsyncDisposable where TLockCookie : class
	{
		private sealed class InnerHandle : IAsyncDisposable
		{
			private static readonly object DisposedSentinel = new object();

			private readonly IDbSynchronizationStrategy<TLockCookie> _strategy;

			private readonly string _name;

			private readonly TLockCookie _lockCookie;

			private readonly bool _transactionScoped;

			private readonly IAsyncDisposable? _connectionResource;

			private object? _connectionMonitoringHandleOrDisposedSentinel;

			public DatabaseConnection Connection { get; }

			public CancellationToken HandleLostToken
			{
				get
				{
					object obj = Volatile.Read(in _connectionMonitoringHandleOrDisposedSentinel);
					if (obj == null)
					{
						IDatabaseConnectionMonitoringHandle connectionMonitoringHandle = Connection.GetConnectionMonitoringHandle();
						obj = Interlocked.CompareExchange(ref _connectionMonitoringHandleOrDisposedSentinel, connectionMonitoringHandle, null);
						if (obj == null)
						{
							return connectionMonitoringHandle.ConnectionLostToken;
						}
						connectionMonitoringHandle.Dispose();
					}
					if (obj == DisposedSentinel)
					{
						throw this.ObjectDisposed();
					}
					return ((IDatabaseConnectionMonitoringHandle)obj).ConnectionLostToken;
				}
			}

			public InnerHandle(DatabaseConnection connection, IDbSynchronizationStrategy<TLockCookie> strategy, string name, TLockCookie lockCookie, bool transactionScoped, IAsyncDisposable? connectionResource)
			{
				Connection = connection;
				_strategy = strategy;
				_name = name;
				_lockCookie = lockCookie;
				_transactionScoped = transactionScoped;
				_connectionResource = connectionResource;
			}

			public async ValueTask DisposeAsync()
			{
				object obj = Interlocked.Exchange(ref _connectionMonitoringHandleOrDisposedSentinel, DisposedSentinel);
				if (obj == DisposedSentinel)
				{
					return;
				}
				if (obj is IDatabaseConnectionMonitoringHandle databaseConnectionMonitoringHandle)
				{
					databaseConnectionMonitoringHandle.Dispose();
				}
				try
				{
					if (!_transactionScoped || (Connection.IsExernallyOwned && Connection.CanExecuteQueries))
					{
						await _strategy.ReleaseAsync(Connection, _name, _lockCookie).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				finally
				{
					await (_connectionResource?.DisposeAsync() ?? default(ValueTask)).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}

		private InnerHandle? _innerHandle;

		private IDisposable? _finalizer;

		public CancellationToken HandleLostToken => (Volatile.Read(in _innerHandle) ?? throw this.ObjectDisposed()).HandleLostToken;

		public DatabaseConnection? Connection => Volatile.Read(in _innerHandle)?.Connection;

		public Handle(DatabaseConnection connection, IDbSynchronizationStrategy<TLockCookie> strategy, string name, TLockCookie lockCookie, bool transactionScoped, IAsyncDisposable? connectionResource)
		{
			_innerHandle = new InnerHandle(connection, strategy, name, lockCookie, transactionScoped, connectionResource);
			_finalizer = (connection.IsExernallyOwned ? null : ManagedFinalizerQueue.Instance.Register(this, _innerHandle));
		}

		public void Dispose()
		{
			this.DisposeSyncViaAsync();
		}

		public ValueTask DisposeAsync()
		{
			Interlocked.Exchange(ref _finalizer, null)?.Dispose();
			return Interlocked.Exchange(ref _innerHandle, null)?.DisposeAsync() ?? default(ValueTask);
		}
	}

	private readonly string _name;

	private readonly Func<DatabaseConnection> _connectionFactory;

	private readonly bool _transactionScopedIfPossible;

	private readonly TimeoutValue _keepaliveCadence;

	public DedicatedConnectionOrTransactionDbDistributedLock(string name, Func<DatabaseConnection> externalConnectionFactory)
		: this(name, externalConnectionFactory, useTransaction: true, Timeout.InfiniteTimeSpan)
	{
	}

	public DedicatedConnectionOrTransactionDbDistributedLock(string name, Func<DatabaseConnection> connectionFactory, bool useTransaction, TimeoutValue keepaliveCadence)
	{
		_name = name;
		_connectionFactory = connectionFactory;
		_transactionScopedIfPossible = useTransaction;
		_keepaliveCadence = keepaliveCadence;
	}

	public async ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync<TLockCookie>(TimeoutValue timeout, IDbSynchronizationStrategy<TLockCookie> strategy, CancellationToken cancellationToken, IDistributedSynchronizationHandle? contextHandle) where TLockCookie : class
	{
		IDistributedSynchronizationHandle result = null;
		IAsyncDisposable connectionResource = null;
		try
		{
			DatabaseConnection connection;
			if (contextHandle != null)
			{
				connection = GetContextHandleConnection<TLockCookie>(contextHandle);
			}
			else
			{
				DatabaseConnection databaseConnection;
				connection = (databaseConnection = _connectionFactory());
				connectionResource = databaseConnection;
				if (connection.IsExernallyOwned)
				{
					if (!connection.CanExecuteQueries)
					{
						throw new InvalidOperationException("The connection and/or transaction are disposed or closed");
					}
				}
				else
				{
					await connection.OpenAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					if (_transactionScopedIfPossible)
					{
						await connection.BeginTransactionAsync().ConfigureAwait(continueOnCapturedContext: false);
					}
				}
			}
			TLockCookie val = await strategy.TryAcquireAsync(connection, _name, timeout, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (val != null)
			{
				result = new Handle<TLockCookie>(connection, strategy, _name, val, _transactionScopedIfPossible && connection.HasTransaction, connectionResource);
				if (!_keepaliveCadence.IsInfinite)
				{
					connection.SetKeepaliveCadence(_keepaliveCadence);
				}
			}
		}
		finally
		{
			if (result == null && connectionResource != null)
			{
				await connectionResource.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		return result;
	}

	private DatabaseConnection GetContextHandleConnection<TLockCookie>(IDistributedSynchronizationHandle contextHandle) where TLockCookie : class
	{
		return ((Handle<TLockCookie>)contextHandle).Connection ?? throw new ObjectDisposedException("contextHandle", "the provided handle is already disposed");
	}
}
