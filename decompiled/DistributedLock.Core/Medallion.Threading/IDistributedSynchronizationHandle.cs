using System;
using System.Threading;

namespace Medallion.Threading;

public interface IDistributedSynchronizationHandle : IDisposable, IAsyncDisposable
{
	CancellationToken HandleLostToken { get; }
}
