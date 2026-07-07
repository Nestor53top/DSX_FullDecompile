using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Versioning;

namespace NuGet;

internal static class PackageExtensions
{
	private const string TagsProperty = "Tags";

	private static readonly string[] _packagePropertiesToSearch = new string[3] { "Id", "Description", "Tags" };

	public static bool IsReleaseVersion(this IPackageName packageMetadata)
	{
		return string.IsNullOrEmpty(packageMetadata.Version.SpecialVersion);
	}

	public static bool IsListed(this IPackage package)
	{
		if (!package.Listed)
		{
			return package.Published > Constants.Unpublished;
		}
		return true;
	}

	public static bool IsSatellitePackage(this IPackageMetadata package)
	{
		if (!string.IsNullOrEmpty(package.Language) && package.Id.EndsWith("." + package.Language, StringComparison.OrdinalIgnoreCase))
		{
			string corePackageId = package.Id.Substring(0, package.Id.Length - package.Language.Length - 1);
			return package.DependencySets.SelectMany((PackageDependencySet s) => s.Dependencies).Any((PackageDependency d) => d.Id.Equals(corePackageId, StringComparison.OrdinalIgnoreCase) && d.VersionSpec != null && d.VersionSpec.MaxVersion == d.VersionSpec.MinVersion && d.VersionSpec.IsMaxInclusive && d.VersionSpec.IsMinInclusive);
		}
		return false;
	}

	public static bool IsEmptyFolder(this IPackageFile packageFile)
	{
		if (packageFile != null)
		{
			return "_._".Equals(Path.GetFileName(packageFile.Path), StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public static IEnumerable<IPackage> FindByVersion(this IEnumerable<IPackage> source, IVersionSpec versionSpec)
	{
		if (versionSpec == null)
		{
			throw new ArgumentNullException("versionSpec");
		}
		return source.Where(versionSpec.ToDelegate());
	}

	public static IEnumerable<IPackageFile> GetFiles(this IPackage package, string directory)
	{
		char directorySeparatorChar = Path.DirectorySeparatorChar;
		string folderPrefix = directory + directorySeparatorChar;
		return from file in package.GetFiles()
			where file.Path.StartsWith(folderPrefix, StringComparison.OrdinalIgnoreCase)
			select file;
	}

	public static IEnumerable<IPackageFile> GetContentFiles(this IPackage package)
	{
		return package.GetFiles(Constants.ContentDirectory);
	}

	public static IEnumerable<IPackageFile> GetToolFiles(this IPackage package)
	{
		return package.GetFiles(Constants.ToolsDirectory);
	}

	public static IEnumerable<IPackageFile> GetBuildFiles(this IPackage package)
	{
		string targetsFile = package.Id + ".targets";
		string propsFile = package.Id + ".props";
		return from p in package.GetFiles(Constants.BuildDirectory)
			where targetsFile.Equals(p.EffectivePath, StringComparison.OrdinalIgnoreCase) || propsFile.Equals(p.EffectivePath, StringComparison.OrdinalIgnoreCase)
			select p;
	}

	public static IEnumerable<IPackageFile> GetLibFiles(this IPackage package)
	{
		return package.GetFiles(Constants.LibDirectory);
	}

	public static bool HasFileWithNullTargetFramework(this IPackage package)
	{
		return package.GetContentFiles().Concat(package.GetLibFiles()).Any((IPackageFile file) => file.TargetFramework == null);
	}

	public static IEnumerable<IPackageFile> GetSatelliteFiles(this IPackage package)
	{
		if (string.IsNullOrEmpty(package.Language))
		{
			return Enumerable.Empty<IPackageFile>();
		}
		return from file in package.GetLibFiles()
			where Enumerable.Contains<string>(Path.GetDirectoryName(file.Path).Split(new char[1] { Path.DirectorySeparatorChar }), package.Language, StringComparer.OrdinalIgnoreCase)
			select file;
	}

	public static IEnumerable<PackageIssue> Validate(this IPackage package, IEnumerable<IPackageRule> rules)
	{
		if (package == null)
		{
			return null;
		}
		if (rules == null)
		{
			throw new ArgumentNullException("rules");
		}
		return rules.Where((IPackageRule r) => r != null).SelectMany((IPackageRule r) => r.Validate(package));
	}

	public static string GetHash(this IPackage package, string hashAlgorithm)
	{
		return package.GetHash(new CryptoHashProvider(hashAlgorithm));
	}

	public static string GetHash(this IPackage package, IHashProvider hashProvider)
	{
		using Stream stream = package.GetStream();
		return Convert.ToBase64String(hashProvider.CalculateHash(stream));
	}

	public static bool HasProjectContent(this IPackage package)
	{
		if (!package.FrameworkAssemblies.Any() && !package.AssemblyReferences.Any() && !package.GetContentFiles().Any() && !package.GetLibFiles().Any())
		{
			return package.GetBuildFiles().Any();
		}
		return true;
	}

	public static IEnumerable<PackageDependency> GetCompatiblePackageDependencies(this IPackageMetadata package, FrameworkName targetFramework)
	{
		IEnumerable<PackageDependencySet> compatibleItems;
		if (targetFramework == null)
		{
			compatibleItems = package.DependencySets;
		}
		else if (!VersionUtility.TryGetCompatibleItems(targetFramework, package.DependencySets, out compatibleItems))
		{
			compatibleItems = new PackageDependencySet[0];
		}
		return compatibleItems.SelectMany((PackageDependencySet d) => d.Dependencies);
	}

	public static string GetFullName(this IPackageName package)
	{
		return package.Id + " " + package.Version;
	}

	public static IEnumerable<IPackage> AsCollapsed(this IEnumerable<IPackage> source)
	{
		return source.DistinctLast(PackageEqualityComparer.Id, PackageComparer.Version);
	}

	internal static IEnumerable<IPackage> CollapseById(this IEnumerable<IPackage> source)
	{
		return from g in source.GroupBy<IPackage, string>((IPackage p) => p.Id, StringComparer.OrdinalIgnoreCase)
			select g.OrderByDescending((IPackage p) => p.Version).First();
	}

	public static IEnumerable<IPackage> FilterByPrerelease(this IEnumerable<IPackage> packages, bool allowPrerelease)
	{
		if (packages == null)
		{
			return null;
		}
		if (!allowPrerelease)
		{
			packages = packages.Where((IPackage p) => p.IsReleaseVersion());
		}
		return packages;
	}

	public static IQueryable<T> Find<T>(this IQueryable<T> packages, string searchText) where T : IPackage
	{
		return packages.Find(_packagePropertiesToSearch, searchText);
	}

	public static IQueryable<T> Find<T>(this IQueryable<T> packages, IEnumerable<string> propertiesToSearch, string searchText) where T : IPackage
	{
		if (propertiesToSearch.IsEmpty())
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "propertiesToSearch");
		}
		if (string.IsNullOrEmpty(searchText))
		{
			return packages;
		}
		return packages.Find(propertiesToSearch, searchText.Split(new char[0]));
	}

	private static IQueryable<T> Find<T>(this IQueryable<T> packages, IEnumerable<string> propertiesToSearch, IEnumerable<string> searchTerms) where T : IPackage
	{
		if (!searchTerms.Any())
		{
			return packages;
		}
		IEnumerable<string> enumerable = searchTerms.Where((string s) => s != null);
		if (!enumerable.Any())
		{
			return packages;
		}
		return packages.Where(BuildSearchExpression<T>(propertiesToSearch, enumerable));
	}

	public static IQueryable<T> FindLatestVersion<T>(this IQueryable<T> packages) where T : IPackage
	{
		return packages.Where((T p) => ((IPackage)p).IsLatestVersion);
	}

	private static Expression<Func<T, bool>> BuildSearchExpression<T>(IEnumerable<string> propertiesToSearch, IEnumerable<string> searchTerms) where T : IPackage
	{
		ParameterExpression parameterExpression = Expression.Parameter(typeof(IPackageMetadata));
		return Expression.Lambda<Func<T, bool>>((from term in searchTerms
			from property in propertiesToSearch
			select BuildExpressionForTerm(parameterExpression, term, property)).Aggregate(Expression.OrElse), new ParameterExpression[1] { parameterExpression });
	}

	private static Expression BuildExpressionForTerm(ParameterExpression packageParameterExpression, string term, string propertyName)
	{
		if (propertyName.Equals("Tags", StringComparison.OrdinalIgnoreCase))
		{
			term = " " + term + " ";
		}
		MethodInfo method = typeof(string).GetMethod("Contains", new Type[1] { typeof(string) });
		MethodInfo method2 = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
		MemberExpression memberExpression = ((!propertyName.Equals("Id", StringComparison.OrdinalIgnoreCase)) ? Expression.Property(packageParameterExpression, propertyName) : Expression.Property(Expression.TypeAs(packageParameterExpression, typeof(IPackageName)), propertyName));
		MethodCallExpression instance = Expression.Call(memberExpression, method2);
		return Expression.AndAlso(Expression.NotEqual(memberExpression, Expression.Constant(null)), Expression.Call(instance, method, Expression.Constant(term.ToLower())));
	}
}
