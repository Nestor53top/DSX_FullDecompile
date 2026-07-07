using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using NuGet.Resources;

namespace NuGet;

[XmlType("file")]
internal class ManifestFile : IValidatableObject
{
	private static readonly char[] _invalidTargetChars = Path.GetInvalidFileNameChars().Except(new char[3] { '\\', '/', '.' }).ToArray();

	private static readonly char[] _invalidSourceCharacters = Path.GetInvalidPathChars();

	[Required(ErrorMessageResourceType = typeof(NuGetResources), ErrorMessageResourceName = "Manifest_RequiredMetadataMissing")]
	[XmlAttribute("src")]
	public string Source { get; set; }

	[XmlAttribute("target")]
	public string Target { get; set; }

	[XmlAttribute("exclude")]
	public string Exclude { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (!string.IsNullOrEmpty(Source) && Source.IndexOfAny(_invalidSourceCharacters) != -1)
		{
			yield return new ValidationResult(string.Format(CultureInfo.CurrentCulture, NuGetResources.Manifest_SourceContainsInvalidCharacters, new object[1] { Source }));
		}
		if (!string.IsNullOrEmpty(Target) && Target.IndexOfAny(_invalidTargetChars) != -1)
		{
			yield return new ValidationResult(string.Format(CultureInfo.CurrentCulture, NuGetResources.Manifest_TargetContainsInvalidCharacters, new object[1] { Target }));
		}
		if (!string.IsNullOrEmpty(Exclude) && Exclude.IndexOfAny(_invalidSourceCharacters) != -1)
		{
			yield return new ValidationResult(string.Format(CultureInfo.CurrentCulture, NuGetResources.Manifest_ExcludeContainsInvalidCharacters, new object[1] { Exclude }));
		}
	}
}
