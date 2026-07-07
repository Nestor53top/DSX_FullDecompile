using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using NuGet.Resources;

namespace NuGet;

[XmlType("package")]
internal class Manifest
{
	private class NullServiceProvider : IServiceProvider
	{
		private static readonly IServiceProvider _instance = new NullServiceProvider();

		public static IServiceProvider Instance => _instance;

		public object GetService(Type serviceType)
		{
			return null;
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<PackageReferenceSet, ManifestReferenceSet> _003C_003E9__19_0;

		public static Func<string, ManifestReference> _003C_003E9__20_0;

		public static Func<PackageDependencySet, ManifestDependencySet> _003C_003E9__21_0;

		public static Func<PackageDependency, ManifestDependency> _003C_003E9__22_0;

		public static Func<FrameworkAssemblyReference, ManifestFrameworkAssembly> _003C_003E9__23_0;

		public static ValidationEventHandler _003C_003E9__25_0;

		public static Func<ManifestDependencySet, IEnumerable<ManifestDependency>> _003C_003E9__29_0;

		public static Func<ValidationResult, string> _003C_003E9__29_1;

		internal ManifestReferenceSet _003CCreateReferenceSets_003Eb__19_0(PackageReferenceSet referenceSet)
		{
			return new ManifestReferenceSet
			{
				TargetFramework = ((referenceSet.TargetFramework != null) ? VersionUtility.GetFrameworkString(referenceSet.TargetFramework) : null),
				References = CreateReferences(referenceSet)
			};
		}

		internal ManifestReference _003CCreateReferences_003Eb__20_0(string reference)
		{
			return new ManifestReference
			{
				File = reference.SafeTrim()
			};
		}

		internal ManifestDependencySet _003CCreateDependencySets_003Eb__21_0(PackageDependencySet dependencySet)
		{
			return new ManifestDependencySet
			{
				TargetFramework = ((dependencySet.TargetFramework != null) ? VersionUtility.GetFrameworkString(dependencySet.TargetFramework) : null),
				Dependencies = CreateDependencies(dependencySet.Dependencies)
			};
		}

		internal ManifestDependency _003CCreateDependencies_003Eb__22_0(PackageDependency dependency)
		{
			return new ManifestDependency
			{
				Id = dependency.Id.SafeTrim(),
				Version = dependency.VersionSpec.ToStringSafe()
			};
		}

		internal ManifestFrameworkAssembly _003CCreateFrameworkAssemblies_003Eb__23_0(FrameworkAssemblyReference reference)
		{
			return new ManifestFrameworkAssembly
			{
				AssemblyName = reference.AssemblyName,
				TargetFramework = string.Join(", ", reference.SupportedFrameworks.Select(VersionUtility.GetFrameworkString))
			};
		}

		internal void _003CValidateManifestSchema_003Eb__25_0(object sender, ValidationEventArgs e)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)e.Severity == 0)
			{
				throw new InvalidOperationException(e.Message);
			}
		}

		internal IEnumerable<ManifestDependency> _003CValidate_003Eb__29_0(ManifestDependencySet d)
		{
			return d.Dependencies;
		}

		internal string _003CValidate_003Eb__29_1(ValidationResult r)
		{
			return r.ErrorMessage;
		}
	}

	private const string SchemaVersionAttributeName = "schemaVersion";

	[XmlElement("metadata", IsNullable = false)]
	public ManifestMetadata Metadata { get; set; }

	[XmlArray("files")]
	public List<ManifestFile> Files { get; set; }

	public Manifest()
	{
		Metadata = new ManifestMetadata();
	}

	public void Save(Stream stream)
	{
		Save(stream, validate: true, 1);
	}

	public void Save(Stream stream, int minimumManifestVersion)
	{
		Save(stream, validate: true, minimumManifestVersion);
	}

	public void Save(Stream stream, bool validate)
	{
		Save(stream, validate, 1);
	}

	public void Save(Stream stream, bool validate, int minimumManifestVersion)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (validate)
		{
			Validate(this);
		}
		string schemaNamespace = ManifestSchemaUtility.GetSchemaNamespace(Math.Max(minimumManifestVersion, ManifestVersionUtility.GetManifestVersion(Metadata)));
		XmlSerializerNamespaces val = new XmlSerializerNamespaces();
		val.Add("", schemaNamespace);
		new XmlSerializer(typeof(Manifest), schemaNamespace).Serialize(stream, (object)this, val);
	}

	public static Manifest ReadFrom(Stream stream, bool validateSchema)
	{
		return ReadFrom(stream, NullPropertyProvider.Instance, validateSchema);
	}

	public static Manifest ReadFrom(Stream stream, IPropertyProvider propertyProvider, bool validateSchema)
	{
		XDocument val = ((propertyProvider != NullPropertyProvider.Instance) ? XDocument.Parse(Preprocessor.Process(stream, propertyProvider)) : XmlUtility.LoadSafe(stream, ignoreWhiteSpace: true));
		string schemaNamespace = GetSchemaNamespace(val);
		foreach (XElement item in ((XContainer)val).Descendants())
		{
			item.Name = XName.Get(item.Name.LocalName, schemaNamespace);
		}
		CheckSchemaVersion(val);
		if (validateSchema)
		{
			ValidateManifestSchema(val, schemaNamespace);
		}
		Manifest manifest = ManifestReader.ReadManifest(val);
		Validate(manifest);
		return manifest;
	}

	private static string GetSchemaNamespace(XDocument document)
	{
		string result = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";
		XNamespace val = document.Root.Name.Namespace;
		if (val != (XNamespace)null && !string.IsNullOrEmpty(val.NamespaceName))
		{
			result = val.NamespaceName;
		}
		return result;
	}

	public static Manifest Create(IPackageMetadata metadata)
	{
		return new Manifest
		{
			Metadata = new ManifestMetadata
			{
				Id = metadata.Id.SafeTrim(),
				Version = metadata.Version.ToStringSafe(),
				Title = metadata.Title.SafeTrim(),
				Authors = GetCommaSeparatedString(metadata.Authors),
				Owners = (GetCommaSeparatedString(metadata.Owners) ?? GetCommaSeparatedString(metadata.Authors)),
				Tags = (string.IsNullOrEmpty(metadata.Tags) ? null : metadata.Tags.SafeTrim()),
				LicenseUrl = ConvertUrlToStringSafe(metadata.LicenseUrl),
				ProjectUrl = ConvertUrlToStringSafe(metadata.ProjectUrl),
				IconUrl = ConvertUrlToStringSafe(metadata.IconUrl),
				RequireLicenseAcceptance = metadata.RequireLicenseAcceptance,
				DevelopmentDependency = metadata.DevelopmentDependency,
				Description = metadata.Description.SafeTrim(),
				Copyright = metadata.Copyright.SafeTrim(),
				Summary = metadata.Summary.SafeTrim(),
				ReleaseNotes = metadata.ReleaseNotes.SafeTrim(),
				Language = metadata.Language.SafeTrim(),
				DependencySets = CreateDependencySets(metadata),
				FrameworkAssemblies = CreateFrameworkAssemblies(metadata),
				ReferenceSets = CreateReferenceSets(metadata),
				MinClientVersionString = metadata.MinClientVersion.ToStringSafe()
			}
		};
	}

	private static string ConvertUrlToStringSafe(Uri url)
	{
		if (url != null)
		{
			string text = url.OriginalString.SafeTrim();
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
		}
		return null;
	}

	private static List<ManifestReferenceSet> CreateReferenceSets(IPackageMetadata metadata)
	{
		return metadata.PackageAssemblyReferences.Select((PackageReferenceSet referenceSet) => new ManifestReferenceSet
		{
			TargetFramework = ((referenceSet.TargetFramework != null) ? VersionUtility.GetFrameworkString(referenceSet.TargetFramework) : null),
			References = CreateReferences(referenceSet)
		}).ToList();
	}

	private static List<ManifestReference> CreateReferences(PackageReferenceSet referenceSet)
	{
		if (referenceSet.References == null)
		{
			return new List<ManifestReference>();
		}
		return referenceSet.References.Select((string reference) => new ManifestReference
		{
			File = reference.SafeTrim()
		}).ToList();
	}

	private static List<ManifestDependencySet> CreateDependencySets(IPackageMetadata metadata)
	{
		if (metadata.DependencySets.IsEmpty())
		{
			return null;
		}
		return metadata.DependencySets.Select((PackageDependencySet dependencySet) => new ManifestDependencySet
		{
			TargetFramework = ((dependencySet.TargetFramework != null) ? VersionUtility.GetFrameworkString(dependencySet.TargetFramework) : null),
			Dependencies = CreateDependencies(dependencySet.Dependencies)
		}).ToList();
	}

	private static List<ManifestDependency> CreateDependencies(ICollection<PackageDependency> dependencies)
	{
		if (dependencies == null)
		{
			return new List<ManifestDependency>(0);
		}
		return dependencies.Select((PackageDependency dependency) => new ManifestDependency
		{
			Id = dependency.Id.SafeTrim(),
			Version = dependency.VersionSpec.ToStringSafe()
		}).ToList();
	}

	private static List<ManifestFrameworkAssembly> CreateFrameworkAssemblies(IPackageMetadata metadata)
	{
		if (metadata.FrameworkAssemblies.IsEmpty())
		{
			return null;
		}
		return metadata.FrameworkAssemblies.Select((FrameworkAssemblyReference reference) => new ManifestFrameworkAssembly
		{
			AssemblyName = reference.AssemblyName,
			TargetFramework = string.Join(", ", reference.SupportedFrameworks.Select(VersionUtility.GetFrameworkString))
		}).ToList();
	}

	private static string GetCommaSeparatedString(IEnumerable<string> values)
	{
		if (values == null || !values.Any())
		{
			return null;
		}
		return string.Join(",", values);
	}

	private static void ValidateManifestSchema(XDocument document, string schemaNamespace)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		XmlSchemaSet manifestSchemaSet = ManifestSchemaUtility.GetManifestSchemaSet(schemaNamespace);
		object obj = _003C_003Ec._003C_003E9__25_0;
		if (obj == null)
		{
			ValidationEventHandler val = delegate(object sender, ValidationEventArgs e)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if ((int)e.Severity == 0)
				{
					throw new InvalidOperationException(e.Message);
				}
			};
			_003C_003Ec._003C_003E9__25_0 = val;
			obj = (object)val;
		}
		Extensions.Validate(document, manifestSchemaSet, (ValidationEventHandler)obj);
	}

	private static void CheckSchemaVersion(XDocument document)
	{
		XElement metadataElement = GetMetadataElement(document);
		if (metadataElement != null)
		{
			XAttribute val = metadataElement.Attribute(XName.op_Implicit("schemaVersion"));
			if (val != null)
			{
				val.Remove();
			}
			string packageId = GetPackageId(metadataElement);
			if (!ManifestSchemaUtility.IsKnownSchema(document.Root.Name.Namespace.NamespaceName))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.IncompatibleSchema, new object[2]
				{
					packageId,
					typeof(Manifest).Assembly.GetName().Version
				}));
			}
		}
	}

	private static string GetPackageId(XElement metadataElement)
	{
		XName val = XName.Get("id", ((XObject)metadataElement).Document.Root.Name.NamespaceName);
		XElement val2 = ((XContainer)metadataElement).Element(val);
		if (val2 != null)
		{
			return val2.Value;
		}
		return null;
	}

	private static XElement GetMetadataElement(XDocument document)
	{
		XName val = XName.Get("metadata", document.Root.Name.Namespace.NamespaceName);
		return ((XContainer)document.Root).Element(val);
	}

	internal static void Validate(Manifest manifest)
	{
		List<ValidationResult> list = new List<ValidationResult>();
		TryValidate(manifest.Metadata, list);
		TryValidate(manifest.Files, list);
		if (manifest.Metadata.DependencySets != null)
		{
			TryValidate(manifest.Metadata.DependencySets.SelectMany((ManifestDependencySet d) => d.Dependencies), list);
		}
		TryValidate(manifest.Metadata.ReferenceSets, list);
		if (list.Any())
		{
			throw new ValidationException(string.Join(Environment.NewLine, list.Select((ValidationResult r) => r.ErrorMessage)));
		}
		ValidateDependencySets(manifest.Metadata);
	}

	private static void ValidateDependencySets(IPackageMetadata metadata)
	{
		foreach (PackageDependencySet dependencySet in metadata.DependencySets)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (PackageDependency dependency in dependencySet.Dependencies)
			{
				if (!hashSet.Add(dependency.Id))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.DuplicateDependenciesDefined, new object[2] { metadata.Id, dependency.Id }));
				}
				ValidateDependencyVersion(dependency);
			}
		}
	}

	private static void ValidateDependencyVersion(PackageDependency dependency)
	{
		if (dependency.VersionSpec != null && dependency.VersionSpec.MinVersion != null && dependency.VersionSpec.MaxVersion != null)
		{
			if ((!dependency.VersionSpec.IsMaxInclusive || !dependency.VersionSpec.IsMinInclusive) && dependency.VersionSpec.MaxVersion == dependency.VersionSpec.MinVersion)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.DependencyHasInvalidVersion, new object[1] { dependency.Id }));
			}
			if (dependency.VersionSpec.MaxVersion < dependency.VersionSpec.MinVersion)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.DependencyHasInvalidVersion, new object[1] { dependency.Id }));
			}
		}
	}

	private static bool TryValidate(object value, ICollection<ValidationResult> results)
	{
		if (value != null)
		{
			if (value is IEnumerable enumerable)
			{
				foreach (object item in enumerable)
				{
					Validator.TryValidateObject(item, CreateValidationContext(item), results);
				}
			}
			return Validator.TryValidateObject(value, CreateValidationContext(value), results);
		}
		return true;
	}

	private static ValidationContext CreateValidationContext(object value)
	{
		return new ValidationContext(value, NullServiceProvider.Instance, new Dictionary<object, object>());
	}
}
