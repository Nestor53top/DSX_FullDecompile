using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace NuGet;

internal class FrameworkNameEqualityComparer : IEqualityComparer<FrameworkName>
{
	public static readonly FrameworkNameEqualityComparer Default = new FrameworkNameEqualityComparer();

	private FrameworkNameEqualityComparer()
	{
	}

	public bool Equals(FrameworkName x, FrameworkName y)
	{
		if (string.Equals(x.Identifier, y.Identifier, StringComparison.OrdinalIgnoreCase) && x.Version == y.Version)
		{
			return string.Equals(x.Profile, y.Profile, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public int GetHashCode(FrameworkName x)
	{
		return x.GetHashCode();
	}
}
