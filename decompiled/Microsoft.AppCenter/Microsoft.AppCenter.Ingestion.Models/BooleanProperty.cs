using Newtonsoft.Json;

namespace Microsoft.AppCenter.Ingestion.Models;

[JsonObject("boolean")]
public class BooleanProperty : CustomProperty
{
	internal const string JsonIdentifier = "boolean";

	[JsonProperty(PropertyName = "value")]
	public bool Value { get; set; }

	public BooleanProperty()
	{
	}

	public BooleanProperty(string name, bool value)
		: base(name)
	{
		Value = value;
	}

	public override object GetValue()
	{
		return Value;
	}

	public override void Validate()
	{
		base.Validate();
	}
}
