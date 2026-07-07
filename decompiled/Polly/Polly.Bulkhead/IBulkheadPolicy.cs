using System;

namespace Polly.Bulkhead;

public interface IBulkheadPolicy : IsPolicy, IDisposable
{
	int BulkheadAvailableCount { get; }

	int QueueAvailableCount { get; }
}
public interface IBulkheadPolicy<TResult> : IBulkheadPolicy, IsPolicy, IDisposable
{
}
