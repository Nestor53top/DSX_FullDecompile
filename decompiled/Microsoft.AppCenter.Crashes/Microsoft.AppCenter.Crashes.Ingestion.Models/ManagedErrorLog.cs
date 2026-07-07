using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Crashes.Ingestion.Models;

[JsonObject("managedError")]
public class ManagedErrorLog : AbstractErrorLog
{
	internal const string JsonIdentifier = "managedError";

	[JsonProperty(PropertyName = "binaries")]
	public IList<Binary> Binaries { get; set; }

	[JsonProperty(PropertyName = "buildId")]
	public string BuildId { get; set; }

	[JsonProperty(PropertyName = "exception")]
	public Exception Exception { get; set; }

	public ManagedErrorLog()
	{
	}

	public ManagedErrorLog(Microsoft.AppCenter.Ingestion.Models.Device device, Guid id, int processId, string processName, bool fatal, Exception exception, DateTime? timestamp = null, Guid? sid = null, string userId = null, int? parentProcessId = null, string parentProcessName = null, long? errorThreadId = null, string errorThreadName = null, DateTime? appLaunchTimestamp = null, string architecture = null, IList<Binary> binaries = null, string buildId = null)
		: base(device, id, processId, processName, fatal, timestamp, sid, userId, parentProcessId, parentProcessName, errorThreadId, errorThreadName, appLaunchTimestamp, architecture)
	{
		Binaries = binaries;
		BuildId = buildId;
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
