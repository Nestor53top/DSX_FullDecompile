using Microsoft.AppCenter.Channel;

namespace Microsoft.AppCenter;

public interface IAppCenterService
{
	string ServiceName { get; }

	bool InstanceEnabled { get; set; }

	void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret);
}
