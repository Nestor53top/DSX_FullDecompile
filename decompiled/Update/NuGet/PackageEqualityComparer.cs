using System;
using System.Collections.Generic;

namespace NuGet;

internal sealed class PackageEqualityComparer : IEqualityComparer<IPackageName>
{
	public static readonly PackageEqualityComparer IdAndVersion = new PackageEqualityComparer((IPackageName x, IPackageName y) => x.Id.Equals(y.Id, StringComparison.OrdinalIgnoreCase) && x.Version.Equals(y.Version), (IPackageName x) => x.Id.GetHashCode() ^ x.Version.GetHashCode());

	public static readonly PackageEqualityComparer Id = new PackageEqualityComparer((IPackageName x, IPackageName y) => x.Id.Equals(y.Id, StringComparison.OrdinalIgnoreCase), (IPackageName x) => x.Id.GetHashCode());

	private readonly Func<IPackageName, IPackageName, bool> _equals;

	private readonly Func<IPackageName, int> _getHashCode;

	private PackageEqualityComparer(Func<IPackageName, IPackageName, bool> equals, Func<IPackageName, int> getHashCode)
	{
		_equals = equals;
		_getHashCode = getHashCode;
	}

	public bool Equals(IPackageName x, IPackageName y)
	{
		return _equals(x, y);
	}

	public int GetHashCode(IPackageName obj)
	{
		return _getHashCode(obj);
	}
}
