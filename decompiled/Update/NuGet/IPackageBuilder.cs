using System.Collections.ObjectModel;
using System.IO;

namespace NuGet;

internal interface IPackageBuilder : IPackageMetadata, IPackageName
{
	Collection<IPackageFile> Files { get; }

	void Save(Stream stream);
}
