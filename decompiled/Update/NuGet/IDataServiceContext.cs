using System;
using System.Collections.Generic;
using System.Data.Services.Client;

namespace NuGet;

[CLSCompliant(false)]
internal interface IDataServiceContext
{
	Uri BaseUri { get; }

	bool IgnoreMissingProperties { get; set; }

	event EventHandler<SendingRequest2EventArgs> SendingRequest;

	event EventHandler<ReadingWritingEntityEventArgs> ReadingEntity;

	bool SupportsServiceMethod(string methodName);

	bool SupportsProperty(string propertyName);

	IDataServiceQuery<T> CreateQuery<T>(string entitySetName);

	IDataServiceQuery<T> CreateQuery<T>(string entitySetName, IDictionary<string, object> queryOptions);

	IEnumerable<T> ExecuteBatch<T>(DataServiceRequest request);

	IEnumerable<T> Execute<T>(Type elementType, DataServiceQueryContinuation continuation);
}
