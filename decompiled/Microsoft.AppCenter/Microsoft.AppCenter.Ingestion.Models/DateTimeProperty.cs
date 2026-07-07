using System;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Ingestion.Models;

[JsonObject("dateTime")]
public class DateTimeProperty : CustomProperty
{
	internal const string JsonIdentifier = "dateTime";

	[JsonProperty(PropertyName = "value")]
	public DateTime Value { get; set; }

	public DateTimeProperty()
	{
	}

	public DateTimeProperty(string name, DateTime value)
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
