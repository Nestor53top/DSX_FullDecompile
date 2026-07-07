using System;
using System.Collections.Generic;
using System.Linq;
using SQLitePCL;

namespace Microsoft.AppCenter.Storage;

internal class StorageAdapter : IStorageAdapter, IDisposable
{
	private sqlite3 _db;

	static StorageAdapter()
	{
		try
		{
			Batteries_V2.Init();
		}
		catch (Exception exception)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, "Failed to initialize sqlite3 provider.", exception);
		}
	}

	public void Initialize(string databasePath)
	{
		int num = raw.sqlite3_open(databasePath, out _db);
		if (num != 0)
		{
			throw ToStorageException(num, "Failed to open database connection");
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (_db != null)
		{
			_db.Dispose();
			_db = null;
		}
	}

	private void BindParameter(sqlite3_stmt stmt, int index, object value)
	{
		int num;
		if (value is string)
		{
			num = raw.sqlite3_bind_text(stmt, index, (string)value);
		}
		else if (value is int)
		{
			num = raw.sqlite3_bind_int(stmt, index, (int)value);
		}
		else
		{
			if (!(value is long))
			{
				throw new NotSupportedException("Type " + value.GetType().FullName + " not supported.");
			}
			num = raw.sqlite3_bind_int64(stmt, index, (long)value);
		}
		if (num != 0)
		{
			throw ToStorageException(num, $"Failed to bind {index} parameter");
		}
	}

	private void BindParameters(sqlite3_stmt stmt, IList<object> values)
	{
		for (int i = 0; i < values?.Count; i++)
		{
			BindParameter(stmt, i + 1, values[i]);
		}
	}

	private object GetColumnValue(sqlite3_stmt stmt, int index)
	{
		int num = raw.sqlite3_column_type(stmt, index);
		switch (num)
		{
		case 1:
			return raw.sqlite3_column_int64(stmt, index);
		case 3:
			return raw.sqlite3_column_text(stmt, index).utf8_to_string();
		default:
			AppCenterLog.Error(AppCenterLog.LogTag, $"Attempt to get unsupported column value {num}.");
			return null;
		}
	}

	private int ExecuteNonSelectionSqlQuery(string query, IList<object> args = null)
	{
		int num = raw.sqlite3_prepare_v2(_db ?? throw new StorageException("The database wasn't initialized."), query, out var stmt);
		if (num != 0)
		{
			throw ToStorageException(num, "Failed to prepare SQL query");
		}
		try
		{
			BindParameters(stmt, args);
			num = raw.sqlite3_step(stmt);
			if (num != 101)
			{
				throw ToStorageException(num, "Failed to run query");
			}
		}
		finally
		{
			num = raw.sqlite3_finalize(stmt);
			if (num != 0)
			{
				AppCenterLog.Error(AppCenterLog.LogTag, $"Failed to finalize statement, result={num}");
			}
		}
		return num;
	}

	private List<object[]> ExecuteSelectionSqlQuery(string query, IList<object> args = null)
	{
		int num = raw.sqlite3_prepare_v2(_db ?? throw new StorageException("The database wasn't initialized."), query, out var stmt);
		if (num != 0)
		{
			throw ToStorageException(num, "Failed to prepare SQL query");
		}
		try
		{
			List<object[]> list = new List<object[]>();
			BindParameters(stmt, args);
			while (raw.sqlite3_step(stmt) == 100)
			{
				int count = raw.sqlite3_column_count(stmt);
				list.Add((from i in Enumerable.Range(0, count)
					select GetColumnValue(stmt, i)).ToArray());
			}
			return list;
		}
		finally
		{
			num = raw.sqlite3_finalize(stmt);
			if (num != 0)
			{
				AppCenterLog.Error(AppCenterLog.LogTag, $"Failed to finalize statement, result={num}");
			}
		}
	}

	private long GetMaxPageCount()
	{
		return GetPragmaValue("max_page_count");
	}

	private long GetPageCount()
	{
		return GetPragmaValue("page_count");
	}

	private long GetPageSize()
	{
		return GetPragmaValue("page_size");
	}

	private long GetPragmaValue(string valueName)
	{
		return (long)(ExecuteSelectionSqlQuery("PRAGMA " + valueName + ";").FirstOrDefault()?.FirstOrDefault() ?? ((object)0L));
	}

	public long GetMaxStorageSize()
	{
		try
		{
			return GetMaxPageCount() * GetPageSize();
		}
		catch (StorageException)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, "Could not get max storage size.");
			return -1L;
		}
	}

	public bool SetMaxStorageSize(long sizeInBytes)
	{
		sqlite3 db = _db ?? throw new StorageException("The database wasn't initialized.");
		long pageCount = GetPageCount();
		long pageSize = GetPageSize();
		AppCenterLog.Info(AppCenterLog.LogTag, $"Found {pageCount} pages in the database.");
		long num = (Convert.ToBoolean(sizeInBytes % pageSize) ? (sizeInBytes / pageSize + 1) : (sizeInBytes / pageSize));
		if (pageCount > num)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, $"Cannot change database size to {sizeInBytes} bytes as it would cause a loss of data. " + "Maximum database size will not be changed.");
			return false;
		}
		int num2 = raw.sqlite3_exec(db, $"PRAGMA max_page_count = {num};");
		if (num2 != 0)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, $"Could not change maximum database size to {sizeInBytes} bytes. SQLite error code: {num2}.");
			return false;
		}
		long maxPageCount = GetMaxPageCount();
		long num3 = maxPageCount * pageSize;
		if (num != maxPageCount)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, $"Could not change maximum database size to {sizeInBytes} bytes, current maximum size is {num3} bytes.");
			return false;
		}
		if (sizeInBytes == num3)
		{
			AppCenterLog.Info(AppCenterLog.LogTag, $"Changed maximum database size to {num3} bytes.");
		}
		else
		{
			AppCenterLog.Info(AppCenterLog.LogTag, $"Changed maximum database size to {num3} bytes (next multiple of 4KiB).");
		}
		return true;
	}

	public void CreateTable(string tableName, string[] columnNames, string[] columnTypes)
	{
		string text = string.Join(",", from i in Enumerable.Range(0, columnNames.Length)
			select columnNames[i] + " " + columnTypes[i]);
		ExecuteNonSelectionSqlQuery("CREATE TABLE IF NOT EXISTS " + tableName + " (" + text + ");");
	}

	public int Count(string tableName, string columnName, object value)
	{
		return (int)(long)(ExecuteSelectionSqlQuery("SELECT COUNT(*) FROM " + tableName + " WHERE " + columnName + " = ?;", new object[1] { value }).FirstOrDefault()?.FirstOrDefault() ?? ((object)0L));
	}

	public IList<object[]> Select(string tableName, string columnName, object value, string excludeColumnName, object[] excludeValues, int? limit = null, string[] orderList = null)
	{
		string text = columnName + " = ?";
		List<object> list = new List<object> { value };
		if (excludeValues != null && excludeValues.Length != 0)
		{
			text = text + " AND " + excludeColumnName + " NOT IN (" + BuildBindingMask(excludeValues.Length) + ")";
			list.AddRange(excludeValues);
		}
		string text2 = (limit.HasValue ? $" LIMIT {limit}" : string.Empty);
		string text3 = ((orderList != null && orderList.Length != 0) ? (" ORDER BY " + string.Join(",", orderList) + " ASC") : string.Empty);
		string query = "SELECT * FROM " + tableName + " WHERE " + text + text3 + text2 + ";";
		return ExecuteSelectionSqlQuery(query, list);
	}

	public void Insert(string tableName, string[] columnNames, ICollection<object[]> values)
	{
		string text = string.Join(",", columnNames);
		string element = "(" + BuildBindingMask(values.First().Length) + ")";
		string text2 = string.Join(",", Enumerable.Repeat(element, values.Count));
		object[] args = values.SelectMany((object[] i) => i).ToArray();
		ExecuteNonSelectionSqlQuery("INSERT INTO " + tableName + "(" + text + ") VALUES " + text2 + ";", args);
	}

	public void Delete(string tableName, string columnName, params object[] values)
	{
		string text = columnName + " IN (" + BuildBindingMask(values.Length) + ")";
		ExecuteNonSelectionSqlQuery("DELETE FROM " + tableName + " WHERE " + text + ";", values);
	}

	private StorageException ToStorageException(int result, string message)
	{
		string arg = raw.sqlite3_errmsg(_db).utf8_to_string();
		string message2 = $"{message}, result={result}\n\t{arg}";
		switch (result)
		{
		case 11:
		case 26:
			return new StorageCorruptedException(message2);
		case 13:
			return new StorageFullException(message2);
		default:
			return new StorageException(message2);
		}
	}

	private static string BuildBindingMask(int amount)
	{
		return string.Join(",", Enumerable.Repeat("?", amount));
	}
}
