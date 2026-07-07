using System;
using System.Globalization;

namespace NuGet.Resolver;

internal class Operation : PackageOperation
{
	private string _projectName;

	public IProjectManager ProjectManager { get; private set; }

	public IPackageManager PackageManager { get; private set; }

	public Operation(PackageOperation operation, IProjectManager projectManager, IPackageManager packageManager)
		: base(operation.Package, operation.Action)
	{
		if (projectManager != null && packageManager != null)
		{
			throw new ArgumentException("Only one of packageManager and projectManager can be non-null");
		}
		if (operation.Target == PackageOperationTarget.PackagesFolder && packageManager == null)
		{
			throw new ArgumentNullException("packageManager");
		}
		if (operation.Target == PackageOperationTarget.Project && projectManager == null)
		{
			throw new ArgumentNullException("projectManager");
		}
		base.Target = operation.Target;
		PackageManager = packageManager;
		ProjectManager = projectManager;
		if (ProjectManager != null)
		{
			_projectName = ProjectManager.Project.ProjectName;
		}
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[2]
		{
			base.ToString(),
			string.IsNullOrEmpty(_projectName) ? "" : (" -> " + _projectName)
		});
	}
}
