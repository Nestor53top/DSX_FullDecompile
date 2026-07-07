using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using NuGet.Resources;

namespace NuGet;

internal class PackageBuilder : IPackageBuilder, IPackageMetadata, IPackageName
{
	private const string DefaultContentType = "application/octet";

	internal const string ManifestRelationType = "manifest";

	private readonly bool _includeEmptyDirectories;

	public string Id { get; set; }

	public SemanticVersion Version { get; set; }

	public string Title { get; set; }

	public ISet<string> Authors { get; private set; }

	public ISet<string> Owners { get; private set; }

	public Uri IconUrl { get; set; }

	public Uri LicenseUrl { get; set; }

	public Uri ProjectUrl { get; set; }

	public bool RequireLicenseAcceptance { get; set; }

	public bool DevelopmentDependency { get; set; }

	public string Description { get; set; }

	public string Summary { get; set; }

	public string ReleaseNotes { get; set; }

	public string Language { get; set; }

	public ISet<string> Tags { get; private set; }

	public Dictionary<string, string> Properties { get; private set; }

	public string Copyright { get; set; }

	public Collection<PackageDependencySet> DependencySets { get; private set; }

	public Collection<IPackageFile> Files { get; private set; }

	public Collection<FrameworkAssemblyReference> FrameworkReferences { get; private set; }

	public ICollection<PackageReferenceSet> PackageAssemblyReferences { get; private set; }

	IEnumerable<string> IPackageMetadata.Authors => Authors;

	IEnumerable<string> IPackageMetadata.Owners => Owners;

	string IPackageMetadata.Tags => string.Join(" ", Tags);

	IEnumerable<PackageDependencySet> IPackageMetadata.DependencySets => DependencySets;

	IEnumerable<FrameworkAssemblyReference> IPackageMetadata.FrameworkAssemblies => FrameworkReferences;

	public Version MinClientVersion { get; set; }

	public PackageBuilder(string path, IPropertyProvider propertyProvider, bool includeEmptyDirectories)
		: this(path, Path.GetDirectoryName(path), propertyProvider, includeEmptyDirectories)
	{
	}

	public PackageBuilder(string path, string basePath, IPropertyProvider propertyProvider, bool includeEmptyDirectories)
		: this(includeEmptyDirectories)
	{
		using Stream stream = File.OpenRead(path);
		ReadManifest(stream, basePath, propertyProvider);
	}

	public PackageBuilder(Stream stream, string basePath)
		: this(stream, basePath, NullPropertyProvider.Instance)
	{
	}

	public PackageBuilder(Stream stream, string basePath, IPropertyProvider propertyProvider)
		: this()
	{
		ReadManifest(stream, basePath, propertyProvider);
	}

	public PackageBuilder()
		: this(includeEmptyDirectories: false)
	{
	}

	private PackageBuilder(bool includeEmptyDirectories)
	{
		_includeEmptyDirectories = includeEmptyDirectories;
		Files = new Collection<IPackageFile>();
		DependencySets = new Collection<PackageDependencySet>();
		FrameworkReferences = new Collection<FrameworkAssemblyReference>();
		PackageAssemblyReferences = new Collection<PackageReferenceSet>();
		Authors = new HashSet<string>();
		Owners = new HashSet<string>();
		Tags = new HashSet<string>();
		Properties = new Dictionary<string, string>();
	}

	public void Save(Stream stream)
	{
		PackageIdValidator.ValidatePackageId(Id);
		if (!Files.Any() && !DependencySets.SelectMany((PackageDependencySet d) => d.Dependencies).Any() && !FrameworkReferences.Any())
		{
			throw new InvalidOperationException(NuGetResources.CannotCreateEmptyPackage);
		}
		if (!ValidateSpecialVersionLength(Version))
		{
			throw new InvalidOperationException(NuGetResources.SemVerSpecialVersionTooLong);
		}
		ValidateDependencySets(Version, DependencySets);
		ValidateReferenceAssemblies(Files, PackageAssemblyReferences);
		Package val = Package.Open(stream, FileMode.Create);
		try
		{
			WriteManifest(val, DetermineMinimumSchemaVersion(Files));
			WriteFiles(val);
			val.PackageProperties.Creator = string.Join(",", Authors);
			val.PackageProperties.Description = Description;
			val.PackageProperties.Identifier = Id;
			val.PackageProperties.Version = Version.ToString();
			val.PackageProperties.Language = Language;
			val.PackageProperties.Keywords = ((IPackageMetadata)this).Tags;
			val.PackageProperties.Title = Title;
			val.PackageProperties.LastModifiedBy = CreatorInfo();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static string CreatorInfo()
	{
		List<string> list = new List<string>();
		Assembly assembly = typeof(PackageBuilder).Assembly;
		list.Add(assembly.FullName);
		list.Add(Environment.OSVersion.ToString());
		object[] customAttributes = assembly.GetCustomAttributes(typeof(TargetFrameworkAttribute), inherit: true);
		if (customAttributes.Length != 0)
		{
			TargetFrameworkAttribute targetFrameworkAttribute = (TargetFrameworkAttribute)customAttributes[0];
			list.Add(targetFrameworkAttribute.FrameworkDisplayName);
		}
		return string.Join(";", list);
	}

	private static int DetermineMinimumSchemaVersion(Collection<IPackageFile> Files)
	{
		if (HasXdtTransformFile(Files))
		{
			return 6;
		}
		if (RequiresV4TargetFrameworkSchema(Files))
		{
			return 4;
		}
		return 1;
	}

	private static bool RequiresV4TargetFrameworkSchema(ICollection<IPackageFile> files)
	{
		if (files.Any(delegate(IPackageFile f)
		{
			if (f.TargetFramework != null && f.TargetFramework != VersionUtility.UnsupportedFrameworkName)
			{
				string path = f.Path;
				string contentDirectory = Constants.ContentDirectory;
				char directorySeparatorChar = Path.DirectorySeparatorChar;
				if (!path.StartsWith(contentDirectory + directorySeparatorChar, StringComparison.OrdinalIgnoreCase))
				{
					string path2 = f.Path;
					string toolsDirectory = Constants.ToolsDirectory;
					directorySeparatorChar = Path.DirectorySeparatorChar;
					return path2.StartsWith(toolsDirectory + directorySeparatorChar, StringComparison.OrdinalIgnoreCase);
				}
				return true;
			}
			return false;
		}))
		{
			return true;
		}
		return files.Any(delegate(IPackageFile f)
		{
			if (f.TargetFramework != null)
			{
				string path = f.Path;
				string libDirectory = Constants.LibDirectory;
				char directorySeparatorChar = Path.DirectorySeparatorChar;
				if (path.StartsWith(libDirectory + directorySeparatorChar, StringComparison.OrdinalIgnoreCase))
				{
					return f.EffectivePath == "_._";
				}
			}
			return false;
		});
	}

	private static bool HasXdtTransformFile(ICollection<IPackageFile> contentFiles)
	{
		return contentFiles.Any(delegate(IPackageFile file)
		{
			if (file.Path != null)
			{
				string path = file.Path;
				string contentDirectory = Constants.ContentDirectory;
				char directorySeparatorChar = Path.DirectorySeparatorChar;
				if (path.StartsWith(contentDirectory + directorySeparatorChar, StringComparison.OrdinalIgnoreCase))
				{
					if (!file.Path.EndsWith(".install.xdt", StringComparison.OrdinalIgnoreCase))
					{
						return file.Path.EndsWith(".uninstall.xdt", StringComparison.OrdinalIgnoreCase);
					}
					return true;
				}
			}
			return false;
		});
	}

	internal static void ValidateDependencySets(SemanticVersion version, IEnumerable<PackageDependencySet> dependencies)
	{
		if (version == null)
		{
			return;
		}
		foreach (PackageDependency item in dependencies.SelectMany((PackageDependencySet s) => s.Dependencies))
		{
			PackageIdValidator.ValidatePackageId(item.Id);
		}
		if (string.IsNullOrEmpty(version.SpecialVersion))
		{
			PackageDependency packageDependency = dependencies.SelectMany((PackageDependencySet set) => set.Dependencies).FirstOrDefault(IsPrereleaseDependency);
			if (packageDependency != null)
			{
				throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, NuGetResources.Manifest_InvalidPrereleaseDependency, new object[1] { packageDependency.ToString() }));
			}
		}
	}

	internal static void ValidateReferenceAssemblies(IEnumerable<IPackageFile> files, IEnumerable<PackageReferenceSet> packageAssemblyReferences)
	{
		HashSet<string> hashSet = new HashSet<string>(from file in files
			where !string.IsNullOrEmpty(file.Path) && file.Path.StartsWith("lib", StringComparison.OrdinalIgnoreCase)
			select Path.GetFileName(file.Path), StringComparer.OrdinalIgnoreCase);
		foreach (string item in packageAssemblyReferences.SelectMany((PackageReferenceSet p) => p.References))
		{
			if (!hashSet.Contains(item) && !hashSet.Contains(item + ".dll") && !hashSet.Contains(item + ".exe") && !hashSet.Contains(item + ".winmd"))
			{
				throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, NuGetResources.Manifest_InvalidReference, new object[1] { item }));
			}
		}
	}

	private void ReadManifest(Stream stream, string basePath, IPropertyProvider propertyProvider)
	{
		Manifest manifest = Manifest.ReadFrom(stream, propertyProvider, validateSchema: true);
		Populate(manifest.Metadata);
		if (basePath != null)
		{
			if (manifest.Files == null)
			{
				AddFiles(basePath, "**\\*", null);
			}
			else
			{
				PopulateFiles(basePath, manifest.Files);
			}
		}
	}

	public void Populate(ManifestMetadata manifestMetadata)
	{
		Id = ((IPackageName)manifestMetadata).Id;
		Version = ((IPackageName)manifestMetadata).Version;
		Title = ((IPackageMetadata)manifestMetadata).Title;
		Authors.AddRange(((IPackageMetadata)manifestMetadata).Authors);
		Owners.AddRange(((IPackageMetadata)manifestMetadata).Owners);
		IconUrl = ((IPackageMetadata)manifestMetadata).IconUrl;
		LicenseUrl = ((IPackageMetadata)manifestMetadata).LicenseUrl;
		ProjectUrl = ((IPackageMetadata)manifestMetadata).ProjectUrl;
		RequireLicenseAcceptance = ((IPackageMetadata)manifestMetadata).RequireLicenseAcceptance;
		DevelopmentDependency = ((IPackageMetadata)manifestMetadata).DevelopmentDependency;
		Description = ((IPackageMetadata)manifestMetadata).Description;
		Summary = ((IPackageMetadata)manifestMetadata).Summary;
		ReleaseNotes = ((IPackageMetadata)manifestMetadata).ReleaseNotes;
		Language = ((IPackageMetadata)manifestMetadata).Language;
		Copyright = ((IPackageMetadata)manifestMetadata).Copyright;
		MinClientVersion = ((IPackageMetadata)manifestMetadata).MinClientVersion;
		if (((IPackageMetadata)manifestMetadata).Tags != null)
		{
			Tags.AddRange(ParseTags(((IPackageMetadata)manifestMetadata).Tags));
		}
		DependencySets.AddRange(((IPackageMetadata)manifestMetadata).DependencySets);
		FrameworkReferences.AddRange(((IPackageMetadata)manifestMetadata).FrameworkAssemblies);
		if (manifestMetadata.ReferenceSets != null)
		{
			PackageAssemblyReferences.AddRange(manifestMetadata.ReferenceSets.Select((ManifestReferenceSet r) => new PackageReferenceSet(r)));
		}
	}

	public void PopulateFiles(string basePath, IEnumerable<ManifestFile> files)
	{
		foreach (ManifestFile file in files)
		{
			AddFiles(basePath, file.Source, file.Target, file.Exclude);
		}
	}

	private void WriteManifest(Package package, int minimumManifestVersion)
	{
		Uri uri = UriUtility.CreatePartUri(Id + Constants.ManifestExtension);
		package.CreateRelationship(uri, (TargetMode)0, "http://schemas.microsoft.com/packaging/2010/07/manifest");
		using Stream stream = package.CreatePart(uri, "application/octet", (CompressionOption)1).GetStream();
		Manifest.Create(this).Save(stream, minimumManifestVersion);
	}

	private void WriteFiles(Package package)
	{
		foreach (IPackageFile item in new HashSet<IPackageFile>(Files))
		{
			using Stream sourceStream = item.GetStream();
			try
			{
				CreatePart(package, item.Path, sourceStream);
			}
			catch
			{
				Console.WriteLine(item.Path);
				throw;
			}
		}
		foreach (IGrouping<Uri, PackagePart> item2 in from s in (IEnumerable<PackagePart>)package.GetParts()
			group s by s.Uri into _
			where _.Count() > 1
			select _)
		{
			Console.WriteLine(item2.Key);
		}
	}

	private void AddFiles(string basePath, string source, string destination, string exclude = null)
	{
		List<PhysicalPackageFile> list = PathResolver.ResolveSearchPattern(basePath, source, destination, _includeEmptyDirectories).ToList();
		if (_includeEmptyDirectories)
		{
			list.RemoveAll((PhysicalPackageFile file) => file.TargetFramework == null && Path.GetFileName(file.TargetPath) == "_._");
		}
		ExcludeFiles(list, basePath, exclude);
		if (!PathResolver.IsWildcardSearch(source) && !PathResolver.IsDirectoryPath(source) && !list.Any())
		{
			throw new FileNotFoundException(string.Format(CultureInfo.CurrentCulture, NuGetResources.PackageAuthoring_FileNotFound, new object[1] { source }));
		}
		Files.AddRange(list);
	}

	private static void ExcludeFiles(List<PhysicalPackageFile> searchFiles, string basePath, string exclude)
	{
		if (string.IsNullOrEmpty(exclude))
		{
			return;
		}
		string[] array = exclude.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string wildcard in array)
		{
			string text = PathResolver.NormalizeWildcardForExcludedFiles(basePath, wildcard);
			PathResolver.FilterPackageFiles(searchFiles, (PhysicalPackageFile p) => p.SourcePath, new string[1] { text });
		}
	}

	private static void CreatePart(Package package, string path, Stream sourceStream)
	{
		if (PackageHelper.IsManifest(path))
		{
			return;
		}
		Uri uri = UriUtility.CreatePartUri(path);
		using Stream destination = package.CreatePart(uri, "application/octet", (CompressionOption)1).GetStream();
		sourceStream.CopyTo(destination);
	}

	private static IEnumerable<string> ParseTags(string tags)
	{
		return from tag in tags.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
			select tag.Trim();
	}

	private static bool IsPrereleaseDependency(PackageDependency dependency)
	{
		IVersionSpec versionSpec = dependency.VersionSpec;
		if (versionSpec != null)
		{
			if (!(versionSpec.MinVersion != null) || string.IsNullOrEmpty(dependency.VersionSpec.MinVersion.SpecialVersion))
			{
				if (versionSpec.MaxVersion != null)
				{
					return !string.IsNullOrEmpty(dependency.VersionSpec.MaxVersion.SpecialVersion);
				}
				return false;
			}
			return true;
		}
		return false;
	}

	private static bool ValidateSpecialVersionLength(SemanticVersion version)
	{
		if (!(version == null) && version.SpecialVersion != null)
		{
			return version.SpecialVersion.Length <= 20;
		}
		return true;
	}
}
