using Newtonsoft.Json;

namespace Microsoft.AppCenter.Ingestion.Models;

[JsonObject("string")]
public class StringProperty : CustomProperty
{
	internal const string JsonIdentifier = "string";

	[JsonProperty(PropertyName = "value")]
	public string Value { get; set; }

	public StringProperty()
	{
	}

	public StringProperty(string name, string value)
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
		if (Value == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "Value");
		}
		if (Value != null && Value.Length > 128)
		{
			throw new ValidationException(ValidationException.Rule.MaxLength, "Value", 128);
		}
	}
}
