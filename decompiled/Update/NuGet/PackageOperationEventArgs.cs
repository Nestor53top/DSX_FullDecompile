using System.ComponentModel;

namespace NuGet;

internal class PackageOperationEventArgs : CancelEventArgs
{
	public string InstallPath { get; private set; }

	public IPackage Package { get; private set; }

	public IFileSystem FileSystem { get; private set; }

	public PackageOperationEventArgs(IPackage package, IFileSystem fileSystem, string installPath)
	{
		Package = package;
		InstallPath = installPath;
		FileSystem = fileSystem;
	}
}
