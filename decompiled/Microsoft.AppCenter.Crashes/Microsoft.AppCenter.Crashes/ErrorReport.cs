using System;
using Microsoft.AppCenter.Crashes.Ingestion.Models;

namespace Microsoft.AppCenter.Crashes;

public class ErrorReport
{
	public string Id { get; }

	public DateTimeOffset AppStartTime { get; }

	public DateTimeOffset AppErrorTime { get; }

	public Device Device { get; }

	[Obsolete("This property is no longer set due to a security issue, use StackTrace as an alternative.")]
	public System.Exception Exception { get; }

	public string StackTrace { get; }

	public AndroidErrorDetails AndroidDetails { get; }

	public iOSErrorDetails iOSDetails { get; }

	public ErrorReport(ManagedErrorLog log, string stackTrace)
	{
		Id = log.Id.ToString();
		AppStartTime = new DateTimeOffset(log.AppLaunchTimestamp.Value);
		AppErrorTime = new DateTimeOffset(log.Timestamp.Value);
		Device = new Device(log.Device);
		StackTrace = stackTrace;
	}
}
