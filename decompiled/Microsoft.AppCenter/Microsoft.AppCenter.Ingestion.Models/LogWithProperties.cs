using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Ingestion.Models;

public class LogWithProperties : Log
{
	[JsonProperty(PropertyName = "properties")]
	public IDictionary<string, string> Properties { get; set; }

	public LogWithProperties()
	{
	}

	public LogWithProperties(Device device, DateTime? timestamp = null, Guid? sid = null, string userId = null, IDictionary<string, string> properties = null)
		: base(device, timestamp, sid, userId)
	{
		Properties = properties;
	}

	public override void Validate()
	{
		base.Validate();
	}
}
