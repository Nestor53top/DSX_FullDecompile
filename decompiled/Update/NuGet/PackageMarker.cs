using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGet;

internal sealed class PackageMarker : IPackageRepository, IDependentsResolver, IPackageLookup
{
	internal enum VisitedState
	{
		Processing,
		Completed
	}

	private readonly Dictionary<string, Dictionary<IPackage, VisitedState>> _visited = new Dictionary<string, Dictionary<IPackage, VisitedState>>(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<IPackage, HashSet<IPackage>> _dependents = new Dictionary<IPackage, HashSet<IPackage>>(PackageEqualityComparer.IdAndVersion);

	public string Source => string.Empty;

	public PackageSaveModes PackageSaveMode
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public bool SupportsPrereleasePackages => true;

	public IEnumerable<IPackage> Packages => _visited.Values.SelectMany((Dictionary<IPackage, VisitedState> p) => p.Keys);

	public bool Contains(IPackage package)
	{
		return GetLookup(package.Id, createEntry: true)?.ContainsKey(package) ?? false;
	}

	public void MarkProcessing(IPackage package)
	{
		GetLookup(package.Id, createEntry: true)[package] = VisitedState.Processing;
	}

	public void MarkVisited(IPackage package)
	{
		GetLookup(package.Id, createEntry: true)[package] = VisitedState.Completed;
	}

	public bool IsVersionCycle(string packageId)
	{
		return GetLookup(packageId)?.Values.Any((VisitedState state) => state == VisitedState.Processing) ?? false;
	}

	public bool IsVisited(IPackage package)
	{
		Dictionary<IPackage, VisitedState> lookup = GetLookup(package.Id);
		if (lookup != null && lookup.TryGetValue(package, out var value))
		{
			return value == VisitedState.Completed;
		}
		return false;
	}

	public bool IsCycle(IPackage package)
	{
		Dictionary<IPackage, VisitedState> lookup = GetLookup(package.Id);
		if (lookup != null && lookup.TryGetValue(package, out var value))
		{
			return value == VisitedState.Processing;
		}
		return false;
	}

	public void Clear()
	{
		_visited.Clear();
		_dependents.Clear();
	}

	IQueryable<IPackage> IPackageRepository.GetPackages()
	{
		return Packages.Where(IsVisited).AsQueryable();
	}

	void IPackageRepository.AddPackage(IPackage package)
	{
		throw new NotSupportedException();
	}

	void IPackageRepository.RemovePackage(IPackage package)
	{
		throw new NotSupportedException();
	}

	IEnumerable<IPackage> IDependentsResolver.GetDependents(IPackage package)
	{
		if (_dependents.TryGetValue(package, out var value))
		{
			return value;
		}
		return Enumerable.Empty<IPackage>();
	}

	public void AddDependent(IPackage package, IPackage dependency)
	{
		if (!_dependents.TryGetValue(dependency, out var value))
		{
			value = new HashSet<IPackage>(PackageEqualityComparer.IdAndVersion);
			_dependents.Add(dependency, value);
		}
		value.Add(package);
	}

	private Dictionary<IPackage, VisitedState> GetLookup(string packageId, bool createEntry = false)
	{
		if (!_visited.TryGetValue(packageId, out var value) && createEntry)
		{
			value = new Dictionary<IPackage, VisitedState>(PackageEqualityComparer.IdAndVersion);
			_visited[packageId] = value;
		}
		return value;
	}

	public bool Exists(string packageId, SemanticVersion version)
	{
		return FindPackage(packageId, version) != null;
	}

	public IPackage FindPackage(string packageId, SemanticVersion version)
	{
		return (from p in FindPackagesById(packageId)
			where p.Version.Equals(version)
			select p).FirstOrDefault();
	}

	public IEnumerable<IPackage> FindPackagesById(string packageId)
	{
		Dictionary<IPackage, VisitedState> packages = GetLookup(packageId);
		if (packages != null)
		{
			return packages.Keys.Where((IPackage p) => packages[p] == VisitedState.Completed);
		}
		return Enumerable.Empty<IPackage>();
	}
}
