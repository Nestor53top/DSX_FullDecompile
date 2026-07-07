namespace Medallion.Threading;

public interface IDistributedLockProvider
{
	IDistributedLock CreateLock(string name);
}
