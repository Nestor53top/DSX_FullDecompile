using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics.Channel;
using Microsoft.AppCenter.Analytics.Ingestion.Models;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Windows.Shared.Utils;

namespace Microsoft.AppCenter.Analytics;

public class Analytics : AppCenterService
{
	private const int MaxEventNameLength = 256;

	private static readonly object AnalyticsLock = new object();

	private static volatile Analytics _instanceField;

	private ISessionTracker _sessionTracker;

	private readonly ISessionTrackerFactory _sessionTrackerFactory;

	public static Analytics Instance
	{
		get
		{
			if (_instanceField != null)
			{
				return _instanceField;
			}
			lock (AnalyticsLock)
			{
				return _instanceField ?? (_instanceField = new Analytics());
			}
		}
		set
		{
			lock (AnalyticsLock)
			{
				_instanceField = value;
			}
		}
	}

	public override bool InstanceEnabled
	{
		get
		{
			return base.InstanceEnabled;
		}
		set
		{
			lock (_serviceLock)
			{
				bool instanceEnabled = InstanceEnabled;
				base.InstanceEnabled = value;
				if (value != instanceEnabled)
				{
					ApplyEnabledState(value);
				}
			}
		}
	}

	protected override string ChannelName => "analytics";

	public override string ServiceName => "Analytics";

	public static Task<bool> IsEnabledAsync()
	{
		lock (AnalyticsLock)
		{
			return Task.FromResult(Instance.InstanceEnabled);
		}
	}

	public static Task SetEnabledAsync(bool enabled)
	{
		lock (AnalyticsLock)
		{
			Instance.InstanceEnabled = enabled;
			return Task.FromResult<object>(null);
		}
	}

	public static void TrackEvent(string name, IDictionary<string, string> properties = null)
	{
		lock (AnalyticsLock)
		{
			Instance.InstanceTrackEvent(name, properties);
		}
	}

	private Analytics()
	{
		LogSerializer.AddLogType("page", typeof(PageLog));
		LogSerializer.AddLogType("event", typeof(EventLog));
		LogSerializer.AddLogType("startSession", typeof(StartSessionLog));
	}

	internal Analytics(ISessionTrackerFactory sessionTrackerFactory)
		: this()
	{
		_sessionTrackerFactory = sessionTrackerFactory;
	}

	private void InstanceTrackEvent(string name, IDictionary<string, string> properties = null)
	{
		lock (_serviceLock)
		{
			if (!base.IsInactive && ValidateName(ref name, "Event"))
			{
				properties = PropertyValidator.ValidateProperties(properties, "Event '" + name + "'");
				EventLog log = new EventLog(null, Guid.NewGuid(), name, null, null, null, properties);
				base.Channel.EnqueueAsync(log);
			}
		}
	}

	public override void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
	{
		lock (_serviceLock)
		{
			base.OnChannelGroupReady(channelGroup, appSecret);
			ApplyEnabledState(InstanceEnabled);
		}
	}

	private void ApplyEnabledState(bool enabled)
	{
		lock (_serviceLock)
		{
			if (enabled && base.ChannelGroup != null && _sessionTracker == null)
			{
				_sessionTracker = CreateSessionTracker(base.ChannelGroup, base.Channel, ApplicationSettings);
				if (!ApplicationLifecycleHelper.Instance.IsSuspended)
				{
					_sessionTracker.Resume();
				}
				SubscribeToApplicationLifecycleEvents();
			}
			else if (!enabled)
			{
				UnsubscribeFromApplicationLifecycleEvents();
				_sessionTracker?.Stop();
				_sessionTracker = null;
			}
		}
	}

	private ISessionTracker CreateSessionTracker(IChannelGroup channelGroup, IChannelUnit channel, IApplicationSettings applicationSettings)
	{
		return _sessionTrackerFactory?.CreateSessionTracker(channelGroup, channel, applicationSettings) ?? new SessionTracker(channelGroup, channel);
	}

	private bool ValidateName(ref string name, string logType)
	{
		if (string.IsNullOrEmpty(name))
		{
			AppCenterLog.Error(LogTag, logType + " name cannot be null or empty.");
			return false;
		}
		if (name.Length > 256)
		{
			AppCenterLog.Warn(LogTag, $"{logType} '{name}' : name length cannot be longer than {256} characters. Name will be truncated.");
			name = name.Substring(0, 256);
			return true;
		}
		return true;
	}

	private void SubscribeToApplicationLifecycleEvents()
	{
		ApplicationLifecycleHelper.Instance.ApplicationResuming += ApplicationResumingEventHandler;
		ApplicationLifecycleHelper.Instance.ApplicationSuspended += ApplicationSuspendedEventHandler;
	}

	private void UnsubscribeFromApplicationLifecycleEvents()
	{
		ApplicationLifecycleHelper.Instance.ApplicationResuming -= ApplicationResumingEventHandler;
		ApplicationLifecycleHelper.Instance.ApplicationSuspended -= ApplicationSuspendedEventHandler;
	}

	private void ApplicationResumingEventHandler(object sender, EventArgs e)
	{
		_sessionTracker?.Resume();
	}

	private void ApplicationSuspendedEventHandler(object sender, EventArgs e)
	{
		_sessionTracker?.Pause();
	}
}
