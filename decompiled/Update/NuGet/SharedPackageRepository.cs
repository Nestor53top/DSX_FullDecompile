using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NuGet.Resources;

namespace NuGet;

internal class SharedPackageRepository : LocalPackageRepository, ISharedPackageRepository, IPackageRepository
{
	private class SharedOptimizedZipPackage : OptimizedZipPackage
	{
		private readonly string _folderPath;

		public SharedOptimizedZipPackage(IFileSystem fileSystem, string packagePath)
			: base(fileSystem, packagePath, fileSystem)
		{
			_folderPath = Path.GetDirectoryName(packagePath);
		}

		protected override string GetExpandedFolderPath()
		{
			return _folderPath;
		}
	}

	private const string StoreFilePath = "repositories.config";

	private readonly PackageReferenceFile _packageReferenceFile;

	private readonly IFileSystem _storeFileSystem;

	public PackageReferenceFile PackageReferenceFile => _packageReferenceFile;

	public override bool SupportsPrereleasePackages => true;

	public SharedPackageRepository(string path)
		: base(path)
	{
		_storeFileSystem = base.FileSystem;
	}

	public SharedPackageRepository(IPackagePathResolver resolver, IFileSystem fileSystem, IFileSystem configSettingsFileSystem)
		: this(resolver, fileSystem, fileSystem, configSettingsFileSystem)
	{
	}

	public SharedPackageRepository(IPackagePathResolver resolver, IFileSystem fileSystem, IFileSystem storeFileSystem, IFileSystem configSettingsFileSystem)
		: base(resolver, fileSystem)
	{
		if (configSettingsFileSystem == null)
		{
			throw new ArgumentNullException("configSettingsFileSystem");
		}
		_storeFileSystem = storeFileSystem ?? fileSystem;
		_packageReferenceFile = new PackageReferenceFile(configSettingsFileSystem, Constants.PackageReferenceFile);
	}

	public void RegisterRepository(PackageReferenceFile packageReferenceFile)
	{
		AddEntry(packageReferenceFile.FullPath);
	}

	public void UnregisterRepository(PackageReferenceFile packageReferenceFile)
	{
		DeleteEntry(packageReferenceFile.FullPath);
	}

	public bool IsReferenced(string packageId, SemanticVersion version)
	{
		return LoadProjectRepositories().Any((IPackageRepository r) => r.Exists(packageId, version));
	}

	public override bool Exists(string packageId, SemanticVersion version)
	{
		if (version != null && (from v in version.GetComparableVersionStrings()
			select packageId + "." + v).Any((string path) => base.FileSystem.FileExists(Path.Combine(path, path + Constants.PackageExtension)) || base.FileSystem.FileExists(Path.Combine(path, path + Constants.ManifestExtension))))
		{
			return true;
		}
		return FindPackage(packageId, version) != null;
	}

	public override IPackage FindPackage(string packageId, SemanticVersion version)
	{
		IPackage package = base.FindPackage(packageId, version);
		if (package != null)
		{
			return package;
		}
		if (version != null)
		{
			string manifestFilePath = GetManifestFilePath(packageId, version);
			if (base.FileSystem.FileExists(manifestFilePath))
			{
				string packageDirectory = base.PathResolver.GetPackageDirectory(packageId, version);
				return new UnzippedPackage(base.FileSystem, packageDirectory);
			}
		}
		return null;
	}

	public void AddPackageReferenceEntry(string packageId, SemanticVersion version)
	{
		if (_packageReferenceFile != null)
		{
			_packageReferenceFile.AddEntry(packageId, version);
		}
	}

	public override IQueryable<IPackage> GetPackages()
	{
		return SearchPackages().AsQueryable();
	}

	protected IEnumerable<IPackage> SearchPackages()
	{
		foreach (string directory in base.FileSystem.GetDirectories(""))
		{
			string text = Path.Combine(directory, directory);
			string text2 = text + Constants.PackageExtension;
			if (base.FileSystem.FileExists(text2))
			{
				yield return new SharedOptimizedZipPackage(base.FileSystem, text2);
			}
			else if (base.FileSystem.FileExists(text + Constants.ManifestExtension))
			{
				yield return new UnzippedPackage(base.FileSystem, directory);
			}
		}
	}

	public override void AddPackage(IPackage package)
	{
		base.AddPackage(package);
		if (_packageReferenceFile != null && IsSolutionLevel(package))
		{
			_packageReferenceFile.AddEntry(package.Id, package.Version);
		}
	}

	public override void RemovePackage(IPackage package)
	{
		string manifestFilePath = GetManifestFilePath(package.Id, package.Version);
		if (base.FileSystem.FileExists(manifestFilePath))
		{
			base.FileSystem.DeleteFileSafe(manifestFilePath);
		}
		string packageFilePath = GetPackageFilePath(package);
		if (base.FileSystem.FileExists(packageFilePath))
		{
			base.FileSystem.DeleteFileSafe(packageFilePath);
		}
		base.FileSystem.DeleteDirectorySafe(base.PathResolver.GetPackageDirectory(package), recursive: true);
		if (!base.FileSystem.GetFilesSafe(string.Empty).Any() && !base.FileSystem.GetDirectoriesSafe(string.Empty).Any())
		{
			base.FileSystem.DeleteDirectorySafe(string.Empty, recursive: false);
		}
		if (_packageReferenceFile != null)
		{
			_packageReferenceFile.DeleteEntry(package.Id, package.Version);
		}
	}

	public bool IsSolutionReferenced(string packageId, SemanticVersion version)
	{
		if (_packageReferenceFile != null)
		{
			return _packageReferenceFile.EntryExists(packageId, version);
		}
		return false;
	}

	protected virtual IPackageRepository CreateRepository(string path)
	{
		return new PackageReferenceRepository(PathUtility.GetAbsolutePath(PathUtility.EnsureTrailingSlash(base.FileSystem.Root), path), this);
	}

	protected override IPackage OpenPackage(string path)
	{
		//IL_0035: Expected O, but got Unknown
		if (!base.FileSystem.FileExists(path))
		{
			return null;
		}
		string extension = Path.GetExtension(path);
		if (extension.Equals(Constants.PackageExtension, StringComparison.OrdinalIgnoreCase))
		{
			try
			{
				return new SharedOptimizedZipPackage(base.FileSystem, path);
			}
			catch (FileFormatException ex)
			{
				FileFormatException innerException = ex;
				throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, NuGetResources.ErrorReadingPackage, new object[1] { path }), (Exception?)(object)innerException);
			}
		}
		if (extension.Equals(Constants.ManifestExtension, StringComparison.OrdinalIgnoreCase))
		{
			return new UnzippedPackage(base.FileSystem, Path.GetDirectoryName(path));
		}
		return null;
	}

	public IEnumerable<IPackageRepository> LoadProjectRepositories()
	{
		return GetRepositoryPaths().Select(CreateRepository);
	}

	internal IEnumerable<string> GetRepositoryPaths()
	{
		XDocument storeDocument = GetStoreDocument();
		if (storeDocument == null)
		{
			return Enumerable.Empty<string>();
		}
		bool flag = false;
		HashSet<string> hashSet = new HashSet<string>();
		foreach (XElement item in GetRepositoryElements(storeDocument).ToList())
		{
			string text = NormalizePath(item.GetOptionalAttributeValue("path"));
			if (string.IsNullOrEmpty(text) || !base.FileSystem.FileExists(text) || !hashSet.Add(text))
			{
				((XNode)item).Remove();
				flag = true;
			}
		}
		if (flag)
		{
			SaveDocument(storeDocument);
		}
		return hashSet;
	}

	private void AddEntry(string path)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		path = NormalizePath(path);
		XDocument storeDocument = GetStoreDocument(createIfNotExists: true);
		if (FindEntry(storeDocument, path) == null)
		{
			((XContainer)storeDocument.Root).Add((object)new XElement(XName.op_Implicit("repository"), (object)new XAttribute(XName.op_Implicit("path"), (object)path)));
			SaveDocument(storeDocument);
		}
	}

	private void DeleteEntry(string path)
	{
		path = NormalizePath(path);
		XDocument storeDocument = GetStoreDocument();
		if (storeDocument == null)
		{
			return;
		}
		XElement val = FindEntry(storeDocument, path);
		if (val != null)
		{
			((XNode)val).Remove();
			if (!storeDocument.Root.HasElements)
			{
				_storeFileSystem.DeleteFile("repositories.config");
			}
			else
			{
				SaveDocument(storeDocument);
			}
		}
	}

	private static IEnumerable<XElement> GetRepositoryElements(XDocument document)
	{
		return from e in ((XContainer)document.Root).Elements(XName.op_Implicit("repository"))
			select (e);
	}

	private XElement FindEntry(XDocument document, string path)
	{
		path = NormalizePath(path);
		return (from e in GetRepositoryElements(document)
			let entryPath = NormalizePath(e.GetOptionalAttributeValue("path"))
			where path.Equals(entryPath, StringComparison.OrdinalIgnoreCase)
			select e).FirstOrDefault();
	}

	private void SaveDocument(XDocument document)
	{
		List<XElement> list = (from e in GetRepositoryElements(document)
			let path = e.GetOptionalAttributeValue("path")
			where !string.IsNullOrEmpty(path)
			orderby path.ToUpperInvariant()
			select e).ToList();
		document.Root.RemoveAll();
		list.ForEach(delegate(XElement e)
		{
			((XContainer)document.Root).Add((object)e);
		});
		_storeFileSystem.AddFile("repositories.config", (Action<Stream>)document.Save);
	}

	private XDocument GetStoreDocument(bool createIfNotExists = false)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		try
		{
			if (_storeFileSystem.FileExists("repositories.config"))
			{
				using Stream input = _storeFileSystem.OpenFile("repositories.config");
				try
				{
					return XmlUtility.LoadSafe(input);
				}
				catch (XmlException)
				{
				}
			}
			if (createIfNotExists)
			{
				return new XDocument(new object[1] { (object)new XElement(XName.op_Implicit("repositories")) });
			}
			return null;
		}
		catch (Exception innerException)
		{
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.ErrorReadingFile, new object[1] { _storeFileSystem.GetFullPath("repositories.config") }), innerException);
		}
	}

	private string NormalizePath(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return path;
		}
		if (Path.IsPathRooted(path))
		{
			return PathUtility.GetRelativePath(PathUtility.EnsureTrailingSlash(base.FileSystem.Root), path);
		}
		return path;
	}

	private bool IsSolutionLevel(IPackage package)
	{
		if (package.HasProjectContent())
		{
			return false;
		}
		if (HasProjectLevelPackageDependency(package))
		{
			return false;
		}
		if (IsReferenced(package.Id, package.Version))
		{
			return false;
		}
		return true;
	}

	private bool HasProjectLevelPackageDependency(IPackage package)
	{
		IEnumerable<PackageDependency> enumerable = package.DependencySets.SelectMany((PackageDependencySet p) => p.Dependencies);
		if (enumerable.IsEmpty())
		{
			return false;
		}
		HashSet<string> solutionLevelPackages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		solutionLevelPackages.AddRange(from packageReference in PackageReferenceFile.GetPackageReferences()
			select packageReference.Id);
		return enumerable.Any((PackageDependency dependency) => !solutionLevelPackages.Contains(dependency.Id));
	}

	private string GetManifestFilePath(string packageId, SemanticVersion version)
	{
		string packageDirectory = base.PathResolver.GetPackageDirectory(packageId, version);
		string path = packageDirectory + Constants.ManifestExtension;
		return Path.Combine(packageDirectory, path);
	}
}
