using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGet;

internal class PriorityPackageRepository : PackageRepositoryBase, IPackageLookup, IPackageRepository, IOperationAwareRepository
{
	private readonly IPackageRepository _primaryRepository;

	private readonly IPackageRepository _secondaryRepository;

	internal IPackageRepository PrimaryRepository => _primaryRepository;

	internal IPackageRepository SecondaryRepository => _secondaryRepository;

	public override string Source => _primaryRepository.Source;

	public override bool SupportsPrereleasePackages => _primaryRepository.SupportsPrereleasePackages;

	public PriorityPackageRepository(IPackageRepository primaryRepository, IPackageRepository secondaryRepository)
	{
		if (primaryRepository == null)
		{
			throw new ArgumentNullException("primaryRepository");
		}
		if (secondaryRepository == null)
		{
			throw new ArgumentNullException("secondaryRepository");
		}
		_primaryRepository = primaryRepository;
		_secondaryRepository = secondaryRepository;
	}

	public override IQueryable<IPackage> GetPackages()
	{
		return _primaryRepository.GetPackages();
	}

	public bool Exists(string packageId, SemanticVersion version)
	{
		bool flag = _primaryRepository.Exists(packageId, version);
		if (!flag)
		{
			flag = _secondaryRepository.Exists(packageId, version);
		}
		return flag;
	}

	public IPackage FindPackage(string packageId, SemanticVersion version)
	{
		return _primaryRepository.FindPackage(packageId, version) ?? _secondaryRepository.FindPackage(packageId, version);
	}

	public IEnumerable<IPackage> FindPackagesById(string packageId)
	{
		IEnumerable<IPackage> enumerable = _primaryRepository.FindPackagesById(packageId);
		if (enumerable.IsEmpty())
		{
			enumerable = _secondaryRepository.FindPackagesById(packageId);
		}
		return enumerable.Distinct();
	}

	public IDisposable StartOperation(string operation, string mainPackageId, string mainPackageVersion)
	{
		return DisposableAction.All(_primaryRepository.StartOperation(operation, mainPackageId, mainPackageVersion), _secondaryRepository.StartOperation(operation, mainPackageId, mainPackageVersion));
	}
}
