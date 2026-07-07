using Newtonsoft.Json;

namespace Microsoft.AppCenter.Ingestion.Models;

[JsonObject("number")]
public class NumberProperty : CustomProperty
{
	internal const string JsonIdentifier = "number";

	[JsonProperty(PropertyName = "value")]
	public object Value { get; set; }

	public NumberProperty()
	{
	}

	public NumberProperty(string name, int value)
		: base(name)
	{
		Value = value;
	}

	public NumberProperty(string name, long value)
		: base(name)
	{
		Value = value;
	}

	public NumberProperty(string name, float value)
		: base(name)
	{
		Value = value;
	}

	public NumberProperty(string name, double value)
		: base(name)
	{
		Value = value;
	}

	public NumberProperty(string name, decimal value)
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
