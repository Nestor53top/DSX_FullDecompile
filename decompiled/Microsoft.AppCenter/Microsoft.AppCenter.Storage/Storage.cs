using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Utils.Files;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Storage;

internal sealed class Storage : IStorage, IDisposable
{
	internal class LogEntry
	{
		public long Id { get; set; }

		public string Channel { get; set; }

		public string Log { get; set; }
	}

	private const string TableName = "LogEntry";

	private const string ColumnChannelName = "Channel";

	private const string ColumnLogName = "Log";

	private const string ColumnIdName = "Id";

	private const string DbIdentifierDelimiter = "@";

	private readonly IStorageAdapter _storageAdapter;

	private readonly string _databasePath;

	private readonly Dictionary<string, IList<long>> _pendingDbIdentifierGroups = new Dictionary<string, IList<long>>();

	private readonly HashSet<long> _pendingDbIdentifiers = new HashSet<long>();

	private readonly BlockingCollection<Task> _queue = new BlockingCollection<Task>();

	private readonly SemaphoreSlim _flushSemaphore = new SemaphoreSlim(0);

	private readonly Task _queueFlushTask;

	public Storage()
		: this(DefaultAdapter(), Constants.AppCenterDatabasePath)
	{
	}

	internal Storage(IStorageAdapter adapter, string databasePath)
	{
		AppCenterLog.Debug(AppCenterLog.LogTag, "Creating database at: " + databasePath);
		_storageAdapter = adapter;
		_databasePath = databasePath;
		_queue.Add(new Task(InitializeDatabase));
		_queueFlushTask = Task.Run((Func<Task?>)FlushQueueAsync);
	}

	private static IStorageAdapter DefaultAdapter()
	{
		return new StorageAdapter();
	}

	public Task PutLog(string channelName, Log log)
	{
		return AddTaskToQueue(delegate
		{
			string text = LogSerializer.Serialize(log);
			long maxStorageSize = _storageAdapter.GetMaxStorageSize();
			int num = Encoding.UTF8.GetBytes(text).Length;
			if (maxStorageSize < 0)
			{
				throw new StorageException("Failed to store a log to the database.");
			}
			if (maxStorageSize <= num)
			{
				throw new StorageException($"Log is too large ({num} bytes) to store in database. " + $"Current maximum database size is {maxStorageSize} bytes.");
			}
			while (true)
			{
				try
				{
					_storageAdapter.Insert("LogEntry", new string[2] { "Channel", "Log" }, new List<object[]> { new object[2] { channelName, text } });
					break;
				}
				catch (StorageFullException)
				{
					IList<object[]> list = _storageAdapter.Select("LogEntry", "Channel", channelName, string.Empty, null, 1, new string[1] { "Id" });
					if (list == null || list.Count <= 0 || list[0].Length == 0)
					{
						throw new StorageException("Failed to add a new log. Storage is full and old logs cannot be purged.");
					}
					_storageAdapter.Delete("LogEntry", "Id", list[0][0]);
				}
			}
		});
	}

	public Task DeleteLogs(string channelName, string batchId)
	{
		return AddTaskToQueue(delegate
		{
			try
			{
				AppCenterLog.Debug(AppCenterLog.LogTag, "Deleting logs from storage for channel '" + channelName + "' with batch id '" + batchId + "'");
				IList<long> list = _pendingDbIdentifierGroups[GetFullIdentifier(channelName, batchId)];
				_pendingDbIdentifierGroups.Remove(GetFullIdentifier(channelName, batchId));
				string text = "The IDs for deleting log(s) is/are:";
				foreach (long item in list)
				{
					text = text + "\n\t" + item;
					_pendingDbIdentifiers.Remove(item);
				}
				AppCenterLog.Debug(AppCenterLog.LogTag, text);
				_storageAdapter.Delete("LogEntry", "Id", list.Cast<object>().ToArray());
			}
			catch (KeyNotFoundException innerException)
			{
				throw new StorageException(innerException);
			}
		});
	}

	public Task DeleteLogs(string channelName)
	{
		return AddTaskToQueue(delegate
		{
			try
			{
				AppCenterLog.Debug(AppCenterLog.LogTag, "Deleting all logs from storage for channel '" + channelName + "'");
				ClearPendingLogStateWithoutEnqueue(channelName);
				_storageAdapter.Delete("LogEntry", "Channel", channelName);
			}
			catch (KeyNotFoundException innerException)
			{
				throw new StorageException(innerException);
			}
		});
	}

	public Task<int> CountLogsAsync(string channelName)
	{
		return AddTaskToQueue(() => _storageAdapter.Count("LogEntry", "Channel", channelName));
	}

	public Task ClearPendingLogState(string channelName)
	{
		return AddTaskToQueue(delegate
		{
			ClearPendingLogStateWithoutEnqueue(channelName);
			AppCenterLog.Debug(AppCenterLog.LogTag, "Clear pending log states for channel " + channelName);
		});
	}

	private void ClearPendingLogStateWithoutEnqueue(string channelName)
	{
		List<string> list = new List<string>();
		foreach (string key in _pendingDbIdentifierGroups.Keys)
		{
			if (!ChannelMatchesIdentifier(channelName, key))
			{
				continue;
			}
			foreach (long item in _pendingDbIdentifierGroups[key])
			{
				_pendingDbIdentifiers.Remove(item);
			}
			list.Add(key);
		}
		foreach (string item2 in list)
		{
			_pendingDbIdentifierGroups.Remove(item2);
		}
	}

	public Task<string> GetLogsAsync(string channelName, int limit, List<Log> logs)
	{
		return AddTaskToQueue(delegate
		{
			logs?.Clear();
			List<Log> list = new List<Log>();
			AppCenterLog.Debug(AppCenterLog.LogTag, $"Trying to get up to {limit} logs from storage for {channelName}");
			List<Tuple<Guid?, long>> list2 = new List<Tuple<Guid?, long>>();
			bool flag = false;
			foreach (LogEntry item in (from entries in _storageAdapter.Select("LogEntry", "Channel", channelName, "Id", _pendingDbIdentifiers.Cast<object>().ToArray(), limit)
				select new LogEntry
				{
					Id = (long)entries[0],
					Channel = (string)entries[1],
					Log = (string)entries[2]
				}).ToList())
			{
				try
				{
					Log log = LogSerializer.DeserializeLog(item.Log);
					list.Add(log);
					list2.Add(Tuple.Create(log.Sid, Convert.ToInt64(item.Id)));
				}
				catch (JsonException exception)
				{
					AppCenterLog.Error(AppCenterLog.LogTag, "Cannot deserialize a log in storage", exception);
					flag = true;
					_storageAdapter.Delete("LogEntry", "Id", item.Id);
				}
			}
			if (flag)
			{
				AppCenterLog.Warn(AppCenterLog.LogTag, "Deleted logs that could not be deserialized");
			}
			if (list2.Count == 0)
			{
				AppCenterLog.Debug(AppCenterLog.LogTag, "No available logs in storage for channel '" + channelName + "'");
				return (string)null;
			}
			string text = Guid.NewGuid().ToString();
			ProcessLogIds(channelName, text, list2);
			logs?.AddRange(list);
			return text;
		});
	}

	public Task<bool> SetMaxStorageSizeAsync(long sizeInBytes)
	{
		return AddTaskToQueue(delegate
		{
			try
			{
				AppCenterLog.Debug(AppCenterLog.LogTag, "Set max storage size.");
				return _storageAdapter.SetMaxStorageSize(sizeInBytes);
			}
			catch (Exception innerException)
			{
				throw new StorageException(innerException);
			}
		});
	}

	private void ProcessLogIds(string channelName, string batchId, IEnumerable<Tuple<Guid?, long>> idPairs)
	{
		List<long> list = new List<long>();
		string text = "The SID/ID pairs for returning logs are:";
		foreach (Tuple<Guid?, long> idPair in idPairs)
		{
			string text2 = idPair.Item1?.ToString() ?? "(null)";
			text = text + "\n\t" + text2 + " / " + idPair.Item2;
			_pendingDbIdentifiers.Add(idPair.Item2);
			list.Add(idPair.Item2);
		}
		_pendingDbIdentifierGroups.Add(GetFullIdentifier(channelName, batchId), list);
		AppCenterLog.Debug(AppCenterLog.LogTag, text);
	}

	private void InitializeDatabase()
	{
		EnsureDatabaseDirectoryExists();
		try
		{
			_storageAdapter.Initialize(_databasePath);
			_storageAdapter.CreateTable("LogEntry", new string[3] { "Id", "Channel", "Log" }, new string[3] { "INTEGER PRIMARY KEY AUTOINCREMENT", "TEXT NOT NULL", "TEXT NOT NULL" });
		}
		catch (Exception exception)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, "An error occurred while initializing storage", exception);
		}
	}

	private void EnsureDatabaseDirectoryExists()
	{
		string directoryName = Path.GetDirectoryName(_databasePath);
		if (string.IsNullOrEmpty(directoryName))
		{
			return;
		}
		try
		{
			Microsoft.AppCenter.Utils.Files.Directory directory = new Microsoft.AppCenter.Utils.Files.Directory(directoryName);
			if (!directory.Exists())
			{
				directory.Create();
			}
		}
		catch (Exception exception)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, "Failed to create database directory.", exception);
		}
	}

	public async Task WaitOperationsAsync(TimeSpan timeout)
	{
		CancellationTokenSource tokenSource = new CancellationTokenSource();
		try
		{
			Task task = AddTaskToQueue(delegate
			{
			});
			Task task2 = Task.Delay(timeout, tokenSource.Token);
			await Task.WhenAny(new Task[2] { task, task2 }).ConfigureAwait(continueOnCapturedContext: false);
		}
		finally
		{
			tokenSource.Cancel();
		}
	}

	public async Task<bool> ShutdownAsync(TimeSpan timeout)
	{
		_queue.CompleteAdding();
		_flushSemaphore.Release();
		CancellationTokenSource tokenSource = new CancellationTokenSource();
		try
		{
			Task timeoutTask = Task.Delay(timeout, tokenSource.Token);
			return await Task.WhenAny(new Task[2] { _queueFlushTask, timeoutTask }).ConfigureAwait(continueOnCapturedContext: false) != timeoutTask;
		}
		finally
		{
			tokenSource.Cancel();
		}
	}

	private static string GetFullIdentifier(string channelName, string identifier)
	{
		return channelName + "@" + identifier;
	}

	private static bool ChannelMatchesIdentifier(string channelName, string identifier)
	{
		return identifier[..identifier.LastIndexOf("@", StringComparison.Ordinal)] == channelName;
	}

	private Task AddTaskToQueue(Action action)
	{
		Task task = new Task(delegate
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				throw HandleStorageRelatedException(e);
			}
		});
		AddTaskToQueue(task);
		return task;
	}

	private Task<T> AddTaskToQueue<T>(Func<T> action)
	{
		Task<T> task = new Task<T>(delegate
		{
			try
			{
				return action();
			}
			catch (Exception ex)
			{
				AppCenterLog.Error(AppCenterLog.LogTag, "The storage operation failed", ex);
				throw HandleStorageRelatedException(ex);
			}
		});
		AddTaskToQueue(task);
		return task;
	}

	private Exception HandleStorageRelatedException(Exception e)
	{
		if (e is StorageCorruptedException)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, "Database corruption detected, deleting the file and starting fresh...", e);
			_storageAdapter.Dispose();
			try
			{
				new Microsoft.AppCenter.Utils.Files.File(_databasePath).Delete();
			}
			catch (IOException exception)
			{
				AppCenterLog.Error(AppCenterLog.LogTag, "Failed to delete database file.", exception);
			}
			InitializeDatabase();
		}
		if (!(e is StorageException))
		{
			return new StorageException(e);
		}
		return e;
	}

	private void AddTaskToQueue(Task task)
	{
		try
		{
			_queue.Add(task);
		}
		catch (InvalidOperationException)
		{
			throw new StorageException("The operation has been canceled");
		}
		_flushSemaphore.Release();
	}

	private async Task FlushQueueAsync()
	{
		while (true)
		{
			if (_queue.Count == 0)
			{
				if (_queue.IsAddingCompleted)
				{
					break;
				}
				await _flushSemaphore.WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
				continue;
			}
			Task task = _queue.Take();
			task.Start();
			try
			{
				await task.ConfigureAwait(continueOnCapturedContext: false);
			}
			catch
			{
			}
		}
	}

	public void Dispose()
	{
		_queue.CompleteAdding();
		_storageAdapter.Dispose();
	}
}
