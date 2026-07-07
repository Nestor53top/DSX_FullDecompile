namespace NuGet.Resolver;

public abstract class PackageAction
{
	public PackageActionType ActionType { get; private set; }

	public IPackage Package { get; private set; }

	protected PackageAction(PackageActionType actionType, IPackage package)
	{
		ActionType = actionType;
		Package = package;
	}
}
