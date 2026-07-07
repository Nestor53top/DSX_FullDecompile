using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace NuGet;

internal class AggregateRepository : PackageRepositoryBase, IPackageLookup, IPackageRepository, IDependencyResolver, IServiceBasedRepository, ICloneableRepository, IOperationAwareRepository
{
	private readonly ConcurrentBag<IPackageRepository> _failingRepositories = new ConcurrentBag<IPackageRepository>();

	private readonly IEnumerable<IPackageRepository> _repositories;

	private readonly Lazy<bool> _supportsPrereleasePackages;

	private const string SourceValue = "(Aggregate source)";

	private ILogger _logger;

	public override string Source => "(Aggregate source)";

	public ILogger Logger
	{
		get
		{
			return _logger ?? NullLogger.Instance;
		}
		set
		{
			_logger = value;
		}
	}

	public bool ResolveDependenciesVertically { get; set; }

	public bool IgnoreFailingRepositories { get; set; }

	public IEnumerable<IPackageRepository> Repositories => _repositories;

	public override bool SupportsPrereleasePackages => _supportsPrereleasePackages.Value;

	public AggregateRepository(IEnumerable<IPackageRepository> repositories)
	{
		AggregateRepository aggregateRepository = this;
		if (repositories == null)
		{
			throw new ArgumentNullException("repositories");
		}
		_repositories = Flatten(repositories);
		Func<IPackageRepository, bool> supportsPrereleasePackages = Wrap((IPackageRepository r) => r.SupportsPrereleasePackages, defaultValue: true);
		_supportsPrereleasePackages = new Lazy<bool>(() => aggregateRepository._repositories.All(supportsPrereleasePackages));
		IgnoreFailingRepositories = true;
	}

	public AggregateRepository(IPackageRepositoryFactory repositoryFactory, IEnumerable<string> packageSources, bool ignoreFailingRepositories)
	{
		AggregateRepository aggregateRepository = this;
		IgnoreFailingRepositories = ignoreFailingRepositories;
		Func<string, IPackageRepository> createRepository = repositoryFactory.CreateRepository;
		if (ignoreFailingRepositories)
		{
			createRepository = delegate(string source)
			{
				try
				{
					return repositoryFactory.CreateRepository(source);
				}
				catch
				{
					return (IPackageRepository)null;
				}
			};
		}
		_repositories = (from source in packageSources
			let repository = createRepository(source)
			where repository != null
			select repository).ToArray();
		Func<IPackageRepository, bool> supportsPrereleasePackages = Wrap((IPackageRepository r) => r.SupportsPrereleasePackages, defaultValue: true);
		_supportsPrereleasePackages = new Lazy<bool>(() => aggregateRepository._repositories.All(supportsPrereleasePackages));
	}

	public override IQueryable<IPackage> GetPackages()
	{
		IQueryable<IPackage> defaultValue = Enumerable.Empty<IPackage>().AsQueryable();
		Func<IPackageRepository, IQueryable<IPackage>> selector = Wrap((IPackageRepository r) => r.GetPackages(), defaultValue);
		return CreateAggregateQuery(Repositories.Select(selector));
	}

	public IPackage FindPackage(string packageId, SemanticVersion version)
	{
		Func<IPackageRepository, IPackage> selector = Wrap((IPackageRepository r) => r.FindPackage(packageId, version));
		return Repositories.Select(selector).FirstOrDefault((IPackage p) => p != null);
	}

	public bool Exists(string packageId, SemanticVersion version)
	{
		Func<IPackageRepository, bool> predicate = Wrap((IPackageRepository r) => r.Exists(packageId, version), defaultValue: false);
		return Repositories.Any(predicate);
	}

	public IPackage ResolveDependency(PackageDependency dependency, IPackageConstraintProvider constraintProvider, bool allowPrereleaseVersions, bool preferListedPackages, DependencyVersion dependencyVersion)
	{
		if (ResolveDependenciesVertically)
		{
			Func<IPackageRepository, IPackage> resolveDependency = Wrap((IPackageRepository r) => DependencyResolveUtility.ResolveDependency(r, dependency, constraintProvider, allowPrereleaseVersions, preferListedPackages, dependencyVersion));
			return Repositories.Select((IPackageRepository r) => Task.Factory.StartNew(() => resolveDependency(r))).ToArray().WhenAny((IPackage package) => package != null);
		}
		return DependencyResolveUtility.ResolveDependencyCore(this, dependency, constraintProvider, allowPrereleaseVersions, preferListedPackages, dependencyVersion);
	}

	private Func<IPackageRepository, T> Wrap<T>(Func<IPackageRepository, T> factory, T defaultValue = default(T))
	{
		if (IgnoreFailingRepositories)
		{
			return delegate(IPackageRepository repository)
			{
				if (_failingRepositories.Contains(repository))
				{
					return defaultValue;
				}
				try
				{
					return factory(repository);
				}
				catch (Exception ex)
				{
					LogRepository(repository, ex);
					return defaultValue;
				}
			};
		}
		return factory;
	}

	public void LogRepository(IPackageRepository repository, Exception ex)
	{
		_failingRepositories.Add(repository);
		Logger.Log(MessageLevel.Warning, ExceptionUtility.Unwrap(ex).Message);
	}

	public IQueryable<IPackage> Search(string searchTerm, IEnumerable<string> targetFrameworks, bool allowPrereleaseVersions, bool includeDelisted)
	{
		return CreateAggregateQuery(Repositories.Select((IPackageRepository r) => r.Search(searchTerm, targetFrameworks, allowPrereleaseVersions, includeDelisted)));
	}

	public IPackageRepository Clone()
	{
		return new AggregateRepository(Repositories.Select(PackageRepositoryExtensions.Clone));
	}

	private AggregateQuery<IPackage> CreateAggregateQuery(IEnumerable<IQueryable<IPackage>> queries)
	{
		return new AggregateQuery<IPackage>(queries, PackageEqualityComparer.IdAndVersion, Logger, IgnoreFailingRepositories);
	}

	internal static IEnumerable<IPackageRepository> Flatten(IEnumerable<IPackageRepository> repositories)
	{
		return repositories.SelectMany((IPackageRepository repository) => (!(repository is AggregateRepository aggregateRepository)) ? ((IEnumerable<IPackageRepository>)new IPackageRepository[1] { repository }) : ((IEnumerable<IPackageRepository>)aggregateRepository.Repositories.ToArray()));
	}

	public IEnumerable<IPackage> FindPackagesById(string packageId)
	{
		Task<IEnumerable<IPackage>>[] array = _repositories.Select((IPackageRepository p) => Task.Factory.StartNew((object state) => p.FindPackagesById(packageId), p)).ToArray();
		try
		{
			Task[] tasks = array;
			Task.WaitAll(tasks);
		}
		catch (AggregateException)
		{
			if (!IgnoreFailingRepositories)
			{
				throw;
			}
		}
		List<IPackage> list = new List<IPackage>();
		Task<IEnumerable<IPackage>>[] array2 = array;
		foreach (Task<IEnumerable<IPackage>> task in array2)
		{
			if (task.IsFaulted)
			{
				LogRepository((IPackageRepository)task.AsyncState, task.Exception);
			}
			else if (task.Result != null)
			{
				list.AddRange(task.Result);
			}
		}
		return list;
	}

	public IEnumerable<IPackage> GetUpdates(IEnumerable<IPackageName> packages, bool includePrerelease, bool includeAllVersions, IEnumerable<FrameworkName> targetFrameworks, IEnumerable<IVersionSpec> versionConstraints)
	{
		Task<IEnumerable<IPackage>>[] array = _repositories.Select((IPackageRepository p) => Task.Factory.StartNew((object state) => p.GetUpdates(packages, includePrerelease, includeAllVersions, targetFrameworks, versionConstraints), p)).ToArray();
		try
		{
			Task[] tasks = array;
			Task.WaitAll(tasks);
		}
		catch (AggregateException)
		{
			if (!IgnoreFailingRepositories)
			{
				throw;
			}
		}
		HashSet<IPackage> hashSet = new HashSet<IPackage>(PackageEqualityComparer.IdAndVersion);
		Task<IEnumerable<IPackage>>[] array2 = array;
		foreach (Task<IEnumerable<IPackage>> task in array2)
		{
			if (task.IsFaulted)
			{
				LogRepository((IPackageRepository)task.AsyncState, task.Exception);
			}
			else if (task.Result != null)
			{
				hashSet.AddRange(task.Result);
			}
		}
		if (includeAllVersions)
		{
			return hashSet.OrderBy<IPackage, string>((IPackage p) => p.Id, StringComparer.OrdinalIgnoreCase).ThenBy((IPackage p) => p.Version);
		}
		return hashSet.CollapseById();
	}

	public IDisposable StartOperation(string operation, string mainPackageId, string mainPackageVersion)
	{
		return DisposableAction.All(Repositories.Select((IPackageRepository r) => r.StartOperation(operation, mainPackageId, mainPackageVersion)));
	}

	public static IPackageRepository Create(IPackageRepositoryFactory factory, IList<PackageSource> sources, bool ignoreFailingRepositories)
	{
		if (sources.Count == 0)
		{
			return null;
		}
		if (sources.Count == 1)
		{
			return factory.CreateRepository(sources[0].Source);
		}
		Func<string, IPackageRepository> createRepository = factory.CreateRepository;
		if (ignoreFailingRepositories)
		{
			createRepository = delegate(string source)
			{
				try
				{
					return factory.CreateRepository(source);
				}
				catch (InvalidOperationException)
				{
					return (IPackageRepository)null;
				}
			};
		}
		return new AggregateRepository(from source in sources
			let repository = createRepository(source.Source)
			where repository != null
			select repository)
		{
			IgnoreFailingRepositories = ignoreFailingRepositories
		};
	}
}
