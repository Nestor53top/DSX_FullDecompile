using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Channel;

public class SentLogEventArgs : ChannelEventArgs
{
	public SentLogEventArgs(Log log)
		: base(log)
	{
	}
}
