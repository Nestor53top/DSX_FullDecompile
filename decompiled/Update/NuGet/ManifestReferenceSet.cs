using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Serialization;

namespace NuGet;

internal class ManifestReferenceSet : IValidatableObject
{
	[XmlAttribute("targetFramework")]
	public string TargetFramework { get; set; }

	[XmlElement("reference")]
	public List<ManifestReference> References { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (References != null)
		{
			return References.SelectMany((ManifestReference r) => r.Validate(validationContext));
		}
		return Enumerable.Empty<ValidationResult>();
	}
}
