using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;

namespace NuGet;

internal interface IPackage : IPackageMetadata, IPackageName, IServerPackageMetadata
{
	bool IsAbsoluteLatestVersion { get; }

	bool IsLatestVersion { get; }

	bool Listed { get; }

	DateTimeOffset? Published { get; }

	IEnumerable<IPackageAssemblyReference> AssemblyReferences { get; }

	IEnumerable<IPackageFile> GetFiles();

	IEnumerable<FrameworkName> GetSupportedFrameworks();

	Stream GetStream();
}
