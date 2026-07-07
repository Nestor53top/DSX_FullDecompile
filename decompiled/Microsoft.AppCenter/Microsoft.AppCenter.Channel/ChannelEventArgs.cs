using System;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Channel;

public abstract class ChannelEventArgs : EventArgs
{
	public Log Log { get; }

	protected ChannelEventArgs(Log log)
	{
		Log = log;
	}
}
