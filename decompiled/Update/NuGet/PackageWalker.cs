using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using NuGet.Resources;
using NuGet.V3Interop;

namespace NuGet;

internal abstract class PackageWalker
{
	private readonly Dictionary<IPackage, PackageWalkInfo> _packageLookup = new Dictionary<IPackage, PackageWalkInfo>();

	private readonly FrameworkName _targetFramework;

	protected FrameworkName TargetFramework => _targetFramework;

	protected virtual bool RaiseErrorOnCycle => true;

	protected virtual bool SkipDependencyResolveError => false;

	protected virtual bool IgnoreDependencies => false;

	protected virtual bool AllowPrereleaseVersions => true;

	public DependencyVersion DependencyVersion { get; set; }

	protected PackageMarker Marker { get; private set; }

	protected virtual bool IgnoreWalkInfo => false;

	protected PackageWalker()
		: this(null)
	{
	}

	protected PackageWalker(FrameworkName targetFramework)
	{
		_targetFramework = targetFramework;
		Marker = new PackageMarker();
		DependencyVersion = DependencyVersion.Lowest;
	}

	public void Walk(IPackage package)
	{
		CheckPackageMinClientVersion(package);
		if (Marker.IsVisited(package))
		{
			ProcessPackageTarget(package);
			return;
		}
		OnBeforePackageWalk(package);
		Marker.MarkProcessing(package);
		if (!IgnoreDependencies)
		{
			foreach (PackageDependency compatiblePackageDependency in package.GetCompatiblePackageDependencies(TargetFramework))
			{
				IPackage package2 = DependencyResolveUtility.ResolveDependency(Marker, compatiblePackageDependency, null, AllowPrereleaseVersions, preferListedPackages: false, DependencyVersion);
				if (package2 == null)
				{
					package2 = ResolveDependency(compatiblePackageDependency);
				}
				if (package2 == null)
				{
					OnDependencyResolveError(compatiblePackageDependency);
					if (!SkipDependencyResolveError)
					{
						return;
					}
					continue;
				}
				if (!IgnoreWalkInfo)
				{
					GetPackageInfo(package2).Parent = package;
				}
				Marker.AddDependent(package, package2);
				if (!OnAfterResolveDependency(package, package2))
				{
					continue;
				}
				if (Marker.IsCycle(package2) || Marker.IsVersionCycle(package2.Id))
				{
					if (RaiseErrorOnCycle)
					{
						List<IPackage> list = Marker.Packages.ToList();
						list.Add(package2);
						throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.CircularDependencyDetected, new object[1] { string.Join(" => ", list.Select((IPackage p) => p.GetFullName())) }));
					}
				}
				else
				{
					Walk(package2);
				}
			}
		}
		Marker.MarkVisited(package);
		ProcessPackageTarget(package);
		OnAfterPackageWalk(package);
	}

	private static void CheckPackageMinClientVersion(IPackage package)
	{
		if (Constants.NuGetVersion < package.MinClientVersion)
		{
			throw new NuGetVersionNotSatisfiedException(string.Format(CultureInfo.CurrentCulture, NuGetResources.PackageMinVersionNotSatisfied, new object[3]
			{
				package.GetFullName(),
				package.MinClientVersion,
				Constants.NuGetVersion
			}));
		}
	}

	private void ProcessPackageTarget(IPackage package)
	{
		if (IgnoreWalkInfo)
		{
			return;
		}
		PackageWalkInfo packageInfo = GetPackageInfo(package);
		if (packageInfo.Parent == null)
		{
			return;
		}
		PackageWalkInfo packageInfo2 = GetPackageInfo(packageInfo.Parent);
		if (packageInfo2.InitialTarget == PackageTargets.None)
		{
			packageInfo2.Target |= packageInfo.Target;
			if (packageInfo2.Target == PackageTargets.All)
			{
				throw new InvalidOperationException(NuGetResources.DependencyOnlyCannotMixDependencies);
			}
		}
		if (packageInfo2.Target != PackageTargets.External || !packageInfo.Target.HasFlag(PackageTargets.Project))
		{
			return;
		}
		throw new InvalidOperationException(NuGetResources.ExternalPackagesCannotDependOnProjectLevelPackages);
	}

	protected virtual bool OnAfterResolveDependency(IPackage package, IPackage dependency)
	{
		return true;
	}

	protected virtual void OnBeforePackageWalk(IPackage package)
	{
	}

	protected virtual void OnAfterPackageWalk(IPackage package)
	{
	}

	protected virtual void OnDependencyResolveError(PackageDependency dependency)
	{
	}

	protected abstract IPackage ResolveDependency(PackageDependency dependency);

	protected internal PackageWalkInfo GetPackageInfo(IPackage package)
	{
		if (!_packageLookup.TryGetValue(package, out var value))
		{
			value = new PackageWalkInfo(GetPackageTarget(package));
			_packageLookup.Add(package, value);
		}
		return value;
	}

	protected static PackageTargets GetPackageTarget(IPackage package)
	{
		if (package is IV3PackageMetadata iV3PackageMetadata)
		{
			return iV3PackageMetadata.PackageTarget;
		}
		if (package.HasProjectContent())
		{
			return PackageTargets.Project;
		}
		if (IsDependencyOnly(package))
		{
			return PackageTargets.None;
		}
		return PackageTargets.External;
	}

	private static bool IsDependencyOnly(IPackage package)
	{
		if (!package.GetFiles().Any((IPackageFile f) => f.Path.StartsWith("tools\\", StringComparison.OrdinalIgnoreCase)))
		{
			return package.DependencySets.SelectMany((PackageDependencySet d) => d.Dependencies).Any();
		}
		return false;
	}
}
