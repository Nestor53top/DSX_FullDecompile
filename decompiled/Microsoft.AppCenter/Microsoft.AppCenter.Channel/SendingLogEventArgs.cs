using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Channel;

public class SendingLogEventArgs : ChannelEventArgs
{
	public SendingLogEventArgs(Log log)
		: base(log)
	{
	}
}
