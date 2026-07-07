using System;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Ingestion.Models;

public class Log
{
	[JsonProperty(PropertyName = "timestamp")]
	public DateTime? Timestamp { get; set; }

	[JsonProperty(PropertyName = "sid")]
	public Guid? Sid { get; set; }

	[JsonProperty(PropertyName = "userId")]
	public string UserId { get; set; }

	[JsonProperty(PropertyName = "device")]
	public Device Device { get; set; }

	public Log()
	{
	}

	public Log(Device device, DateTime? timestamp = null, Guid? sid = null, string userId = null)
	{
		Timestamp = timestamp;
		Sid = sid;
		UserId = userId;
		Device = device;
	}

	public virtual void Validate()
	{
		if (Device == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "Device");
		}
		if (Device != null)
		{
			Device.Validate();
		}
	}
}
