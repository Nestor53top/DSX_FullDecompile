using System;
using System.IO;

namespace NuGet;

internal sealed class EmptyFrameworkFolderFile : PhysicalPackageFile
{
	public EmptyFrameworkFolderFile(string directoryPathInPackage)
		: base(() => Stream.Null)
	{
		if (directoryPathInPackage == null)
		{
			throw new ArgumentNullException("directoryPathInPackage");
		}
		base.TargetPath = System.IO.Path.Combine(directoryPathInPackage, "_._");
	}
}
