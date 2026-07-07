using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Channel;

public interface IChannel : IDisposable
{
	event EventHandler<EnqueuingLogEventArgs> EnqueuingLog;

	event EventHandler<FilteringLogEventArgs> FilteringLog;

	event EventHandler<SendingLogEventArgs> SendingLog;

	event EventHandler<SentLogEventArgs> SentLog;

	event EventHandler<FailedToSendLogEventArgs> FailedToSendLog;

	void SetEnabled(bool enabled);

	Task ShutdownAsync();

	void SetNetworkRequestAllowed(bool isAllowed);
}
