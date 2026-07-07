using System;
using System.Globalization;

namespace NuGet;

internal class PackageOperation
{
	public IPackage Package { get; private set; }

	public PackageAction Action { get; private set; }

	public PackageOperationTarget Target { get; set; }

	public PackageOperation(IPackage package, PackageAction action)
	{
		Package = package;
		Action = action;
		Target = PackageOperationTarget.Project;
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", new object[3]
		{
			(Action == PackageAction.Install) ? "+" : "-",
			Package.Id,
			Package.Version
		});
	}

	public override bool Equals(object obj)
	{
		if (obj is PackageOperation packageOperation && packageOperation.Action == Action && packageOperation.Package.Id.Equals(Package.Id, StringComparison.OrdinalIgnoreCase))
		{
			return packageOperation.Package.Version.Equals(Package.Version);
		}
		return false;
	}

	public override int GetHashCode()
	{
		HashCodeCombiner hashCodeCombiner = new HashCodeCombiner();
		hashCodeCombiner.AddObject(Package.Id);
		hashCodeCombiner.AddObject(Package.Version);
		hashCodeCombiner.AddObject(Action);
		return hashCodeCombiner.CombinedHash;
	}
}
