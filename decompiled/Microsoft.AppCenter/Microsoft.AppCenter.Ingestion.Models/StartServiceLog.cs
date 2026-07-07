using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Ingestion.Models;

[JsonObject("startService")]
public class StartServiceLog : Log
{
	internal const string JsonIdentifier = "startService";

	[JsonProperty(PropertyName = "services")]
	public IList<string> Services { get; set; }

	public StartServiceLog()
	{
		Services = new List<string>();
	}

	public StartServiceLog(Device device, DateTime? timestamp = null, Guid? sid = null, IList<string> services = null)
		: base(device, timestamp, sid)
	{
		Services = services;
	}

	public override void Validate()
	{
		base.Validate();
		if (Services != null && Services.Count < 1)
		{
			throw new ValidationException(ValidationException.Rule.MinItems, "Services", 1);
		}
	}
}
