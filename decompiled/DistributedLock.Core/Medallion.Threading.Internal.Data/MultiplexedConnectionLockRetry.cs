namespace Medallion.Threading.Internal.Data;

internal enum MultiplexedConnectionLockRetry
{
	NoRetry,
	RetryOnThisLock,
	Retry
}
