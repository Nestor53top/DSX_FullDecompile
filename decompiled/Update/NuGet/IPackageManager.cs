using System;

namespace NuGet;

internal interface IPackageManager
{
	IFileSystem FileSystem { get; set; }

	ISharedPackageRepository LocalRepository { get; }

	ILogger Logger { get; set; }

	DependencyVersion DependencyVersion { get; set; }

	IPackageRepository SourceRepository { get; }

	IDependencyResolver2 DependencyResolver { get; }

	IPackagePathResolver PathResolver { get; }

	bool BindingRedirectEnabled { get; set; }

	event EventHandler<PackageOperationEventArgs> PackageInstalled;

	event EventHandler<PackageOperationEventArgs> PackageInstalling;

	event EventHandler<PackageOperationEventArgs> PackageUninstalled;

	event EventHandler<PackageOperationEventArgs> PackageUninstalling;

	void Execute(PackageOperation operation);

	bool IsProjectLevel(IPackage package);

	void AddBindingRedirects(IProjectManager projectManager);

	IPackage LocatePackageToUninstall(IProjectManager projectManager, string id, SemanticVersion version);
}
