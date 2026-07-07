using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal.Data;

internal abstract class DatabaseConnection : IAsyncDisposable
{
	private IDbTransaction? _transaction;

	internal ConnectionMonitor ConnectionMonitor { get; }

	internal IDbConnection InnerConnection { get; }

	public bool HasTransaction => _transaction != null;

	public bool IsExernallyOwned { get; }

	public abstract bool ShouldPrepareCommands { get; }

	internal bool CanExecuteQueries
	{
		get
		{
			if (InnerConnection.State == ConnectionState.Open)
			{
				if (_transaction != null)
				{
					return _transaction.Connection != null;
				}
				return true;
			}
			return false;
		}
	}

	protected DatabaseConnection(IDbConnection connection, bool isExternallyOwned)
	{
		InnerConnection = connection;
		IsExernallyOwned = isExternallyOwned;
		ConnectionMonitor = new ConnectionMonitor(this);
	}

	protected DatabaseConnection(IDbTransaction transaction, bool isExternallyOwned)
		: this(transaction.Connection ?? throw new InvalidOperationException("Cannot execute queries against a transaction that has been disposed"), isExternallyOwned)
	{
		_transaction = transaction;
	}

	internal void SetKeepaliveCadence(TimeoutValue cadence)
	{
		ConnectionMonitor.SetKeepaliveCadence(cadence);
	}

	internal IDatabaseConnectionMonitoringHandle GetConnectionMonitoringHandle()
	{
		return ConnectionMonitor.GetMonitoringHandle();
	}

	public DatabaseCommand CreateCommand()
	{
		IDbCommand dbCommand = InnerConnection.CreateCommand();
		dbCommand.Transaction = _transaction;
		return new DatabaseCommand(dbCommand, this);
	}

	public async ValueTask BeginTransactionAsync()
	{
		using (await ConnectionMonitor.AcquireConnectionLockAsync(CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false))
		{
			_transaction = InnerConnection.BeginTransaction();
		}
	}

	public async ValueTask OpenAsync(CancellationToken cancellationToken)
	{
		if ((cancellationToken.CanBeCanceled || !SyncViaAsync.IsSynchronous) && InnerConnection is DbConnection dbConnection)
		{
			await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		else
		{
			cancellationToken.ThrowIfCancellationRequested();
			InnerConnection.Open();
		}
		ConnectionMonitor.Start();
	}

	public ValueTask CloseAsync()
	{
		return DisposeOrCloseAsync(isDispose: false);
	}

	public ValueTask DisposeAsync()
	{
		return DisposeOrCloseAsync(isDispose: true);
	}

	private async ValueTask DisposeOrCloseAsync(bool isDispose)
	{
		try
		{
			await (isDispose ? ConnectionMonitor.DisposeAsync() : ConnectionMonitor.StopAsync()).ConfigureAwait(continueOnCapturedContext: false);
		}
		finally
		{
			if (!IsExernallyOwned)
			{
				try
				{
					await DisposeTransactionAsync(isClosingOrDisposingConnection: true).ConfigureAwait(continueOnCapturedContext: false);
				}
				finally
				{
					SyncDisposeConnection();
				}
			}
		}
		void SyncDisposeConnection()
		{
			if (isDispose)
			{
				InnerConnection.Dispose();
			}
			else
			{
				InnerConnection.Close();
			}
		}
	}

	public ValueTask DisposeTransactionAsync()
	{
		return DisposeTransactionAsync(isClosingOrDisposingConnection: false);
	}

	private async ValueTask DisposeTransactionAsync(bool isClosingOrDisposingConnection)
	{
		IDbTransaction transaction = _transaction;
		if (transaction == null)
		{
			return;
		}
		_transaction = null;
		IDisposable disposable = ((!isClosingOrDisposingConnection) ? (await ConnectionMonitor.AcquireConnectionLockAsync(CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false)) : null);
		using (disposable)
		{
			transaction.Dispose();
		}
	}

	public abstract bool IsCommandCancellationException(Exception exception);

	public abstract Task SleepAsync(TimeSpan sleepTime, CancellationToken cancellationToken, Func<DatabaseCommand, CancellationToken, ValueTask<int>> executor);
}
