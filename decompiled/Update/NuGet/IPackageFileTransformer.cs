using System.Collections.Generic;

namespace NuGet;

internal interface IPackageFileTransformer
{
	void TransformFile(IPackageFile file, string targetPath, IProjectSystem projectSystem);

	void RevertFile(IPackageFile file, string targetPath, IEnumerable<IPackageFile> matchingFiles, IProjectSystem projectSystem);
}
