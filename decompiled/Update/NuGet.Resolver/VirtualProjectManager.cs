namespace NuGet.Resolver;

internal class VirtualProjectManager
{
	public IProjectManager ProjectManager { get; private set; }

	public VirtualRepository LocalRepository { get; private set; }

	public VirtualProjectManager(IProjectManager projectManager)
	{
		ProjectManager = projectManager;
		LocalRepository = new VirtualRepository(projectManager.LocalRepository);
	}
}
