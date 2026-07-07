using System;
using System.IO;

namespace NuGet;

internal class DefaultPackagePathResolver : IPackagePathResolver
{
	private readonly IFileSystem _fileSystem;

	private readonly bool _useSideBySidePaths;

	public DefaultPackagePathResolver(string path)
		: this(new PhysicalFileSystem(path))
	{
	}

	public DefaultPackagePathResolver(IFileSystem fileSystem)
		: this(fileSystem, useSideBySidePaths: true)
	{
	}

	public DefaultPackagePathResolver(string path, bool useSideBySidePaths)
		: this(new PhysicalFileSystem(path), useSideBySidePaths)
	{
	}

	public DefaultPackagePathResolver(IFileSystem fileSystem, bool useSideBySidePaths)
	{
		_useSideBySidePaths = useSideBySidePaths;
		if (fileSystem == null)
		{
			throw new ArgumentNullException("fileSystem");
		}
		_fileSystem = fileSystem;
	}

	public virtual string GetInstallPath(IPackage package)
	{
		return Path.Combine(_fileSystem.Root, GetPackageDirectory(package));
	}

	public virtual string GetPackageDirectory(IPackage package)
	{
		return GetPackageDirectory(package.Id, package.Version);
	}

	public virtual string GetPackageFileName(IPackage package)
	{
		return GetPackageFileName(package.Id, package.Version);
	}

	public virtual string GetPackageDirectory(string packageId, SemanticVersion version)
	{
		string text = packageId;
		if (_useSideBySidePaths)
		{
			text = text + "." + version;
		}
		return text;
	}

	public virtual string GetPackageFileName(string packageId, SemanticVersion version)
	{
		string text = packageId;
		if (_useSideBySidePaths)
		{
			text = text + "." + version;
		}
		return text + Constants.PackageExtension;
	}
}
