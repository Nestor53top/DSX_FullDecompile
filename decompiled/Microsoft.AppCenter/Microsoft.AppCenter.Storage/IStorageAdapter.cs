using System;
using System.Collections.Generic;

namespace Microsoft.AppCenter.Storage;

public interface IStorageAdapter : IDisposable
{
	void Initialize(string databasePath);

	void CreateTable(string tableName, string[] columnNames, string[] columnTypes);

	int Count(string tableName, string columnName, object value);

	IList<object[]> Select(string tableName, string columnName, object value, string excludeColumnName, object[] excludeValues, int? limit = null, string[] orderList = null);

	void Insert(string tableName, string[] columnNames, ICollection<object[]> values);

	void Delete(string tableName, string columnName, params object[] values);

	bool SetMaxStorageSize(long sizeInBytes);

	long GetMaxStorageSize();
}
