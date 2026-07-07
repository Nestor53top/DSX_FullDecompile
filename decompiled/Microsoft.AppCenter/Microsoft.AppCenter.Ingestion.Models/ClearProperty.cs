using Newtonsoft.Json;

namespace Microsoft.AppCenter.Ingestion.Models;

[JsonObject("clear")]
public class ClearProperty : CustomProperty
{
	internal const string JsonIdentifier = "clear";

	public ClearProperty()
	{
	}

	public ClearProperty(string name)
		: base(name)
	{
	}

	public override object GetValue()
	{
		return null;
	}

	public override void Validate()
	{
		base.Validate();
	}
}
