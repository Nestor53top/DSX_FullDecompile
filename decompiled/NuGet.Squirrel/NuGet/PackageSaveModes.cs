using System;

namespace NuGet;

[Flags]
public enum PackageSaveModes
{
	None = 0,
	Nuspec = 1,
	Nupkg = 2
}
