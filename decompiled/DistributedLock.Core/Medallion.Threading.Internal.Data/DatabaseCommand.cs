using System;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Medallion.Threading.Internal.Data;

internal sealed class DatabaseCommand : IDisposable
{
	private readonly IDbCommand _command;

	private readonly DatabaseConnection _connection;

	public IDataParameterCollection Parameters => _command.Parameters;

	internal DatabaseCommand(IDbCommand command, DatabaseConnection connection)
	{
		_command = command;
		_connection = connection;
	}

	public void SetCommandText(string sql)
	{
		_command.CommandText = sql;
	}

	public void SetTimeout(TimeoutValue operationTimeout)
	{
		_command.CommandTimeout = ((!operationTimeout.IsInfinite) ? (operationTimeout.InSeconds + 30) : 0);
	}

	public void SetCommandType(CommandType type)
	{
		_command.CommandType = type;
	}

	public IDbDataParameter AddParameter(string? name = null, object? value = null, DbType? type = null, ParameterDirection? direction = null)
	{
		IDbDataParameter dbDataParameter = _command.CreateParameter();
		if (name != null)
		{
			dbDataParameter.ParameterName = name;
		}
		if (value != null)
		{
			dbDataParameter.Value = value;
		}
		if (type.HasValue)
		{
			dbDataParameter.DbType = type.Value;
		}
		if (direction.HasValue)
		{
			dbDataParameter.Direction = direction.Value;
		}
		_command.Parameters.Add(dbDataParameter);
		return dbDataParameter;
	}

	public ValueTask<int> ExecuteNonQueryAsync(CancellationToken cancellationToken, bool disallowAsyncCancellation = false)
	{
		return ExecuteNonQueryAsync(cancellationToken, disallowAsyncCancellation, isConnectionMonitoringQuery: false);
	}

	internal ValueTask<int> ExecuteNonQueryAsync(CancellationToken cancellationToken, bool disallowAsyncCancellation, bool isConnectionMonitoringQuery)
	{
		return ExecuteAsync((DbCommand c, CancellationToken t) => c.ExecuteNonQueryAsync(t), (IDbCommand c) => c.ExecuteNonQuery(), cancellationToken, disallowAsyncCancellation, isConnectionMonitoringQuery);
	}

	public ValueTask<object> ExecuteScalarAsync(CancellationToken cancellationToken, bool disallowAsyncCancellation = false)
	{
		return ExecuteAsync((DbCommand c, CancellationToken t) => c.ExecuteScalarAsync(t), (IDbCommand c) => c.ExecuteScalar(), cancellationToken, disallowAsyncCancellation, isConnectionMonitoringQuery: false);
	}

	private async ValueTask<TResult> ExecuteAsync<TResult>(Func<DbCommand, CancellationToken, Task<TResult>> executeAsync, Func<IDbCommand, TResult> executeSync, CancellationToken cancellationToken, bool disallowAsyncCancellation, bool isConnectionMonitoringQuery)
	{
		if (!SyncViaAsync.IsSynchronous)
		{
			IDbCommand command = _command;
			if (command is DbCommand dbCommand)
			{
				if (!cancellationToken.CanBeCanceled)
				{
					using (await AcquireConnectionLockIfNeeded(isConnectionMonitoringQuery).ConfigureAwait(continueOnCapturedContext: false))
					{
						await PrepareIfNeededAsync(CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
						return await executeAsync(dbCommand, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				if (!disallowAsyncCancellation)
				{
					return await InternalExecuteAndPropagateCancellationAsync((dbCommand, executeAsync), ((DbCommand dbCommand, Func<DbCommand, CancellationToken, Task<TResult>> executeAsync) state, CancellationToken arg) => state.executeAsync(state.dbCommand, arg).AsValueTask(), cancellationToken, isConnectionMonitoringQuery).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}
		if (cancellationToken.CanBeCanceled)
		{
			cancellationToken.ThrowIfCancellationRequested();
			StrongBox<IDbCommand?> commandBox = new StrongBox<IDbCommand>(_command);
			using (cancellationToken.Register(delegate(object state)
			{
				Task.Run(async delegate
				{
					StrongBox<IDbCommand?> commandBox2 = (StrongBox<IDbCommand>)state;
					IDbCommand dbCommand2;
					while ((dbCommand2 = Volatile.Read(in commandBox2.Value)) != null)
					{
						try
						{
							dbCommand2.Cancel();
						}
						catch
						{
						}
						await Task.Delay(1).ConfigureAwait(continueOnCapturedContext: false);
					}
				});
			}, commandBox))
			{
				int num = 4;
				try
				{
					return await InternalExecuteAndPropagateCancellationAsync((_command, executeSync), ((IDbCommand command, Func<IDbCommand, TResult> executeSync) state, CancellationToken cancellationToken2) => state.executeSync(state.command).AsValueTask(), cancellationToken, isConnectionMonitoringQuery).ConfigureAwait(continueOnCapturedContext: false);
				}
				finally
				{
					Volatile.Write(ref commandBox.Value, null);
				}
			}
		}
		using (await AcquireConnectionLockIfNeeded(isConnectionMonitoringQuery).ConfigureAwait(continueOnCapturedContext: false))
		{
			return executeSync(_command);
		}
	}

	private async ValueTask<TResult> InternalExecuteAndPropagateCancellationAsync<TState, TResult>(TState state, Func<TState, CancellationToken, ValueTask<TResult>> executeAsync, CancellationToken cancellationToken, bool isConnectionMonitoringQuery)
	{
		using (await AcquireConnectionLockIfNeeded(isConnectionMonitoringQuery).ConfigureAwait(continueOnCapturedContext: false))
		{
			await PrepareIfNeededAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				return await executeAsync(state, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex) when (cancellationToken.IsCancellationRequested && _connection.IsCommandCancellationException(ex))
			{
				throw new OperationCanceledException("Command was canceled", ex, cancellationToken);
			}
		}
	}

	private ValueTask PrepareIfNeededAsync(CancellationToken cancellationToken)
	{
		if (_connection.ShouldPrepareCommands)
		{
			_command.Prepare();
		}
		return default(ValueTask);
	}

	public void Dispose()
	{
		_command.Dispose();
	}

	private ValueTask<IDisposable?> AcquireConnectionLockIfNeeded(bool isConnectionMonitoringQuery)
	{
		if (!isConnectionMonitoringQuery)
		{
			return _connection.ConnectionMonitor?.AcquireConnectionLockAsync(CancellationToken.None).Convert(To<IDisposable>.ValueTask) ?? ((IDisposable)null).AsValueTask();
		}
		return ((IDisposable)null).AsValueTask();
	}
}
