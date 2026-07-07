using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Crashes.Ingestion.Models;

public class Binary
{
	[JsonProperty(PropertyName = "id")]
	public string Id { get; set; }

	[JsonProperty(PropertyName = "startAddress")]
	public string StartAddress { get; set; }

	[JsonProperty(PropertyName = "endAddress")]
	public string EndAddress { get; set; }

	[JsonProperty(PropertyName = "name")]
	public string Name { get; set; }

	[JsonProperty(PropertyName = "path")]
	public string Path { get; set; }

	[JsonProperty(PropertyName = "architecture")]
	public string Architecture { get; set; }

	[JsonProperty(PropertyName = "primaryArchitectureId")]
	public long? PrimaryArchitectureId { get; set; }

	[JsonProperty(PropertyName = "architectureVariantId")]
	public long? ArchitectureVariantId { get; set; }

	public Binary()
	{
	}

	public Binary(string id, string startAddress, string endAddress, string name, string path, string architecture = null, long? primaryArchitectureId = null, long? architectureVariantId = null)
	{
		Id = id;
		StartAddress = startAddress;
		EndAddress = endAddress;
		Name = name;
		Path = path;
		Architecture = architecture;
		PrimaryArchitectureId = primaryArchitectureId;
		ArchitectureVariantId = architectureVariantId;
	}

	public virtual void Validate()
	{
		if (Id == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "Id");
		}
		if (StartAddress == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "StartAddress");
		}
		if (EndAddress == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "EndAddress");
		}
		if (Name == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "Name");
		}
		if (Path == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "Path");
		}
	}
}
