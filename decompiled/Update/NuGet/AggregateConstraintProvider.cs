using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGet;

internal class AggregateConstraintProvider : IPackageConstraintProvider
{
	private readonly IEnumerable<IPackageConstraintProvider> _constraintProviders;

	public string Source => string.Join(", ", _constraintProviders.Select((IPackageConstraintProvider cp) => cp.Source));

	public AggregateConstraintProvider(params IPackageConstraintProvider[] constraintProviders)
	{
		if (constraintProviders.IsEmpty() || constraintProviders.Any((IPackageConstraintProvider cp) => cp == null))
		{
			throw new ArgumentNullException("constraintProviders");
		}
		_constraintProviders = constraintProviders;
	}

	public IVersionSpec GetConstraint(string packageId)
	{
		return _constraintProviders.Select((IPackageConstraintProvider cp) => cp.GetConstraint(packageId)).FirstOrDefault((IVersionSpec constraint) => constraint != null);
	}
}
