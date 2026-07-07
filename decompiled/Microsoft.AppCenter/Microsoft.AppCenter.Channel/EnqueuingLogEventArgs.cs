using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Channel;

public class EnqueuingLogEventArgs : ChannelEventArgs
{
	public EnqueuingLogEventArgs(Log log)
		: base(log)
	{
	}
}
