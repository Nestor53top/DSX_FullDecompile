using System.IO;
using System.Runtime.Versioning;

namespace NuGet;

public interface IPackageFile : IFrameworkTargetable
{
	string Path { get; }

	string EffectivePath { get; }

	FrameworkName TargetFramework { get; }

	Stream GetStream();
}
