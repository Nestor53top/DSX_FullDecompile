using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Channel;

public class FilteringLogEventArgs : ChannelEventArgs
{
	public bool FilterRequested { get; set; }

	public FilteringLogEventArgs(Log log)
		: base(log)
	{
	}
}
