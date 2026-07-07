using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Analytics.Ingestion.Models;

[JsonObject("event")]
public class EventLog : LogWithProperties
{
	internal const string JsonIdentifier = "event";

	[JsonProperty(PropertyName = "id")]
	public Guid Id { get; set; }

	[JsonProperty(PropertyName = "name")]
	public string Name { get; set; }

	public EventLog()
	{
	}

	public EventLog(Microsoft.AppCenter.Ingestion.Models.Device device, Guid id, string name, DateTime? timestamp = null, Guid? sid = null, string userId = null, IDictionary<string, string> properties = null)
		: base(device, timestamp, sid, userId, properties)
	{
		Id = id;
		Name = name;
	}

	public override void Validate()
	{
		base.Validate();
		if (Name == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "Name");
		}
		if (Name != null && Name.Length > 256)
		{
			throw new ValidationException(ValidationException.Rule.MaxLength, "Name", 256);
		}
	}
}
