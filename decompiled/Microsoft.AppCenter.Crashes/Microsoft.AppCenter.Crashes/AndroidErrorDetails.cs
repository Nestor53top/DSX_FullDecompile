using System;

namespace Microsoft.AppCenter.Crashes;

public class AndroidErrorDetails
{
	[Obsolete("This property is no longer set due to a security issue, use StackTrace instead.")]
	public object Throwable { get; }

	public string StackTrace { get; }

	public string ThreadName { get; }

	internal AndroidErrorDetails(string stackTrace, string threadName)
	{
		StackTrace = stackTrace;
		ThreadName = threadName;
	}
}
