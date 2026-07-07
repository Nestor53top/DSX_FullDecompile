using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using NuGet.Resources;

namespace NuGet;

internal class UnzippedPackage : LocalPackage
{
	private readonly IFileSystem _repositoryFileSystem;

	private readonly string _packageFileName;

	private readonly string _packageName;

	public UnzippedPackage(string repositoryDirectory, string packageName)
		: this(new PhysicalFileSystem(repositoryDirectory), packageName)
	{
	}

	public UnzippedPackage(IFileSystem repositoryFileSystem, string packageName)
	{
		if (repositoryFileSystem == null)
		{
			throw new ArgumentNullException("repositoryFileSystem");
		}
		if (string.IsNullOrEmpty(packageName))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "packageName");
		}
		_packageName = packageName;
		_packageFileName = packageName + Constants.PackageExtension;
		_repositoryFileSystem = repositoryFileSystem;
		EnsureManifest();
	}

	public override Stream GetStream()
	{
		if (_repositoryFileSystem.FileExists(_packageFileName))
		{
			return _repositoryFileSystem.OpenFile(_packageFileName);
		}
		string path = Path.Combine(_packageName, _packageFileName);
		return _repositoryFileSystem.OpenFile(path);
	}

	public override IEnumerable<FrameworkName> GetSupportedFrameworks()
	{
		IEnumerable<FrameworkName> second = from file in GetPackageFilePaths().Select(GetPackageRelativePath)
			let targetFramework = VersionUtility.ParseFrameworkNameFromFilePath(file, out var _)
			where targetFramework != null
			select targetFramework;
		return base.GetSupportedFrameworks().Concat(second).Distinct();
	}

	protected override IEnumerable<IPackageFile> GetFilesBase()
	{
		return from p in GetPackageFilePaths()
			select new PhysicalPackageFile
			{
				SourcePath = _repositoryFileSystem.GetFullPath(p),
				TargetPath = GetPackageRelativePath(p)
			};
	}

	protected override IEnumerable<IPackageAssemblyReference> GetAssemblyReferencesCore()
	{
		string path = Path.Combine(_packageName, Constants.LibDirectory);
		return from p in _repositoryFileSystem.GetFiles(path, "*.*", recursive: true)
			let targetPath = GetPackageRelativePath(p)
			where LocalPackage.IsAssemblyReference(targetPath)
			select new PhysicalPackageAssemblyReference
			{
				SourcePath = _repositoryFileSystem.GetFullPath(p),
				TargetPath = targetPath
			};
	}

	private IEnumerable<string> GetPackageFilePaths()
	{
		return from p in _repositoryFileSystem.GetFiles(_packageName, "*.*", recursive: true)
			where !PackageHelper.IsManifest(p) && !PackageHelper.IsPackageFile(p)
			select p;
	}

	private string GetPackageRelativePath(string path)
	{
		return path.Substring(_packageName.Length + 1);
	}

	private void EnsureManifest()
	{
		string path = Path.Combine(_packageName, _packageName + Constants.ManifestExtension);
		if (!_repositoryFileSystem.FileExists(path))
		{
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.Manifest_NotFound, new object[1] { _repositoryFileSystem.GetFullPath(path) }));
		}
		using (Stream manifestStream = _repositoryFileSystem.OpenFile(path))
		{
			ReadManifest(manifestStream);
		}
		base.Published = _repositoryFileSystem.GetLastModified(path);
	}
}
