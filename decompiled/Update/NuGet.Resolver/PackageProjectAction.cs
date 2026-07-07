using System.Globalization;

namespace NuGet.Resolver;

internal class PackageProjectAction : PackageAction
{
	private string _projectName;

	public IProjectManager ProjectManager { get; private set; }

	public PackageProjectAction(PackageActionType actionType, IPackage package, IProjectManager projectManager)
		: base(actionType, package)
	{
		ProjectManager = projectManager;
		_projectName = ProjectManager.Project.ProjectName;
	}

	public override string ToString()
	{
		if (base.ActionType == PackageActionType.Install)
		{
			return string.Format(CultureInfo.InvariantCulture, "Install {0} into project '{1}'", new object[2]
			{
				base.Package.ToString(),
				_projectName
			});
		}
		return string.Format(CultureInfo.InvariantCulture, "Uninstall {0} from project '{1}'", new object[2]
		{
			base.Package.ToString(),
			_projectName
		});
	}
}
