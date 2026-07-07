using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Ingestion.Models;

[JsonObject("customProperties")]
public class CustomPropertyLog : Log
{
	internal const string JsonIdentifier = "customProperties";

	[JsonProperty(PropertyName = "properties")]
	public IList<CustomProperty> Properties { get; set; }

	public CustomPropertyLog()
	{
		Properties = new List<CustomProperty>();
	}

	public override void Validate()
	{
		base.Validate();
		if (Properties == null)
		{
			return;
		}
		if (Properties.Count > 60)
		{
			throw new ValidationException(ValidationException.Rule.MaxItems, "Properties", 60);
		}
		if (Properties.Count < 1)
		{
			throw new ValidationException(ValidationException.Rule.MinItems, "Properties", 1);
		}
		foreach (CustomProperty property in Properties)
		{
			property?.Validate();
		}
	}
}
