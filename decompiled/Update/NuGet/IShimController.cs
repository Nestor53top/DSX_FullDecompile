using System;

namespace NuGet;

internal interface IShimController : IDisposable
{
	void Enable(IPackageSourceProvider sourceProvider);

	void UpdateSources();

	void Disable();
}
