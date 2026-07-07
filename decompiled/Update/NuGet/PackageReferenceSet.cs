using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Versioning;

namespace NuGet;

internal class PackageReferenceSet : IFrameworkTargetable
{
	private readonly FrameworkName _targetFramework;

	private readonly ICollection<string> _references;

	public ICollection<string> References => _references;

	public FrameworkName TargetFramework => _targetFramework;

	public IEnumerable<FrameworkName> SupportedFrameworks
	{
		get
		{
			if (!(TargetFramework == null))
			{
				yield return TargetFramework;
			}
		}
	}

	public PackageReferenceSet(FrameworkName targetFramework, IEnumerable<string> references)
	{
		if (references == null)
		{
			throw new ArgumentNullException("references");
		}
		_targetFramework = targetFramework;
		_references = new ReadOnlyCollection<string>(references.ToList());
	}

	public PackageReferenceSet(ManifestReferenceSet manifestReferenceSet)
	{
		if (manifestReferenceSet == null)
		{
			throw new ArgumentNullException("manifestReferenceSet");
		}
		if (!string.IsNullOrEmpty(manifestReferenceSet.TargetFramework))
		{
			_targetFramework = VersionUtility.ParseFrameworkName(manifestReferenceSet.TargetFramework);
		}
		_references = new ReadOnlyHashSet<string>(manifestReferenceSet.References.Select((ManifestReference r) => r.File), StringComparer.OrdinalIgnoreCase);
	}
}
