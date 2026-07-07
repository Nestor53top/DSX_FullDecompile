using System;

namespace NuGet;

[Flags]
internal enum PackageTargets
{
	None = 0,
	Project = 1,
	External = 2,
	All = 3
}
