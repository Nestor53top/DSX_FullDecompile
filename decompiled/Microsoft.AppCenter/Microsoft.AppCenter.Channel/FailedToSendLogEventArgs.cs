using System;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Channel;

public class FailedToSendLogEventArgs : ChannelEventArgs
{
	public Exception Exception { get; }

	public FailedToSendLogEventArgs(Log log, Exception exception)
		: base(log)
	{
		Exception = exception;
	}
}
