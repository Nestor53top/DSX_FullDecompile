using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Analytics.Ingestion.Models;

[JsonObject("page")]
public class PageLog : LogWithProperties
{
	internal const string JsonIdentifier = "page";

	[JsonProperty(PropertyName = "name")]
	public string Name { get; set; }

	public PageLog()
	{
	}

	public PageLog(Microsoft.AppCenter.Ingestion.Models.Device device, string name, DateTime? timestamp = null, Guid? sid = null, string userId = null, IDictionary<string, string> properties = null)
		: base(device, timestamp, sid, userId, properties)
	{
		Name = name;
	}

	public override void Validate()
	{
		base.Validate();
		if (Name == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "Name");
		}
	}
}
