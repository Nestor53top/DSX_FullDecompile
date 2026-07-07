using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;

namespace NuGet;

internal abstract class LocalPackage : IPackage, IPackageMetadata, IPackageName, IServerPackageMetadata
{
	private const string ResourceAssemblyExtension = ".resources.dll";

	private IList<IPackageAssemblyReference> _assemblyReferences;

	public string Id { get; set; }

	public SemanticVersion Version { get; set; }

	public string Title { get; set; }

	public IEnumerable<string> Authors { get; set; }

	public IEnumerable<string> Owners { get; set; }

	public Uri IconUrl { get; set; }

	public Uri LicenseUrl { get; set; }

	public Uri ProjectUrl { get; set; }

	public Uri ReportAbuseUrl => null;

	public int DownloadCount => -1;

	public bool RequireLicenseAcceptance { get; set; }

	public bool DevelopmentDependency { get; set; }

	public string Description { get; set; }

	public string Summary { get; set; }

	public string ReleaseNotes { get; set; }

	public string Language { get; set; }

	public string Tags { get; set; }

	public Version MinClientVersion { get; private set; }

	public bool IsAbsoluteLatestVersion => true;

	public bool IsLatestVersion => this.IsReleaseVersion();

	public bool Listed { get; set; }

	public DateTimeOffset? Published { get; set; }

	public string Copyright { get; set; }

	public IEnumerable<PackageDependencySet> DependencySets { get; set; }

	public IEnumerable<FrameworkAssemblyReference> FrameworkAssemblies { get; set; }

	public IEnumerable<IPackageAssemblyReference> AssemblyReferences
	{
		get
		{
			if (_assemblyReferences == null)
			{
				_assemblyReferences = GetAssemblyReferencesCore().ToList();
			}
			return _assemblyReferences;
		}
	}

	public ICollection<PackageReferenceSet> PackageAssemblyReferences { get; private set; }

	protected LocalPackage()
	{
		Listed = true;
	}

	public virtual IEnumerable<FrameworkName> GetSupportedFrameworks()
	{
		return FrameworkAssemblies.SelectMany((FrameworkAssemblyReference f) => f.SupportedFrameworks).Distinct();
	}

	public IEnumerable<IPackageFile> GetFiles()
	{
		return GetFilesBase();
	}

	public abstract Stream GetStream();

	protected abstract IEnumerable<IPackageFile> GetFilesBase();

	protected abstract IEnumerable<IPackageAssemblyReference> GetAssemblyReferencesCore();

	protected void ReadManifest(Stream manifestStream)
	{
		IPackageMetadata metadata = Manifest.ReadFrom(manifestStream, validateSchema: false).Metadata;
		Id = metadata.Id;
		Version = metadata.Version;
		Title = metadata.Title;
		Authors = metadata.Authors;
		Owners = metadata.Owners;
		IconUrl = metadata.IconUrl;
		LicenseUrl = metadata.LicenseUrl;
		ProjectUrl = metadata.ProjectUrl;
		RequireLicenseAcceptance = metadata.RequireLicenseAcceptance;
		DevelopmentDependency = metadata.DevelopmentDependency;
		Description = metadata.Description;
		Summary = metadata.Summary;
		ReleaseNotes = metadata.ReleaseNotes;
		Language = metadata.Language;
		Tags = metadata.Tags;
		DependencySets = metadata.DependencySets;
		FrameworkAssemblies = metadata.FrameworkAssemblies;
		Copyright = metadata.Copyright;
		PackageAssemblyReferences = metadata.PackageAssemblyReferences;
		MinClientVersion = metadata.MinClientVersion;
		if (!string.IsNullOrEmpty(Tags))
		{
			Tags = " " + Tags + " ";
		}
	}

	protected internal static bool IsAssemblyReference(string filePath)
	{
		string libDirectory = Constants.LibDirectory;
		char directorySeparatorChar = Path.DirectorySeparatorChar;
		if (!filePath.StartsWith(libDirectory + directorySeparatorChar, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		if (Path.GetFileName(filePath) == "_._")
		{
			return true;
		}
		if (!filePath.EndsWith(".resources.dll", StringComparison.OrdinalIgnoreCase))
		{
			return Constants.AssemblyReferencesExtensions.Contains<string>(Path.GetExtension(filePath), StringComparer.OrdinalIgnoreCase);
		}
		return false;
	}

	public override string ToString()
	{
		return this.GetFullName();
	}
}
