using System.IO;
using System.Runtime.Versioning;

namespace NuGet;

internal interface IPackageFile : IFrameworkTargetable
{
	string Path { get; }

	string EffectivePath { get; }

	FrameworkName TargetFramework { get; }

	Stream GetStream();
}
