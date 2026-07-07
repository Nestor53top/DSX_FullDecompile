using System;

namespace NuGet;

internal interface IShimControllerProvider : IDisposable
{
	IShimController Controller { get; }
}
