using System;
using System.Collections.Generic;

namespace NuGet;

internal class PackageComparer : IComparer<IPackage>
{
	public static readonly PackageComparer Version = new PackageComparer((IPackage x, IPackage y) => x.Version.CompareTo(y.Version));

	public static readonly PackageComparer IdVersion = new PackageComparer(delegate(IPackage x, IPackage y)
	{
		int num = string.Compare(x.Id, y.Id, StringComparison.OrdinalIgnoreCase);
		return (num == 0) ? x.Version.CompareTo(y.Version) : num;
	});

	private readonly Func<IPackage, IPackage, int> _compareTo;

	private PackageComparer(Func<IPackage, IPackage, int> compareTo)
	{
		_compareTo = compareTo;
	}

	public int Compare(IPackage x, IPackage y)
	{
		if (x == null && y == null)
		{
			return 0;
		}
		if (x == null)
		{
			return -1;
		}
		if (y == null)
		{
			return 1;
		}
		return _compareTo(x, y);
	}
}
