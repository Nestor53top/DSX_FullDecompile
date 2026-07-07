using System;

namespace NuGet;

public interface IShimControllerProvider : IDisposable
{
	IShimController Controller { get; }
}
