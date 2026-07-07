namespace HidSharp.Platform.SystemEvents;

internal abstract class EventManager
{
	internal abstract void Start();

	public abstract SystemEvent CreateEvent(string name);

	public abstract SystemMutex CreateMutex(string name);

	public bool MutexMayExist(string name)
	{
		using SystemMutex systemMutex = CreateMutex(name);
		return !systemMutex.CreatedNew;
	}
}
