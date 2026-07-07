using System.Threading;

namespace Polly.Bulkhead;

internal static class BulkheadSemaphoreFactory
{
	public static (SemaphoreSlim, SemaphoreSlim) CreateBulkheadSemaphores(int maxParallelization, int maxQueueingActions)
	{
		SemaphoreSlim item = new SemaphoreSlim(maxParallelization, maxParallelization);
		int num = ((maxQueueingActions <= int.MaxValue - maxParallelization) ? (maxQueueingActions + maxParallelization) : int.MaxValue);
		SemaphoreSlim item2 = new SemaphoreSlim(num, num);
		return (item, item2);
	}
}
