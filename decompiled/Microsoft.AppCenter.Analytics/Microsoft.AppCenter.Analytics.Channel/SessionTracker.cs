using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AppCenter.Analytics.Ingestion.Models;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Windows.Shared.Utils;

namespace Microsoft.AppCenter.Analytics.Channel;

internal class SessionTracker : ISessionTracker
{
	private enum SessionState
	{
		Inactive,
		Active,
		None
	}

	private SessionState _currentSessionState = SessionState.None;

	internal static long SessionTimeout = 20000L;

	private readonly IChannelUnit _channel;

	private long _lastQueuedLogTime;

	private long _lastResumedTime;

	private long _lastPausedTime;

	private readonly object _lockObject = new object();

	public SessionTracker(IChannel channelGroup, IChannelUnit channel)
	{
		lock (_lockObject)
		{
			_channel = channel;
			channelGroup.EnqueuingLog += HandleEnqueuingLog;
		}
	}

	public void Pause()
	{
		lock (_lockObject)
		{
			if (_currentSessionState == SessionState.Inactive)
			{
				AppCenterLog.Warn(Analytics.Instance.LogTag, "Trying to pause already inactive session.");
				return;
			}
			AppCenterLog.Debug(Analytics.Instance.LogTag, "SessionTracker.Pause");
			_lastPausedTime = TimeHelper.CurrentTimeInMilliseconds();
			_currentSessionState = SessionState.Inactive;
		}
	}

	public void Resume()
	{
		lock (_lockObject)
		{
			if (_currentSessionState == SessionState.Active)
			{
				AppCenterLog.Warn(Analytics.Instance.LogTag, "Trying to resume already active session.");
				return;
			}
			AppCenterLog.Debug(Analytics.Instance.LogTag, "SessionTracker.Resume");
			_lastResumedTime = TimeHelper.CurrentTimeInMilliseconds();
			_currentSessionState = SessionState.Active;
			SendStartSessionIfNeeded();
		}
	}

	public void Stop()
	{
		lock (_lockObject)
		{
			SessionContext.SessionId = null;
		}
	}

	private void HandleEnqueuingLog(object sender, EnqueuingLogEventArgs e)
	{
		lock (_lockObject)
		{
			if (!(e.Log is StartSessionLog) && !(e.Log is StartServiceLog))
			{
				if (!e.Log.Timestamp.HasValue)
				{
					e.Log.Sid = SessionContext.SessionId;
				}
				_lastQueuedLogTime = TimeHelper.CurrentTimeInMilliseconds();
			}
		}
	}

	private void SendStartSessionIfNeeded()
	{
		long now = TimeHelper.CurrentTimeInMilliseconds();
		if (!SessionContext.SessionId.HasValue || HasSessionTimedOut(now))
		{
			SessionContext.SessionId = Guid.NewGuid();
			_lastQueuedLogTime = TimeHelper.CurrentTimeInMilliseconds();
			StartSessionLog log = new StartSessionLog
			{
				Sid = SessionContext.SessionId
			};
			_channel.EnqueueAsync(log).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private bool HasSessionTimedOut(long now)
	{
		return HasSessionTimedOut(now, _lastQueuedLogTime, _lastResumedTime, _lastPausedTime);
	}

	internal static bool HasSessionTimedOut(long now, long lastQueuedLogTime, long lastResumedTime, long lastPausedTime)
	{
		if (lastPausedTime == 0L)
		{
			return false;
		}
		bool flag = lastQueuedLogTime == 0L || now - lastQueuedLogTime >= SessionTimeout;
		bool flag2 = lastResumedTime - Math.Max(lastPausedTime, lastQueuedLogTime) >= SessionTimeout;
		AppCenterLog.Debug(Analytics.Instance.LogTag, $"noLogSentForLong={flag} " + $"wasBackgroundForLong={flag2}");
		return flag && flag2;
	}

	internal static bool SetExistingSessionId(Log log, IDictionary<long, Guid> sessions)
	{
		if (!log.Timestamp.HasValue)
		{
			return false;
		}
		long logTime = log.Timestamp.Value.Ticks / 10000;
		long num = sessions.Keys.Where((long num2) => num2 <= logTime).DefaultIfEmpty(-1L).Max();
		if (num == -1)
		{
			return false;
		}
		log.Sid = sessions[num];
		return true;
	}
}
