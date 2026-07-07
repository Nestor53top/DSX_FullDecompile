using System;

namespace NuGet;

[Flags]
internal enum PackageSaveModes
{
	None = 0,
	Nuspec = 1,
	Nupkg = 2
}
