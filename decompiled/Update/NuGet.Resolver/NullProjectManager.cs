using System;

namespace NuGet.Resolver;

internal class NullProjectManager : IProjectManager
{
	private IPackageRepository _localRepository;

	private IProjectSystem _project;

	public IPackageRepository LocalRepository => _localRepository;

	public IPackageManager PackageManager { get; private set; }

	public ILogger Logger { get; set; }

	public IProjectSystem Project => _project;

	public IPackageConstraintProvider ConstraintProvider
	{
		get
		{
			return NullConstraintProvider.Instance;
		}
		set
		{
		}
	}

	public event EventHandler<PackageOperationEventArgs> PackageReferenceAdded;

	public event EventHandler<PackageOperationEventArgs> PackageReferenceAdding;

	public event EventHandler<PackageOperationEventArgs> PackageReferenceRemoved;

	public event EventHandler<PackageOperationEventArgs> PackageReferenceRemoving;

	public NullProjectManager(IPackageManager packageManager)
	{
		_localRepository = new VirtualRepository(null);
		_project = new NullProjectSystem();
		PackageManager = packageManager;
	}

	public void Execute(PackageOperation operation)
	{
	}
}
