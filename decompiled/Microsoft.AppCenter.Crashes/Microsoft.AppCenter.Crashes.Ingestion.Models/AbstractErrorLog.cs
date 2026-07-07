using System;
using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Crashes.Ingestion.Models;

public class AbstractErrorLog : Log
{
	[JsonProperty(PropertyName = "id")]
	public Guid Id { get; set; }

	[JsonProperty(PropertyName = "processId")]
	public int ProcessId { get; set; }

	[JsonProperty(PropertyName = "processName")]
	public string ProcessName { get; set; }

	[JsonProperty(PropertyName = "parentProcessId")]
	public int? ParentProcessId { get; set; }

	[JsonProperty(PropertyName = "parentProcessName")]
	public string ParentProcessName { get; set; }

	[JsonProperty(PropertyName = "errorThreadId")]
	public long? ErrorThreadId { get; set; }

	[JsonProperty(PropertyName = "errorThreadName")]
	public string ErrorThreadName { get; set; }

	[JsonProperty(PropertyName = "fatal")]
	public bool Fatal { get; set; }

	[JsonProperty(PropertyName = "appLaunchTimestamp")]
	public DateTime? AppLaunchTimestamp { get; set; }

	[JsonProperty(PropertyName = "architecture")]
	public string Architecture { get; set; }

	public AbstractErrorLog()
	{
	}

	public AbstractErrorLog(Microsoft.AppCenter.Ingestion.Models.Device device, Guid id, int processId, string processName, bool fatal, DateTime? timestamp = null, Guid? sid = null, string userId = null, int? parentProcessId = null, string parentProcessName = null, long? errorThreadId = null, string errorThreadName = null, DateTime? appLaunchTimestamp = null, string architecture = null)
		: base(device, timestamp, sid, userId)
	{
		Id = id;
		ProcessId = processId;
		ProcessName = processName;
		ParentProcessId = parentProcessId;
		ParentProcessName = parentProcessName;
		ErrorThreadId = errorThreadId;
		ErrorThreadName = errorThreadName;
		Fatal = fatal;
		AppLaunchTimestamp = appLaunchTimestamp;
		Architecture = architecture;
	}

	public override void Validate()
	{
		base.Validate();
		if (ProcessName == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "ProcessName");
		}
	}
}
