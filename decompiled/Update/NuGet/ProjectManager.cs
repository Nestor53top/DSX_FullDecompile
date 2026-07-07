using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Xml.Linq;
using NuGet.Resources;

namespace NuGet;

internal class ProjectManager : IProjectManager
{
	private class PackageFileComparer : IEqualityComparer<IPackageFile>
	{
		internal static readonly PackageFileComparer Default = new PackageFileComparer();

		private PackageFileComparer()
		{
		}

		public bool Equals(IPackageFile x, IPackageFile y)
		{
			if (x.TargetFramework == y.TargetFramework)
			{
				return x.EffectivePath.Equals(y.EffectivePath, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		public int GetHashCode(IPackageFile obj)
		{
			return obj.Path.GetHashCode();
		}
	}

	private ILogger _logger;

	private IPackageConstraintProvider _constraintProvider;

	private readonly IPackageReferenceRepository _packageReferenceRepository;

	private readonly IDictionary<FileTransformExtensions, IPackageFileTransformer> _fileTransformers = new Dictionary<FileTransformExtensions, IPackageFileTransformer>
	{
		{
			new FileTransformExtensions(".transform", ".transform"),
			new XmlTransformer(GetConfigMappings())
		},
		{
			new FileTransformExtensions(".pp", ".pp"),
			new Preprocessor()
		},
		{
			new FileTransformExtensions(".install.xdt", ".uninstall.xdt"),
			new XdtTransformer()
		}
	};

	public IPackageManager PackageManager { get; private set; }

	public IPackagePathResolver PathResolver { get; private set; }

	public IPackageRepository LocalRepository { get; private set; }

	public IPackageConstraintProvider ConstraintProvider
	{
		get
		{
			return _constraintProvider ?? NullConstraintProvider.Instance;
		}
		set
		{
			_constraintProvider = value;
		}
	}

	public IProjectSystem Project { get; private set; }

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

	public event EventHandler<PackageOperationEventArgs> PackageReferenceAdding;

	public event EventHandler<PackageOperationEventArgs> PackageReferenceAdded;

	public event EventHandler<PackageOperationEventArgs> PackageReferenceRemoving;

	public event EventHandler<PackageOperationEventArgs> PackageReferenceRemoved;

	public ProjectManager(IPackageManager packageManager, IPackagePathResolver pathResolver, IProjectSystem project, IPackageRepository localRepository)
	{
		if (pathResolver == null)
		{
			throw new ArgumentNullException("pathResolver");
		}
		if (project == null)
		{
			throw new ArgumentNullException("project");
		}
		if (localRepository == null)
		{
			throw new ArgumentNullException("localRepository");
		}
		PackageManager = packageManager;
		Project = project;
		PathResolver = pathResolver;
		LocalRepository = localRepository;
		_packageReferenceRepository = LocalRepository as IPackageReferenceRepository;
	}

	public virtual void Execute(PackageOperation operation)
	{
		bool flag = LocalRepository.Exists(operation.Package);
		if (operation.Action == PackageAction.Install)
		{
			if (flag)
			{
				Logger.Log(MessageLevel.Info, NuGetResources.Log_ProjectAlreadyReferencesPackage, Project.ProjectName, operation.Package.GetFullName());
			}
			else
			{
				AddPackageReferenceToProject(operation.Package);
			}
		}
		else if (flag)
		{
			RemovePackageReferenceFromProject(operation.Package);
		}
	}

	private void AddPackageReferenceToNuGetAwareProject(IPackage package)
	{
	}

	private bool IsNuGetAwareProject()
	{
		return false;
	}

	protected void AddPackageReferenceToProject(IPackage package)
	{
		string fullName = package.GetFullName();
		Logger.Log(MessageLevel.Info, NuGetResources.Log_BeginAddPackageReference, fullName, Project.ProjectName);
		if (IsNuGetAwareProject())
		{
			AddPackageReferenceToNuGetAwareProject(package);
			Logger.Log(MessageLevel.Info, NuGetResources.Log_SuccessfullyAddedPackageReference, fullName, Project.ProjectName);
			return;
		}
		PackageOperationEventArgs e = CreateOperation(package);
		OnPackageReferenceAdding(e);
		if (!e.Cancel)
		{
			ExtractPackageFilesToProject(package);
			Logger.Log(MessageLevel.Info, NuGetResources.Log_SuccessfullyAddedPackageReference, fullName, Project.ProjectName);
			OnPackageReferenceAdded(e);
		}
	}

	protected virtual void ExtractPackageFilesToProject(IPackage package)
	{
		List<IPackageAssemblyReference> list = Project.GetCompatibleItemsCore(package.AssemblyReferences).ToList();
		List<FrameworkAssemblyReference> list2 = Project.GetCompatibleItemsCore(package.FrameworkAssemblies).ToList();
		List<IPackageFile> list3 = Project.GetCompatibleItemsCore(package.GetContentFiles()).ToList();
		List<IPackageFile> list4 = Project.GetCompatibleItemsCore(package.GetBuildFiles()).ToList();
		if (list.Count == 0 && list2.Count == 0 && list3.Count == 0 && list4.Count == 0 && (package.FrameworkAssemblies.Any() || package.AssemblyReferences.Any() || package.GetContentFiles().Any() || package.GetBuildFiles().Any()))
		{
			FrameworkName targetFramework = Project.TargetFramework;
			string text = (targetFramework.IsPortableFramework() ? VersionUtility.GetShortFrameworkName(targetFramework) : ((targetFramework != null) ? targetFramework.ToString() : null));
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.UnableToFindCompatibleItems, new object[2]
			{
				package.GetFullName(),
				text
			}));
		}
		FilterAssemblyReferences(list, package.PackageAssemblyReferences);
		try
		{
			LogTargetFrameworkInfo(package, list, list3, list4);
			Project.AddFiles(list3, _fileTransformers);
			foreach (IPackageAssemblyReference item in list)
			{
				if (!item.IsEmptyFolder())
				{
					string path = Path.Combine(PathResolver.GetInstallPath(package), item.Path);
					string relativePath = PathUtility.GetRelativePath(Project.Root, path);
					if (Project.ReferenceExists(item.Name))
					{
						Project.RemoveReference(item.Name);
					}
					Project.AddReference(relativePath);
				}
			}
			foreach (FrameworkAssemblyReference item2 in list2)
			{
				if (!Project.ReferenceExists(item2.AssemblyName))
				{
					Project.AddFrameworkReference(item2.AssemblyName);
				}
			}
			foreach (IPackageFile item3 in list4)
			{
				string targetFullPath = Path.Combine(PathResolver.GetInstallPath(package), item3.Path);
				Project.AddImport(targetFullPath, (!item3.Path.EndsWith(".props", StringComparison.OrdinalIgnoreCase)) ? ProjectImportLocation.Bottom : ProjectImportLocation.Top);
			}
		}
		finally
		{
			if (_packageReferenceRepository != null)
			{
				_packageReferenceRepository.AddPackage(package.Id, package.Version, package.DevelopmentDependency, Project.TargetFramework);
			}
			else
			{
				LocalRepository.AddPackage(package);
			}
		}
	}

	private void LogTargetFrameworkInfo(IPackage package, List<IPackageAssemblyReference> assemblyReferences, List<IPackageFile> contentFiles, List<IPackageFile> buildFiles)
	{
		if (assemblyReferences.Count > 0 || contentFiles.Count > 0 || buildFiles.Count > 0)
		{
			string text = ((Project.TargetFramework == null) ? string.Empty : VersionUtility.GetShortFrameworkName(Project.TargetFramework));
			Logger.Log(MessageLevel.Debug, NuGetResources.Debug_TargetFrameworkInfoPrefix, package.GetFullName(), Project.ProjectName, text);
			if (assemblyReferences.Count > 0)
			{
				Logger.Log(MessageLevel.Debug, NuGetResources.Debug_TargetFrameworkInfo, NuGetResources.Debug_TargetFrameworkInfo_AssemblyReferences, Path.GetDirectoryName(assemblyReferences[0].Path), VersionUtility.GetTargetFrameworkLogString(assemblyReferences[0].TargetFramework));
			}
			if (contentFiles.Count > 0)
			{
				Logger.Log(MessageLevel.Debug, NuGetResources.Debug_TargetFrameworkInfo, NuGetResources.Debug_TargetFrameworkInfo_ContentFiles, Path.GetDirectoryName(contentFiles[0].Path), VersionUtility.GetTargetFrameworkLogString(contentFiles[0].TargetFramework));
			}
			if (buildFiles.Count > 0)
			{
				Logger.Log(MessageLevel.Debug, NuGetResources.Debug_TargetFrameworkInfo, NuGetResources.Debug_TargetFrameworkInfo_BuildFiles, Path.GetDirectoryName(buildFiles[0].Path), VersionUtility.GetTargetFrameworkLogString(buildFiles[0].TargetFramework));
			}
		}
	}

	private void FilterAssemblyReferences(List<IPackageAssemblyReference> assemblyReferences, ICollection<PackageReferenceSet> packageAssemblyReferences)
	{
		if (packageAssemblyReferences == null || packageAssemblyReferences.Count <= 0)
		{
			return;
		}
		PackageReferenceSet packageReferences = Project.GetCompatibleItemsCore(packageAssemblyReferences).FirstOrDefault();
		if (packageReferences != null)
		{
			assemblyReferences.RemoveAll((IPackageAssemblyReference assembly) => !packageReferences.References.Contains<string>(assembly.Name, StringComparer.OrdinalIgnoreCase));
		}
	}

	private void RemovePackageReferenceFromNuGetAwareProject(IPackage package)
	{
	}

	private void RemovePackageReferenceFromProject(IPackage package)
	{
		if (IsNuGetAwareProject())
		{
			RemovePackageReferenceFromNuGetAwareProject(package);
			return;
		}
		string fullName = package.GetFullName();
		Logger.Log(MessageLevel.Info, NuGetResources.Log_BeginRemovePackageReference, fullName, Project.ProjectName);
		PackageOperationEventArgs e = CreateOperation(package);
		OnPackageReferenceRemoving(e);
		if (e.Cancel)
		{
			return;
		}
		IEnumerable<IPackage> enumerable = from p in LocalRepository.GetPackages()
			where p.Id != package.Id
			select p;
		IEnumerable<IPackageAssemblyReference> second = from p in enumerable
			let assemblyReferences = GetFilteredAssembliesToDelete(p)
			from assemblyReference in assemblyReferences ?? Enumerable.Empty<IPackageAssemblyReference>()
			select assemblyReference;
		IEnumerable<IPackageFile> second2 = from p in enumerable
			from file in GetCompatibleInstalledItemsForPackage(p.Id, p.GetContentFiles(), NetPortableProfileTable.Default)
			where !IsTransformFile(file.Path)
			select file;
		IEnumerable<IPackageFile> enumerable2 = GetFilteredAssembliesToDelete(package).Except(second, PackageFileComparer.Default);
		IEnumerable<IPackageFile> files = GetCompatibleInstalledItemsForPackage(package.Id, package.GetContentFiles(), NetPortableProfileTable.Default).Except(second2, PackageFileComparer.Default);
		IEnumerable<IPackageFile> compatibleInstalledItemsForPackage = GetCompatibleInstalledItemsForPackage(package.Id, package.GetBuildFiles(), NetPortableProfileTable.Default);
		Project.DeleteFiles(files, enumerable, _fileTransformers);
		foreach (IPackageAssemblyReference item in enumerable2)
		{
			Project.RemoveReference(item.Name);
		}
		foreach (IPackageFile item2 in compatibleInstalledItemsForPackage)
		{
			string targetFullPath = Path.Combine(PathResolver.GetInstallPath(package), item2.Path);
			Project.RemoveImport(targetFullPath);
		}
		LocalRepository.RemovePackage(package);
		Logger.Log(MessageLevel.Info, NuGetResources.Log_SuccessfullyRemovedPackageReference, fullName, Project.ProjectName);
		OnPackageReferenceRemoved(e);
	}

	private bool IsTransformFile(string path)
	{
		return _fileTransformers.Keys.Any((FileTransformExtensions file) => path.EndsWith(file.InstallExtension, StringComparison.OrdinalIgnoreCase) || path.EndsWith(file.UninstallExtension, StringComparison.OrdinalIgnoreCase));
	}

	private IList<IPackageAssemblyReference> GetFilteredAssembliesToDelete(IPackage package)
	{
		List<IPackageAssemblyReference> list = GetCompatibleInstalledItemsForPackage(package.Id, package.AssemblyReferences, NetPortableProfileTable.Default).ToList();
		if (list.Count == 0)
		{
			return list;
		}
		PackageReferenceSet packageReferences = GetCompatibleInstalledItemsForPackage(package.Id, package.PackageAssemblyReferences, NetPortableProfileTable.Default).FirstOrDefault();
		if (packageReferences != null)
		{
			list.RemoveAll((IPackageAssemblyReference p) => !packageReferences.References.Contains<string>(p.Name, StringComparer.OrdinalIgnoreCase));
		}
		return list;
	}

	private void OnPackageReferenceAdding(PackageOperationEventArgs e)
	{
		if (this.PackageReferenceAdding != null)
		{
			this.PackageReferenceAdding(this, e);
		}
	}

	private void OnPackageReferenceAdded(PackageOperationEventArgs e)
	{
		if (this.PackageReferenceAdded != null)
		{
			this.PackageReferenceAdded(this, e);
		}
	}

	private void OnPackageReferenceRemoved(PackageOperationEventArgs e)
	{
		if (this.PackageReferenceRemoved != null)
		{
			this.PackageReferenceRemoved(this, e);
		}
	}

	private void OnPackageReferenceRemoving(PackageOperationEventArgs e)
	{
		if (this.PackageReferenceRemoving != null)
		{
			this.PackageReferenceRemoving(this, e);
		}
	}

	private IEnumerable<T> GetCompatibleInstalledItemsForPackage<T>(string packageId, IEnumerable<T> items, NetPortableProfileTable portableProfileTable) where T : IFrameworkTargetable
	{
		FrameworkName targetFrameworkForPackage = this.GetTargetFrameworkForPackage(packageId);
		if (targetFrameworkForPackage == null)
		{
			return items;
		}
		if (VersionUtility.TryGetCompatibleItems(targetFrameworkForPackage, items, portableProfileTable, out var compatibleItems))
		{
			return compatibleItems;
		}
		return Enumerable.Empty<T>();
	}

	public PackageOperationEventArgs CreateOperation(IPackage package)
	{
		return new PackageOperationEventArgs(package, Project, PathResolver.GetInstallPath(package));
	}

	private static IDictionary<XName, Action<XElement, XElement>> GetConfigMappings()
	{
		return new Dictionary<XName, Action<XElement, XElement>> { 
		{
			XName.op_Implicit("configSections"),
			delegate(XElement parent, XElement element)
			{
				((XContainer)parent).AddFirst((object)element);
			}
		} };
	}
}
