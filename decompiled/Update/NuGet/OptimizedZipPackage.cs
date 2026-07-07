using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.Versioning;
using NuGet.Resources;

namespace NuGet;

internal class OptimizedZipPackage : LocalPackage
{
	private static readonly ConcurrentDictionary<PackageName, Tuple<string, DateTimeOffset>> _cachedExpandedFolder = new ConcurrentDictionary<PackageName, Tuple<string, DateTimeOffset>>();

	private static readonly IFileSystem _tempFileSystem = new PhysicalFileSystem(Path.Combine(Path.GetTempPath(), "nuget"));

	private Dictionary<string, PhysicalPackageFile> _files;

	private ICollection<FrameworkName> _supportedFrameworks;

	private readonly IFileSystem _fileSystem;

	private readonly IFileSystem _expandedFileSystem;

	private readonly string _packagePath;

	private string _expandedFolderPath;

	private readonly bool _forceUseCache;

	public bool IsValid => _fileSystem.FileExists(_packagePath);

	protected IFileSystem FileSystem => _fileSystem;

	public OptimizedZipPackage(string fullPackagePath)
	{
		if (string.IsNullOrEmpty(fullPackagePath))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "fullPackagePath");
		}
		if (!File.Exists(fullPackagePath))
		{
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, NuGetResources.FileDoesNotExit, new object[1] { fullPackagePath }), "fullPackagePath");
		}
		string directoryName = Path.GetDirectoryName(fullPackagePath);
		_fileSystem = new PhysicalFileSystem(directoryName);
		_packagePath = Path.GetFileName(fullPackagePath);
		_expandedFileSystem = _tempFileSystem;
		EnsureManifest();
	}

	public OptimizedZipPackage(IFileSystem fileSystem, string packagePath)
	{
		if (fileSystem == null)
		{
			throw new ArgumentNullException("fileSystem");
		}
		if (string.IsNullOrEmpty(packagePath))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "packagePath");
		}
		_fileSystem = fileSystem;
		_packagePath = packagePath;
		_expandedFileSystem = _tempFileSystem;
		EnsureManifest();
	}

	public OptimizedZipPackage(IFileSystem fileSystem, string packagePath, IFileSystem expandedFileSystem)
	{
		if (fileSystem == null)
		{
			throw new ArgumentNullException("fileSystem");
		}
		if (expandedFileSystem == null)
		{
			throw new ArgumentNullException("expandedFileSystem");
		}
		if (string.IsNullOrEmpty(packagePath))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "packagePath");
		}
		_fileSystem = fileSystem;
		_packagePath = packagePath;
		_expandedFileSystem = expandedFileSystem;
		EnsureManifest();
	}

	internal OptimizedZipPackage(IFileSystem fileSystem, string packagePath, IFileSystem expandedFileSystem, bool forceUseCache)
		: this(fileSystem, packagePath, expandedFileSystem)
	{
		_forceUseCache = forceUseCache;
	}

	public override Stream GetStream()
	{
		return _fileSystem.OpenFile(_packagePath);
	}

	protected override IEnumerable<IPackageFile> GetFilesBase()
	{
		EnsurePackageFiles();
		return _files.Values;
	}

	protected override IEnumerable<IPackageAssemblyReference> GetAssemblyReferencesCore()
	{
		EnsurePackageFiles();
		return _files.Values.Where((PhysicalPackageFile file) => LocalPackage.IsAssemblyReference(file.Path)).Select((Func<PhysicalPackageFile, IPackageAssemblyReference>)((PhysicalPackageFile file) => new PhysicalPackageAssemblyReference(file)));
	}

	public override IEnumerable<FrameworkName> GetSupportedFrameworks()
	{
		EnsurePackageFiles();
		if (_supportedFrameworks == null)
		{
			IEnumerable<FrameworkName> second = _files.Values.Select((PhysicalPackageFile c) => c.TargetFramework);
			IEnumerable<FrameworkName> source = (from f in base.GetSupportedFrameworks().Concat(second)
				where f != null
				select f).Distinct();
			_supportedFrameworks = new ReadOnlyCollection<FrameworkName>(source.ToList());
		}
		return _supportedFrameworks;
	}

	private void EnsureManifest()
	{
		using Stream stream = _fileSystem.OpenFile(_packagePath);
		Package obj = Package.Open(stream);
		PackageRelationship val = ((IEnumerable<PackageRelationship>)obj.GetRelationshipsByType("http://schemas.microsoft.com/packaging/2010/07/manifest")).SingleOrDefault();
		if (val == null)
		{
			throw new InvalidOperationException(NuGetResources.PackageDoesNotContainManifest);
		}
		using Stream manifestStream = (obj.GetPart(val.TargetUri) ?? throw new InvalidOperationException(NuGetResources.PackageDoesNotContainManifest)).GetStream();
		ReadManifest(manifestStream);
	}

	private void EnsurePackageFiles()
	{
		if (_files != null && _expandedFolderPath != null && _expandedFileSystem.DirectoryExists(_expandedFolderPath))
		{
			return;
		}
		_files = new Dictionary<string, PhysicalPackageFile>();
		_supportedFrameworks = null;
		PackageName key = new PackageName(base.Id, base.Version);
		if (_expandedFileSystem == _tempFileSystem || _forceUseCache)
		{
			DateTimeOffset lastModified = _fileSystem.GetLastModified(_packagePath);
			if (!_cachedExpandedFolder.TryGetValue(key, out var value) || value.Item2 < lastModified)
			{
				value = Tuple.Create(GetExpandedFolderPath(), lastModified);
				_cachedExpandedFolder[key] = value;
			}
			_expandedFolderPath = value.Item1;
		}
		else
		{
			_expandedFolderPath = GetExpandedFolderPath();
		}
		using Stream stream = GetStream();
		foreach (PackagePart item in ((IEnumerable<PackagePart>)Package.Open(stream).GetParts()).Where((PackagePart part) => ZipPackage.IsPackageFile(part)))
		{
			string path = UriUtility.GetPath(item.Uri);
			string path2 = Path.Combine(_expandedFolderPath, path);
			bool flag = true;
			if (_expandedFileSystem.FileExists(path2))
			{
				using Stream stream2 = item.GetStream();
				using Stream stream3 = _expandedFileSystem.OpenFile(path2);
				flag = stream2.Length != stream3.Length;
			}
			if (flag)
			{
				using Stream stream4 = item.GetStream();
				try
				{
					using Stream destination = _expandedFileSystem.CreateFile(path2);
					stream4.CopyTo(destination);
				}
				catch (Exception)
				{
				}
			}
			PhysicalPackageFile value2 = new PhysicalPackageFile
			{
				SourcePath = _expandedFileSystem.GetFullPath(path2),
				TargetPath = path
			};
			_files[path] = value2;
		}
	}

	protected virtual string GetExpandedFolderPath()
	{
		return Path.GetRandomFileName();
	}

	public static void PurgeCache()
	{
		lock (_cachedExpandedFolder)
		{
			if (_cachedExpandedFolder.Count <= 0)
			{
				return;
			}
			foreach (Tuple<string, DateTimeOffset> value in _cachedExpandedFolder.Values)
			{
				try
				{
					string item = value.Item1;
					_tempFileSystem.DeleteDirectory(item, recursive: true);
				}
				catch (Exception)
				{
				}
			}
			_cachedExpandedFolder.Clear();
		}
	}
}
