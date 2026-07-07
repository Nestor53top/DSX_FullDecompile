namespace Medallion.Threading;

public interface IDistributedReaderWriterLockProvider
{
	IDistributedReaderWriterLock CreateReaderWriterLock(string name);
}
