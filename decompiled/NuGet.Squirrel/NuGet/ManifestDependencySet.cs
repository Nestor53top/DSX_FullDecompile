using System.Collections.Generic;
using System.Xml.Serialization;

namespace NuGet;

public class ManifestDependencySet
{
	[XmlAttribute("targetFramework")]
	public string TargetFramework { get; set; }

	[XmlElement("dependency")]
	public List<ManifestDependency> Dependencies { get; set; }
}
