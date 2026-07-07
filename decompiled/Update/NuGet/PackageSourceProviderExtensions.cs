using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGet;

internal static class PackageSourceProviderExtensions
{
	public static AggregateRepository CreateAggregateRepository(this IPackageSourceProvider provider, IPackageRepositoryFactory factory, bool ignoreFailingRepositories)
	{
		return new AggregateRepository(factory, from s in provider.GetEnabledPackageSources()
			select s.Source, ignoreFailingRepositories);
	}

	public static IPackageRepository CreatePriorityPackageRepository(this IPackageSourceProvider provider, IPackageRepositoryFactory factory, IPackageRepository primaryRepository)
	{
		PackageSource[] array = (from s in provider.GetEnabledPackageSources()
			where !s.Source.Equals(primaryRepository.Source, StringComparison.OrdinalIgnoreCase)
			select s).ToArray();
		if (array.Length == 0)
		{
			return primaryRepository;
		}
		IPackageRepository secondaryRepository = AggregateRepository.Create(factory, array, ignoreFailingRepositories: true);
		return new PriorityPackageRepository(primaryRepository, secondaryRepository);
	}

	public static string ResolveSource(this IPackageSourceProvider provider, string value)
	{
		return (from source in provider.GetEnabledPackageSources()
			where source.Name.Equals(value, StringComparison.CurrentCultureIgnoreCase) || source.Source.Equals(value, StringComparison.OrdinalIgnoreCase)
			select source.Source).FirstOrDefault() ?? value;
	}

	public static IEnumerable<PackageSource> GetEnabledPackageSources(this IPackageSourceProvider provider)
	{
		return from p in provider.LoadPackageSources()
			where p.IsEnabled
			select p;
	}
}
