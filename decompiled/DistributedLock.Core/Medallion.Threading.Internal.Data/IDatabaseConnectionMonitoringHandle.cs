using System;
using System.Threading;

namespace Medallion.Threading.Internal.Data;

internal interface IDatabaseConnectionMonitoringHandle : IDisposable
{
	CancellationToken ConnectionLostToken { get; }
}
