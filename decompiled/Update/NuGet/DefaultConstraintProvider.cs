using System;
using System.Collections.Generic;

namespace NuGet;

internal class DefaultConstraintProvider : IPackageConstraintProvider
{
	private readonly Dictionary<string, IVersionSpec> _constraints = new Dictionary<string, IVersionSpec>(StringComparer.OrdinalIgnoreCase);

	public string Source => string.Empty;

	public void AddConstraint(string packageId, IVersionSpec versionSpec)
	{
		_constraints[packageId] = versionSpec;
	}

	public IVersionSpec GetConstraint(string packageId)
	{
		if (_constraints.TryGetValue(packageId, out var value))
		{
			return value;
		}
		return null;
	}
}
