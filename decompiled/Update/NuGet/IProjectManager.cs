using System;

namespace NuGet;

internal interface IProjectManager
{
	IPackageRepository LocalRepository { get; }

	IPackageManager PackageManager { get; }

	ILogger Logger { get; set; }

	IProjectSystem Project { get; }

	IPackageConstraintProvider ConstraintProvider { get; set; }

	event EventHandler<PackageOperationEventArgs> PackageReferenceAdded;

	event EventHandler<PackageOperationEventArgs> PackageReferenceAdding;

	event EventHandler<PackageOperationEventArgs> PackageReferenceRemoved;

	event EventHandler<PackageOperationEventArgs> PackageReferenceRemoving;

	void Execute(PackageOperation operation);
}
