using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.Versioning;
using NuGet.Resources;

namespace NuGet;

internal class ZipPackage : LocalPackage
{
	private const string CacheKeyFormat = "NUGET_ZIP_PACKAGE_{0}_{1}{2}";

	private const string AssembliesCacheKey = "ASSEMBLIES";

	private const string FilesCacheKey = "FILES";

	private readonly bool _enableCaching;

	private static readonly TimeSpan CacheTimeout = TimeSpan.FromSeconds(15.0);

	private static readonly string[] ExcludePaths = new string[2] { "_rels", "package" };

	private readonly Func<Stream> _streamFactory;

	public ZipPackage(string filePath)
		: this(filePath, enableCaching: false)
	{
	}

	public ZipPackage(Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		_enableCaching = false;
		_streamFactory = stream.ToStreamFactory();
		EnsureManifest();
	}

	private ZipPackage(string filePath, bool enableCaching)
	{
		if (string.IsNullOrEmpty(filePath))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "filePath");
		}
		_enableCaching = enableCaching;
		_streamFactory = () => File.OpenRead(filePath);
		EnsureManifest();
	}

	internal ZipPackage(Func<Stream> streamFactory, bool enableCaching)
	{
		if (streamFactory == null)
		{
			throw new ArgumentNullException("streamFactory");
		}
		_enableCaching = enableCaching;
		_streamFactory = streamFactory;
		EnsureManifest();
	}

	public override Stream GetStream()
	{
		return _streamFactory();
	}

	public override IEnumerable<FrameworkName> GetSupportedFrameworks()
	{
		IEnumerable<FrameworkName> second;
		if (_enableCaching && MemoryCache.Instance.TryGetValue<IEnumerable<IPackageFile>>(GetFilesCacheKey(), out var value))
		{
			second = value.Select((IPackageFile c) => c.TargetFramework);
		}
		else
		{
			using Stream stream = _streamFactory();
			second = from part in (IEnumerable<PackagePart>)Package.Open(stream).GetParts()
				where IsPackageFile(part)
				select VersionUtility.ParseFrameworkNameFromFilePath(UriUtility.GetPath(part.Uri), out var _);
		}
		return (from f in base.GetSupportedFrameworks().Concat(second)
			where f != null
			select f).Distinct();
	}

	protected override IEnumerable<IPackageAssemblyReference> GetAssemblyReferencesCore()
	{
		if (_enableCaching)
		{
			return MemoryCache.Instance.GetOrAdd(GetAssembliesCacheKey(), GetAssembliesNoCache, CacheTimeout);
		}
		return GetAssembliesNoCache();
	}

	protected override IEnumerable<IPackageFile> GetFilesBase()
	{
		if (_enableCaching)
		{
			return MemoryCache.Instance.GetOrAdd(GetFilesCacheKey(), GetFilesNoCache, CacheTimeout);
		}
		return GetFilesNoCache();
	}

	private List<IPackageAssemblyReference> GetAssembliesNoCache()
	{
		return (from file in GetFiles()
			where LocalPackage.IsAssemblyReference(file.Path)
			select file).Select((Func<IPackageFile, IPackageAssemblyReference>)((IPackageFile file) => new ZipPackageAssemblyReference(file))).ToList();
	}

	private List<IPackageFile> GetFilesNoCache()
	{
		using Stream stream = _streamFactory();
		return ((IEnumerable<PackagePart>)Package.Open(stream).GetParts()).Where((PackagePart part) => IsPackageFile(part)).Select((Func<PackagePart, IPackageFile>)((PackagePart part) => new ZipPackageFile(part))).ToList();
	}

	private void EnsureManifest()
	{
		using Stream stream = _streamFactory();
		Package obj = Package.Open(stream);
		PackageRelationship val = ((IEnumerable<PackageRelationship>)obj.GetRelationshipsByType("http://schemas.microsoft.com/packaging/2010/07/manifest")).SingleOrDefault();
		if (val == null)
		{
			throw new InvalidOperationException(NuGetResources.PackageDoesNotContainManifest);
		}
		using Stream manifestStream = (obj.GetPart(val.TargetUri) ?? throw new InvalidOperationException(NuGetResources.PackageDoesNotContainManifest)).GetStream();
		ReadManifest(manifestStream);
	}

	private string GetFilesCacheKey()
	{
		return string.Format(CultureInfo.InvariantCulture, "NUGET_ZIP_PACKAGE_{0}_{1}{2}", new object[3] { "FILES", base.Id, base.Version });
	}

	private string GetAssembliesCacheKey()
	{
		return string.Format(CultureInfo.InvariantCulture, "NUGET_ZIP_PACKAGE_{0}_{1}{2}", new object[3] { "ASSEMBLIES", base.Id, base.Version });
	}

	internal static bool IsPackageFile(PackagePart part)
	{
		string path = UriUtility.GetPath(part.Uri);
		string directory = Path.GetDirectoryName(path);
		if (!ExcludePaths.Any((string p) => directory.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
		{
			return !PackageHelper.IsManifest(path);
		}
		return false;
	}

	internal static void ClearCache(IPackage package)
	{
		if (package is ZipPackage zipPackage)
		{
			MemoryCache.Instance.Remove(zipPackage.GetAssembliesCacheKey());
			MemoryCache.Instance.Remove(zipPackage.GetFilesCacheKey());
		}
	}
}
