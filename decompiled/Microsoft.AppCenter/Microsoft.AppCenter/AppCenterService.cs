using System;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Ingestion.Http;
using Microsoft.AppCenter.Utils;

namespace Microsoft.AppCenter;

public abstract class AppCenterService : IAppCenterService
{
	private const string PreferenceKeySeparator = "_";

	private const string KeyEnabled = "AppCenterServiceEnabled";

	protected readonly object _serviceLock = new object();

	protected virtual IApplicationSettings ApplicationSettings => AppCenter.Instance.ApplicationSettings;

	protected virtual INetworkStateAdapter NetworkStateAdapter => AppCenter.Instance.NetworkStateAdapter;

	protected IChannelUnit Channel { get; private set; }

	protected IChannelGroup ChannelGroup { get; private set; }

	protected abstract string ChannelName { get; }

	public abstract string ServiceName { get; }

	public virtual string LogTag => AppCenterLog.LogTag + ServiceName;

	protected virtual string EnabledPreferenceKey => "AppCenterServiceEnabled_" + ChannelName;

	protected virtual int TriggerCount => 50;

	protected virtual TimeSpan TriggerInterval => Constants.DefaultTriggerInterval;

	protected virtual int TriggerMaxParallelRequests => 3;

	public virtual bool InstanceEnabled
	{
		get
		{
			lock (_serviceLock)
			{
				return ApplicationSettings.GetValue(EnabledPreferenceKey, defaultValue: true);
			}
		}
		set
		{
			lock (_serviceLock)
			{
				string text = (value ? "enabled" : "disabled");
				if (value && !AppCenter.IsEnabledAsync().Result)
				{
					AppCenterLog.Error(LogTag, "The SDK is disabled. Set AppCenter.Enabled to 'true' before enabling a specific service.");
					return;
				}
				if (value == InstanceEnabled)
				{
					AppCenterLog.Info(LogTag, ServiceName + " service has already been " + text + ".");
					return;
				}
				Channel?.SetEnabled(value);
				ApplicationSettings.SetValue(EnabledPreferenceKey, value);
				AppCenterLog.Info(LogTag, ServiceName + " service has been " + text);
			}
		}
	}

	protected bool IsInactive
	{
		get
		{
			lock (_serviceLock)
			{
				if (Channel == null)
				{
					AppCenterLog.Error(AppCenterLog.LogTag, ServiceName + " service not initialized; discarding calls.");
					return true;
				}
				if (InstanceEnabled)
				{
					return false;
				}
				AppCenterLog.Info(AppCenterLog.LogTag, ServiceName + " service not enabled; discarding calls.");
				return true;
			}
		}
	}

	public virtual void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
	{
		if (channelGroup == null)
		{
			throw new ArgumentNullException("channelGroup");
		}
		lock (_serviceLock)
		{
			ChannelGroup = channelGroup;
			Channel = channelGroup.AddChannel(ChannelName, TriggerCount, TriggerInterval, TriggerMaxParallelRequests);
			Channel.SetEnabled(InstanceEnabled);
		}
	}
}
