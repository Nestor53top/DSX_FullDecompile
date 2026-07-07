using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using NuGet.Resources;

namespace NuGet;

[XmlType("reference")]
internal class ManifestReference : IValidatableObject, IEquatable<ManifestReference>
{
	private static readonly char[] _referenceFileInvalidCharacters = Path.GetInvalidFileNameChars();

	[Required(ErrorMessageResourceType = typeof(NuGetResources), ErrorMessageResourceName = "Manifest_RequiredMetadataMissing")]
	[XmlAttribute("file")]
	public string File { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (string.IsNullOrEmpty(File))
		{
			yield return new ValidationResult(NuGetResources.Manifest_RequiredElementMissing, new string[1] { "File" });
		}
		else if (File.IndexOfAny(_referenceFileInvalidCharacters) != -1)
		{
			yield return new ValidationResult(string.Format(CultureInfo.CurrentCulture, NuGetResources.Manifest_InvalidReferenceFile, new object[1] { File }));
		}
	}

	public bool Equals(ManifestReference other)
	{
		if (other != null)
		{
			return string.Equals(File, other.File, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public override int GetHashCode()
	{
		if (File != null)
		{
			return File.GetHashCode();
		}
		return 0;
	}
}
