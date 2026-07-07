using System.Globalization;

namespace NuGet.Resolver;

internal class PackageSolutionAction : PackageAction
{
	public IPackageManager PackageManager { get; private set; }

	public PackageSolutionAction(PackageActionType actionType, IPackage package, IPackageManager packageManager)
		: base(actionType, package)
	{
		PackageManager = packageManager;
	}

	public override string ToString()
	{
		return base.ActionType switch
		{
			PackageActionType.Install => string.Format(CultureInfo.InvariantCulture, "Install {0} into solution", new object[1] { base.Package.ToString() }), 
			PackageActionType.Uninstall => string.Format(CultureInfo.InvariantCulture, "Uninstall {0} from solution", new object[1] { base.Package.ToString() }), 
			PackageActionType.AddToPackagesFolder => string.Format(CultureInfo.InvariantCulture, "Add {0} into packages folder", new object[1] { base.Package.ToString() }), 
			_ => string.Format(CultureInfo.InvariantCulture, "Delete {0} from packages folder", new object[1] { base.Package.ToString() }), 
		};
	}
}
