using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.Resources;

namespace NuGet;

internal class PackageManager : IPackageManager
{
	private ILogger _logger;

	private bool _bindingRedirectEnabled = true;

	public IFileSystem FileSystem { get; set; }

	public IPackageRepository SourceRepository { get; private set; }

	public IDependencyResolver2 DependencyResolver { get; private set; }

	public ISharedPackageRepository LocalRepository { get; private set; }

	public IPackagePathResolver PathResolver { get; private set; }

	public ILogger Logger
	{
		get
		{
			return _logger ?? NullLogger.Instance;
		}
		set
		{
			_logger = value;
		}
	}

	public DependencyVersion DependencyVersion { get; set; }

	public bool CheckDowngrade { get; set; }

	public bool BindingRedirectEnabled
	{
		get
		{
			return _bindingRedirectEnabled;
		}
		set
		{
			_bindingRedirectEnabled = value;
		}
	}

	public event EventHandler<PackageOperationEventArgs> PackageInstalling;

	public event EventHandler<PackageOperationEventArgs> PackageInstalled;

	public event EventHandler<PackageOperationEventArgs> PackageUninstalling;

	public event EventHandler<PackageOperationEventArgs> PackageUninstalled;

	public PackageManager(IPackageRepository sourceRepository, string path)
		: this(sourceRepository, new DefaultPackagePathResolver(path), new PhysicalFileSystem(path))
	{
	}

	public PackageManager(IPackageRepository sourceRepository, IPackagePathResolver pathResolver, IFileSystem fileSystem)
		: this(sourceRepository, pathResolver, fileSystem, new SharedPackageRepository(pathResolver, fileSystem, fileSystem))
	{
	}

	public PackageManager(IPackageRepository sourceRepository, IPackagePathResolver pathResolver, IFileSystem fileSystem, ISharedPackageRepository localRepository)
	{
		if (sourceRepository == null)
		{
			throw new ArgumentNullException("sourceRepository");
		}
		if (pathResolver == null)
		{
			throw new ArgumentNullException("pathResolver");
		}
		if (fileSystem == null)
		{
			throw new ArgumentNullException("fileSystem");
		}
		if (localRepository == null)
		{
			throw new ArgumentNullException("localRepository");
		}
		SourceRepository = sourceRepository;
		DependencyResolver = new DependencyResolverFromRepo(sourceRepository);
		PathResolver = pathResolver;
		FileSystem = fileSystem;
		LocalRepository = localRepository;
		DependencyVersion = DependencyVersion.Lowest;
		CheckDowngrade = true;
	}

	public void Execute(PackageOperation operation)
	{
		bool flag = LocalRepository.Exists(operation.Package);
		if (operation.Action == PackageAction.Install)
		{
			if (flag)
			{
				Logger.Log(MessageLevel.Info, NuGetResources.Log_PackageAlreadyInstalled, operation.Package.GetFullName());
			}
			else
			{
				ExecuteInstall(operation.Package);
			}
		}
		else if (flag)
		{
			ExecuteUninstall(operation.Package);
		}
	}

	protected void ExecuteInstall(IPackage package)
	{
		string fullName = package.GetFullName();
		Logger.Log(MessageLevel.Info, NuGetResources.Log_BeginInstallPackage, fullName);
		PackageOperationEventArgs e = CreateOperation(package);
		OnInstalling(e);
		if (!e.Cancel)
		{
			OnExpandFiles(e);
			LocalRepository.AddPackage(package);
			Logger.Log(MessageLevel.Info, NuGetResources.Log_PackageInstalledSuccessfully, fullName);
			OnInstalled(e);
		}
	}

	private void ExpandFiles(IPackage package)
	{
		IBatchProcessor<string> batchProcessor = FileSystem as IBatchProcessor<string>;
		try
		{
			List<IPackageFile> list = package.GetFiles().ToList();
			batchProcessor?.BeginProcessing(list.Select((IPackageFile p) => p.Path), PackageAction.Install);
			string packageDirectory = PathResolver.GetPackageDirectory(package);
			FileSystem.AddFiles(list, packageDirectory);
			if (PackageHelper.IsSatellitePackage(package, LocalRepository, null, out var runtimePackage))
			{
				IEnumerable<IPackageFile> satelliteFiles = package.GetSatelliteFiles();
				string packageDirectory2 = PathResolver.GetPackageDirectory(runtimePackage);
				FileSystem.AddFiles(satelliteFiles, packageDirectory2);
			}
		}
		finally
		{
			batchProcessor?.EndProcessing();
		}
	}

	protected virtual void ExecuteUninstall(IPackage package)
	{
		string fullName = package.GetFullName();
		Logger.Log(MessageLevel.Info, NuGetResources.Log_BeginUninstallPackage, fullName);
		PackageOperationEventArgs e = CreateOperation(package);
		OnUninstalling(e);
		if (!e.Cancel)
		{
			OnRemoveFiles(e);
			LocalRepository.RemovePackage(package);
			Logger.Log(MessageLevel.Info, NuGetResources.Log_SuccessfullyUninstalledPackage, fullName);
			OnUninstalled(e);
		}
	}

	private void RemoveFiles(IPackage package)
	{
		string packageDirectory = PathResolver.GetPackageDirectory(package);
		if (PackageHelper.IsSatellitePackage(package, LocalRepository, null, out var runtimePackage))
		{
			IEnumerable<IPackageFile> satelliteFiles = package.GetSatelliteFiles();
			string packageDirectory2 = PathResolver.GetPackageDirectory(runtimePackage);
			FileSystem.DeleteFiles(satelliteFiles, packageDirectory2);
		}
		FileSystem.DeleteFiles(package.GetFiles(), packageDirectory);
	}

	protected virtual void OnInstalling(PackageOperationEventArgs e)
	{
		if (this.PackageInstalling != null)
		{
			this.PackageInstalling(this, e);
		}
	}

	protected virtual void OnExpandFiles(PackageOperationEventArgs e)
	{
		ExpandFiles(e.Package);
	}

	protected virtual void OnInstalled(PackageOperationEventArgs e)
	{
		if (this.PackageInstalled != null)
		{
			this.PackageInstalled(this, e);
		}
	}

	protected virtual void OnUninstalling(PackageOperationEventArgs e)
	{
		if (this.PackageUninstalling != null)
		{
			this.PackageUninstalling(this, e);
		}
	}

	protected virtual void OnRemoveFiles(PackageOperationEventArgs e)
	{
		RemoveFiles(e.Package);
	}

	protected virtual void OnUninstalled(PackageOperationEventArgs e)
	{
		if (this.PackageUninstalled != null)
		{
			this.PackageUninstalled(this, e);
		}
	}

	public PackageOperationEventArgs CreateOperation(IPackage package)
	{
		return new PackageOperationEventArgs(package, FileSystem, PathResolver.GetInstallPath(package));
	}

	public bool IsProjectLevel(IPackage package)
	{
		if (!package.HasProjectContent() && !package.DependencySets.SelectMany((PackageDependencySet p) => p.Dependencies).Any())
		{
			return LocalRepository.IsReferenced(package.Id, package.Version);
		}
		return true;
	}

	public virtual void AddBindingRedirects(IProjectManager projectManager)
	{
	}

	public virtual IPackage LocatePackageToUninstall(IProjectManager projectManager, string id, SemanticVersion version)
	{
		return LocalRepository.FindPackagesById(id).SingleOrDefault() ?? throw new InvalidOperationException();
	}
}
