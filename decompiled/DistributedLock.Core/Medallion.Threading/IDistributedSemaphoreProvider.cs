namespace Medallion.Threading;

public interface IDistributedSemaphoreProvider
{
	IDistributedSemaphore CreateSemaphore(string name, int maxCount);
}
