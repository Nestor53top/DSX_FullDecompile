using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Xml.Serialization;
using NuGet.Resources;

namespace NuGet;

[XmlType("metadata")]
internal class ManifestMetadata : IPackageMetadata, IPackageName, IValidatableObject
{
	private string _owners;

	private string _minClientVersionString;

	[XmlAttribute("minClientVersion")]
	[ManifestVersion(5)]
	public string MinClientVersionString
	{
		get
		{
			return _minClientVersionString;
		}
		set
		{
			Version result = null;
			if (!string.IsNullOrEmpty(value) && !System.Version.TryParse(value, out result))
			{
				throw new InvalidDataException(NuGetResources.Manifest_InvalidMinClientVersion);
			}
			_minClientVersionString = value;
			MinClientVersion = result;
		}
	}

	[XmlIgnore]
	public Version MinClientVersion { get; private set; }

	[Required(ErrorMessageResourceType = typeof(NuGetResources), ErrorMessageResourceName = "Manifest_RequiredMetadataMissing")]
	[XmlElement("id")]
	public string Id { get; set; }

	[Required(ErrorMessageResourceType = typeof(NuGetResources), ErrorMessageResourceName = "Manifest_RequiredMetadataMissing")]
	[XmlElement("version")]
	public string Version { get; set; }

	[XmlElement("title")]
	public string Title { get; set; }

	[XmlElement("authors")]
	[Required(ErrorMessageResourceType = typeof(NuGetResources), ErrorMessageResourceName = "Manifest_RequiredMetadataMissing")]
	public string Authors { get; set; }

	[XmlElement("owners")]
	public string Owners
	{
		get
		{
			return _owners ?? Authors;
		}
		set
		{
			_owners = value;
		}
	}

	[XmlElement("licenseUrl")]
	public string LicenseUrl { get; set; }

	[XmlElement("projectUrl")]
	public string ProjectUrl { get; set; }

	[XmlElement("iconUrl")]
	public string IconUrl { get; set; }

	[XmlElement("requireLicenseAcceptance")]
	public bool RequireLicenseAcceptance { get; set; }

	[DefaultValue(false)]
	[XmlElement("developmentDependency")]
	public bool DevelopmentDependency { get; set; }

	[Required(ErrorMessageResourceType = typeof(NuGetResources), ErrorMessageResourceName = "Manifest_RequiredMetadataMissing")]
	[XmlElement("description")]
	public string Description { get; set; }

	[XmlElement("summary")]
	public string Summary { get; set; }

	[ManifestVersion(2)]
	[XmlElement("releaseNotes")]
	public string ReleaseNotes { get; set; }

	[ManifestVersion(2)]
	[XmlElement("copyright")]
	public string Copyright { get; set; }

	[XmlElement("language")]
	public string Language { get; set; }

	[XmlElement("tags")]
	public string Tags { get; set; }

	[XmlArrayItem("group", typeof(ManifestDependencySet))]
	[XmlArray("dependencies", IsNullable = false)]
	[XmlArrayItem("dependency", typeof(ManifestDependency))]
	public List<object> DependencySetsSerialize
	{
		get
		{
			if (DependencySets == null || DependencySets.Count == 0)
			{
				return null;
			}
			if (DependencySets.Any((ManifestDependencySet set) => set.TargetFramework != null))
			{
				return DependencySets.Cast<object>().ToList();
			}
			return DependencySets.SelectMany((ManifestDependencySet set) => set.Dependencies).Cast<object>().ToList();
		}
		set
		{
			throw new InvalidOperationException();
		}
	}

	[XmlIgnore]
	public List<ManifestDependencySet> DependencySets { get; set; }

	[XmlArray("frameworkAssemblies")]
	[XmlArrayItem("frameworkAssembly")]
	public List<ManifestFrameworkAssembly> FrameworkAssemblies { get; set; }

	[ManifestVersion(2)]
	[XmlArray("references", IsNullable = false)]
	[XmlArrayItem("group", typeof(ManifestReferenceSet))]
	[XmlArrayItem("reference", typeof(ManifestReference))]
	public List<object> ReferenceSetsSerialize
	{
		get
		{
			if (ReferenceSets == null || ReferenceSets.Count == 0)
			{
				return null;
			}
			if (ReferenceSets.Any((ManifestReferenceSet set) => set.TargetFramework != null))
			{
				return ReferenceSets.Cast<object>().ToList();
			}
			return ReferenceSets.SelectMany((ManifestReferenceSet set) => set.References).Cast<object>().ToList();
		}
		set
		{
			throw new InvalidOperationException();
		}
	}

	[XmlIgnore]
	public List<ManifestReferenceSet> ReferenceSets { get; set; }

	SemanticVersion IPackageName.Version
	{
		get
		{
			if (Version == null)
			{
				return null;
			}
			return new SemanticVersion(Version);
		}
	}

	Uri IPackageMetadata.IconUrl
	{
		get
		{
			if (IconUrl == null)
			{
				return null;
			}
			return new Uri(IconUrl);
		}
	}

	Uri IPackageMetadata.LicenseUrl
	{
		get
		{
			if (LicenseUrl == null)
			{
				return null;
			}
			return new Uri(LicenseUrl);
		}
	}

	Uri IPackageMetadata.ProjectUrl
	{
		get
		{
			if (ProjectUrl == null)
			{
				return null;
			}
			return new Uri(ProjectUrl);
		}
	}

	IEnumerable<string> IPackageMetadata.Authors
	{
		get
		{
			if (string.IsNullOrEmpty(Authors))
			{
				return Enumerable.Empty<string>();
			}
			return Authors.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		}
	}

	IEnumerable<string> IPackageMetadata.Owners
	{
		get
		{
			if (string.IsNullOrEmpty(Owners))
			{
				return Enumerable.Empty<string>();
			}
			return Owners.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		}
	}

	IEnumerable<PackageDependencySet> IPackageMetadata.DependencySets
	{
		get
		{
			if (DependencySets == null)
			{
				return Enumerable.Empty<PackageDependencySet>();
			}
			List<PackageDependencySet> list = (from set in DependencySets.Select(CreatePackageDependencySet)
				group set by set.TargetFramework into @group
				select new PackageDependencySet(@group.Key, @group.SelectMany((PackageDependencySet g) => g.Dependencies))).ToList();
			int num = list.FindIndex((PackageDependencySet set) => set.TargetFramework == null);
			if (num > -1)
			{
				PackageDependencySet item = list[num];
				list.RemoveAt(num);
				list.Insert(0, item);
			}
			return list;
		}
	}

	ICollection<PackageReferenceSet> IPackageMetadata.PackageAssemblyReferences
	{
		get
		{
			if (ReferenceSets == null)
			{
				return new PackageReferenceSet[0];
			}
			List<PackageReferenceSet> list = (from r in ReferenceSets
				select new PackageReferenceSet(r) into set
				group set by set.TargetFramework into @group
				select new PackageReferenceSet(@group.Key, @group.SelectMany((PackageReferenceSet g) => g.References))).ToList();
			int num = list.FindIndex((PackageReferenceSet set) => set.TargetFramework == null);
			if (num > -1)
			{
				PackageReferenceSet item = list[num];
				list.RemoveAt(num);
				list.Insert(0, item);
			}
			return list;
		}
	}

	IEnumerable<FrameworkAssemblyReference> IPackageMetadata.FrameworkAssemblies
	{
		get
		{
			if (FrameworkAssemblies == null)
			{
				return Enumerable.Empty<FrameworkAssemblyReference>();
			}
			return FrameworkAssemblies.Select((ManifestFrameworkAssembly frameworkReference) => new FrameworkAssemblyReference(frameworkReference.AssemblyName, ParseFrameworkNames(frameworkReference.TargetFramework)));
		}
	}

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (!string.IsNullOrEmpty(Id))
		{
			if (Id.Length > 100)
			{
				yield return new ValidationResult(string.Format(CultureInfo.CurrentCulture, NuGetResources.Manifest_IdMaxLengthExceeded, new object[0]));
			}
			else if (!PackageIdValidator.IsValidPackageId(Id))
			{
				yield return new ValidationResult(string.Format(CultureInfo.CurrentCulture, NuGetResources.InvalidPackageId, new object[1] { Id }));
			}
		}
		if (LicenseUrl == string.Empty)
		{
			yield return new ValidationResult(string.Format(CultureInfo.CurrentCulture, NuGetResources.Manifest_UriCannotBeEmpty, new object[1] { "LicenseUrl" }));
		}
		if (IconUrl == string.Empty)
		{
			yield return new ValidationResult(string.Format(CultureInfo.CurrentCulture, NuGetResources.Manifest_UriCannotBeEmpty, new object[1] { "IconUrl" }));
		}
		if (ProjectUrl == string.Empty)
		{
			yield return new ValidationResult(string.Format(CultureInfo.CurrentCulture, NuGetResources.Manifest_UriCannotBeEmpty, new object[1] { "ProjectUrl" }));
		}
		if (RequireLicenseAcceptance && string.IsNullOrWhiteSpace(LicenseUrl))
		{
			yield return new ValidationResult(NuGetResources.Manifest_RequireLicenseAcceptanceRequiresLicenseUrl);
		}
	}

	private static IEnumerable<FrameworkName> ParseFrameworkNames(string frameworkNames)
	{
		if (string.IsNullOrEmpty(frameworkNames))
		{
			return Enumerable.Empty<FrameworkName>();
		}
		return frameworkNames.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(VersionUtility.ParseFrameworkName);
	}

	private static PackageDependencySet CreatePackageDependencySet(ManifestDependencySet manifestDependencySet)
	{
		FrameworkName targetFramework = ((manifestDependencySet.TargetFramework == null) ? null : VersionUtility.ParseFrameworkName(manifestDependencySet.TargetFramework));
		IEnumerable<PackageDependency> dependencies = manifestDependencySet.Dependencies.Select((ManifestDependency d) => new PackageDependency(d.Id, string.IsNullOrEmpty(d.Version) ? null : VersionUtility.ParseVersionSpec(d.Version)));
		return new PackageDependencySet(targetFramework, dependencies);
	}
}
