using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Crashes.Ingestion.Models;

[JsonObject("handledError")]
public class HandledErrorLog : LogWithProperties
{
	internal const string JsonIdentifier = "handledError";

	[JsonProperty(PropertyName = "id")]
	public Guid? Id { get; set; }

	[JsonProperty(PropertyName = "binaries")]
	public IList<Binary> Binaries { get; set; }

	[JsonProperty(PropertyName = "exception")]
	public Exception Exception { get; set; }

	public HandledErrorLog()
	{
	}

	public HandledErrorLog(Microsoft.AppCenter.Ingestion.Models.Device device, Exception exception, DateTime? timestamp = null, Guid? sid = null, string userId = null, IDictionary<string, string> properties = null, Guid? id = null, IList<Binary> binaries = null)
		: base(device, timestamp, sid, userId, properties)
	{
		Id = id;
		Binaries = binaries;
		Exception = exception;
	}

	public override void Validate()
	{
		base.Validate();
		if (Exception == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "Exception");
		}
		if (Binaries != null)
		{
			foreach (Binary binary in Binaries)
			{
				binary?.Validate();
			}
		}
		if (Exception != null)
		{
			Exception.Validate();
		}
	}
}
