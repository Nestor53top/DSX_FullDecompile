using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Crashes.Utils;
using Microsoft.AppCenter.Crashes.Windows.Utils;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Utils.Files;
using Microsoft.AppCenter.Windows.Shared.Utils;

namespace Microsoft.AppCenter.Crashes;

public class Crashes : AppCenterService
{
	public new const string LogTag = "AppCenterCrashes";

	public static ShouldProcessErrorReportCallback ShouldProcessErrorReport;

	public static ShouldAwaitUserConfirmationCallback ShouldAwaitUserConfirmation;

	public static GetErrorAttachmentsCallback GetErrorAttachments;

	private static readonly object CrashesLock;

	private static volatile Crashes _instanceField;

	private const int MaxAttachmentSize = 7340032;

	internal const string PrefKeyAlwaysSend = "AppCenterCrashesAlwaysSend";

	internal readonly IDictionary<Guid, ManagedErrorLog> _unprocessedManagedErrorLogs;

	private readonly IDictionary<Guid, ErrorReport> _errorReportCache;

	private TaskCompletionSource<ErrorReport> _lastSessionErrorReportTaskSource;

	public static Crashes Instance
	{
		get
		{
			if (_instanceField != null)
			{
				return _instanceField;
			}
			lock (CrashesLock)
			{
				return _instanceField ?? (_instanceField = new Crashes());
			}
		}
		set
		{
			lock (CrashesLock)
			{
				_instanceField = value;
			}
		}
	}

	protected override string ChannelName => "crashes";

	protected override int TriggerCount => 1;

	protected override TimeSpan TriggerInterval => TimeSpan.FromSeconds(1.0);

	public override string ServiceName => "Crashes";

	internal Task ProcessPendingErrorsTask { get; set; }

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

	public static event SendingErrorReportEventHandler SendingErrorReport;

	public static event SentErrorReportEventHandler SentErrorReport;

	public static event FailedToSendErrorReportEventHandler FailedToSendErrorReport;

	public static void NotifyUserConfirmation(UserConfirmation confirmation)
	{
		PlatformNotifyUserConfirmation(confirmation);
	}

	public static Task<bool> IsEnabledAsync()
	{
		return PlatformIsEnabledAsync();
	}

	public static Task SetEnabledAsync(bool enabled)
	{
		return PlatformSetEnabledAsync(enabled);
	}

	public static Task<bool> HasCrashedInLastSessionAsync()
	{
		return PlatformHasCrashedInLastSessionAsync();
	}

	public static Task<ErrorReport> GetLastSessionCrashReportAsync()
	{
		return PlatformGetLastSessionCrashReportAsync();
	}

	public static Task<bool> HasReceivedMemoryWarningInLastSessionAsync()
	{
		return PlatformHasReceivedMemoryWarningInLastSessionAsync();
	}

	[Conditional("DEBUG")]
	public static void GenerateTestCrash()
	{
		throw new TestCrashException();
	}

	public static void TrackError(System.Exception exception, IDictionary<string, string> properties = null, params ErrorAttachmentLog[] attachments)
	{
		if (exception == null)
		{
			AppCenterLog.Error("AppCenterCrashes", "TrackError exception parameter cannot be null.");
		}
		else
		{
			PlatformTrackError(exception, properties, attachments);
		}
	}

	internal static void UnsetInstance()
	{
		PlatformUnsetInstance();
	}

	static Crashes()
	{
		CrashesLock = new object();
		LogSerializer.AddLogType("managedError", typeof(ManagedErrorLog));
		LogSerializer.AddLogType("errorAttachment", typeof(ErrorAttachmentLog));
		LogSerializer.AddLogType("handledError", typeof(HandledErrorLog));
	}

	private static Task<bool> PlatformIsEnabledAsync()
	{
		lock (CrashesLock)
		{
			return Task.FromResult(Instance.InstanceEnabled);
		}
	}

	private static Task PlatformSetEnabledAsync(bool enabled)
	{
		lock (CrashesLock)
		{
			Instance.InstanceEnabled = enabled;
			return Task.FromResult<object>(null);
		}
	}

	private static void OnUnhandledExceptionOccurred(object sender, UnhandledExceptionOccurredEventArgs args)
	{
		ManagedErrorLog errorLog = ErrorLogHelper.CreateErrorLog(args.Exception);
		ErrorLogHelper.SaveErrorLogFiles(args.Exception, errorLog);
	}

	private static Task<bool> PlatformHasCrashedInLastSessionAsync()
	{
		return Instance.InstanceHasCrashedInLastSessionAsync();
	}

	private static Task<bool> PlatformHasReceivedMemoryWarningInLastSessionAsync()
	{
		return Instance.InstanceHasReceivedMemoryWarningInLastSessionAsync();
	}

	private static Task<ErrorReport> PlatformGetLastSessionCrashReportAsync()
	{
		return Instance.InstanceGetLastSessionCrashReportAsync();
	}

	private static void PlatformNotifyUserConfirmation(UserConfirmation userConfirmation)
	{
		Instance.InstanceHandlerUserConfirmation(userConfirmation);
	}

	private static void PlatformTrackError(System.Exception exception, IDictionary<string, string> properties, ErrorAttachmentLog[] attachments)
	{
		Instance.InstanceTrackError(exception, properties, attachments);
	}

	internal Crashes()
	{
		_unprocessedManagedErrorLogs = new Dictionary<Guid, ManagedErrorLog>();
		_errorReportCache = new ConcurrentDictionary<Guid, ErrorReport>();
	}

	public override void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
	{
		lock (_serviceLock)
		{
			base.OnChannelGroupReady(channelGroup, appSecret);
			ApplyEnabledState(InstanceEnabled);
			if (InstanceEnabled)
			{
				_lastSessionErrorReportTaskSource = new TaskCompletionSource<ErrorReport>();
				ProcessPendingErrorsTask = ProcessPendingErrorsAsync();
			}
		}
	}

	private void ApplyEnabledState(bool enabled)
	{
		lock (_serviceLock)
		{
			if (enabled && base.ChannelGroup != null)
			{
				ApplicationLifecycleHelper.Instance.UnhandledExceptionOccurred += OnUnhandledExceptionOccurred;
				base.ChannelGroup.SendingLog += ChannelSendingLog;
				base.ChannelGroup.SentLog += ChannelSentLog;
				base.ChannelGroup.FailedToSendLog += ChannelFailedToSendLog;
			}
			else if (!enabled)
			{
				ApplicationLifecycleHelper.Instance.UnhandledExceptionOccurred -= OnUnhandledExceptionOccurred;
				if (base.ChannelGroup != null)
				{
					base.ChannelGroup.SendingLog -= ChannelSendingLog;
					base.ChannelGroup.SentLog -= ChannelSentLog;
					base.ChannelGroup.FailedToSendLog -= ChannelFailedToSendLog;
				}
				ProcessPendingErrorsTask?.Wait();
				ErrorLogHelper.RemoveAllStoredErrorLogFiles();
				_unprocessedManagedErrorLogs.Clear();
				_lastSessionErrorReportTaskSource = null;
			}
		}
	}

	private async Task<bool> InstanceHasCrashedInLastSessionAsync()
	{
		return await InstanceGetLastSessionCrashReportAsync() != null;
	}

	private Task<ErrorReport> InstanceGetLastSessionCrashReportAsync()
	{
		return _lastSessionErrorReportTaskSource?.Task ?? Task.FromResult<ErrorReport>(null);
	}

	private async Task<bool> InstanceHasReceivedMemoryWarningInLastSessionAsync()
	{
		return await Task.FromResult(result: false);
	}

	private Task ProcessPendingErrorsAsync()
	{
		return Task.Run(async delegate
		{
			ErrorReport errorReport = null;
			foreach (Microsoft.AppCenter.Utils.Files.File errorLogFile in ErrorLogHelper.GetErrorLogFiles())
			{
				AppCenterLog.Debug("AppCenterCrashes", "Process pending error file " + errorLogFile.Name);
				ManagedErrorLog managedErrorLog = ErrorLogHelper.ReadErrorLogFile(errorLogFile);
				if (managedErrorLog == null)
				{
					AppCenterLog.Error("AppCenterCrashes", "Error parsing error log. Deleting invalid file: " + errorLogFile.Name);
					try
					{
						errorLogFile.Delete();
					}
					catch (System.Exception exception)
					{
						AppCenterLog.Warn("AppCenterCrashes", "Failed to delete error log file " + errorLogFile.Name + ".", exception);
					}
					if (Guid.TryParse(Path.GetFileNameWithoutExtension(errorLogFile.Name), out var result))
					{
						ErrorLogHelper.RemoveStoredExceptionFile(result);
					}
				}
				else if (managedErrorLog.Device == null || !managedErrorLog.AppLaunchTimestamp.HasValue || !managedErrorLog.Timestamp.HasValue)
				{
					AppCenterLog.Error("AppCenterCrashes", "Error parsing error log. Deleting invalid file: " + errorLogFile.Name);
					RemoveAllStoredErrorLogFiles(managedErrorLog.Id);
				}
				else
				{
					ErrorReport errorReport2 = BuildErrorReport(managedErrorLog);
					if (errorReport == null || errorReport.AppErrorTime < errorReport2.AppErrorTime)
					{
						errorReport = errorReport2;
					}
					if (ShouldProcessErrorReport?.Invoke(errorReport2) ?? true)
					{
						_unprocessedManagedErrorLogs.Add(managedErrorLog.Id, managedErrorLog);
					}
					else
					{
						AppCenterLog.Debug("AppCenterCrashes", $"ShouldProcessErrorReport returned false, clean up and ignore log: {managedErrorLog.Id}");
						RemoveAllStoredErrorLogFiles(managedErrorLog.Id);
					}
				}
			}
			_lastSessionErrorReportTaskSource.SetResult(errorReport);
			await SendCrashReportsOrAwaitUserConfirmationAsync().ConfigureAwait(continueOnCapturedContext: false);
		});
	}

	private void RemoveAllStoredErrorLogFiles(Guid errorId)
	{
		_errorReportCache.Remove(errorId);
		ErrorLogHelper.RemoveStoredErrorLogFile(errorId);
		ErrorLogHelper.RemoveStoredExceptionFile(errorId);
	}

	private async Task SendCrashReportsOrAwaitUserConfirmationAsync()
	{
		bool value = ApplicationSettings.GetValue("AppCenterCrashesAlwaysSend", defaultValue: false);
		if (!_unprocessedManagedErrorLogs.Any())
		{
			return;
		}
		if (value)
		{
			AppCenterLog.Debug("AppCenterCrashes", "The flag for user confirmation is set to AlwaysSend, will send logs.");
			await HandleUserConfirmationAsync(UserConfirmation.Send);
			return;
		}
		bool? flag = ShouldAwaitUserConfirmation?.Invoke();
		if (flag.HasValue && flag.Value)
		{
			AppCenterLog.Debug("AppCenterCrashes", "ShouldAwaitUserConfirmation returned true, wait sending logs.");
			return;
		}
		AppCenterLog.Debug("AppCenterCrashes", "ShouldAwaitUserConfirmation returned false or is not implemented, will send logs.");
		await HandleUserConfirmationAsync(UserConfirmation.Send);
	}

	private void InstanceHandlerUserConfirmation(UserConfirmation userConfirmation)
	{
		lock (_serviceLock)
		{
			if (!base.IsInactive)
			{
				HandleUserConfirmationAsync(userConfirmation);
			}
		}
	}

	private Task HandleUserConfirmationAsync(UserConfirmation userConfirmation)
	{
		List<Guid> list = _unprocessedManagedErrorLogs.Keys.ToList();
		List<Task> list2 = new List<Task>();
		if (userConfirmation == UserConfirmation.DontSend)
		{
			foreach (Guid item in list)
			{
				_unprocessedManagedErrorLogs.Remove(item);
				RemoveAllStoredErrorLogFiles(item);
			}
		}
		else
		{
			if (userConfirmation == UserConfirmation.AlwaysSend)
			{
				ApplicationSettings.SetValue("AppCenterCrashesAlwaysSend", true);
			}
			foreach (Guid item2 in list)
			{
				ManagedErrorLog managedErrorLog = _unprocessedManagedErrorLogs[item2];
				list2.Add(base.Channel.EnqueueAsync(managedErrorLog));
				_unprocessedManagedErrorLogs.Remove(item2);
				ErrorLogHelper.RemoveStoredErrorLogFile(item2);
				ErrorReport report = BuildErrorReport(managedErrorLog);
				IEnumerable<ErrorAttachmentLog> enumerable = GetErrorAttachments?.Invoke(report);
				if (enumerable == null)
				{
					AppCenterLog.Debug("AppCenterCrashes", $"Crashes.GetErrorAttachments returned null; no additional information will be attached to log: {managedErrorLog.Id}.");
				}
				else
				{
					list2.Add(SendErrorAttachmentsAsync(managedErrorLog.Id, enumerable));
				}
			}
		}
		return Task.WhenAll(list2);
	}

	private void InstanceTrackError(System.Exception exception, IDictionary<string, string> properties, ErrorAttachmentLog[] attachments)
	{
		lock (_serviceLock)
		{
			if (!base.IsInactive)
			{
				properties = PropertyValidator.ValidateProperties(properties, "HandledError");
				ErrorExceptionAndBinaries errorExceptionAndBinaries = ErrorLogHelper.CreateModelExceptionAndBinaries(exception);
				Guid guid = Guid.NewGuid();
				errorExceptionAndBinaries.Exception.StackTrace = ErrorLogHelper.ObfuscateUserName(errorExceptionAndBinaries.Exception.StackTrace);
				Microsoft.AppCenter.Crashes.Ingestion.Models.Exception exception2 = errorExceptionAndBinaries.Exception;
				IList<Binary> binaries = errorExceptionAndBinaries.Binaries;
				IDictionary<string, string> properties2 = properties;
				Guid? id = guid;
				string userId = UserIdContext.Instance.UserId;
				HandledErrorLog log = new HandledErrorLog(null, exception2, null, null, userId, properties2, id, binaries);
				base.Channel.EnqueueAsync(log);
				SendErrorAttachmentsAsync(guid, attachments);
			}
		}
	}

	private Task SendErrorAttachmentsAsync(Guid errorId, IEnumerable<ErrorAttachmentLog> attachments)
	{
		List<Task> list = new List<Task>();
		foreach (ErrorAttachmentLog attachment in attachments)
		{
			if (attachment != null)
			{
				attachment.Id = Guid.NewGuid();
				attachment.ErrorId = errorId;
				if (!attachment.ValidatePropertiesForAttachment())
				{
					AppCenterLog.Error("AppCenterCrashes", "Not all required fields are present in ErrorAttachmentLog.");
				}
				else if (attachment.Data.Length > 7340032)
				{
					AppCenterLog.Error("AppCenterCrashes", $"Discarding attachment with size above {7340032} bytes: size={attachment.Data.Length}, fileName={attachment.FileName}.");
				}
				else
				{
					list.Add(base.Channel.EnqueueAsync(attachment));
				}
			}
			else
			{
				AppCenterLog.Warn("AppCenterCrashes", "Skipping null ErrorAttachmentLog.");
			}
		}
		return Task.WhenAll(list);
	}

	private ErrorReport BuildErrorReport(ManagedErrorLog log)
	{
		if (_errorReportCache.ContainsKey(log.Id))
		{
			return _errorReportCache[log.Id];
		}
		Microsoft.AppCenter.Utils.Files.File storedExceptionFile = ErrorLogHelper.GetStoredExceptionFile(log.Id);
		string stackTrace = ((storedExceptionFile == null) ? null : ErrorLogHelper.ReadExceptionFile(storedExceptionFile));
		ErrorReport errorReport = new ErrorReport(log, stackTrace);
		_errorReportCache.Add(log.Id, errorReport);
		return errorReport;
	}

	private void ChannelSendingLog(object sender, SendingLogEventArgs e)
	{
		ErrorReport errorReport = MapLogEventToReportAndDeleteOnLastEvent(e);
		if (errorReport != null)
		{
			Crashes.SendingErrorReport?.Invoke(sender, new SendingErrorReportEventArgs
			{
				Report = errorReport
			});
		}
	}

	private void ChannelSentLog(object sender, SentLogEventArgs e)
	{
		ErrorReport errorReport = MapLogEventToReportAndDeleteOnLastEvent(e);
		if (errorReport != null)
		{
			Crashes.SentErrorReport?.Invoke(sender, new SentErrorReportEventArgs
			{
				Report = errorReport
			});
		}
	}

	private void ChannelFailedToSendLog(object sender, FailedToSendLogEventArgs e)
	{
		ErrorReport errorReport = MapLogEventToReportAndDeleteOnLastEvent(e);
		if (errorReport != null)
		{
			Crashes.FailedToSendErrorReport?.Invoke(sender, new FailedToSendErrorReportEventArgs
			{
				Report = errorReport,
				Exception = e.Exception
			});
		}
	}

	private ErrorReport MapLogEventToReportAndDeleteOnLastEvent(ChannelEventArgs channelEventArgs)
	{
		if (channelEventArgs.Log is ManagedErrorLog managedErrorLog)
		{
			ErrorReport result = BuildErrorReport(managedErrorLog);
			if (channelEventArgs is SentLogEventArgs || channelEventArgs is FailedToSendLogEventArgs)
			{
				ErrorLogHelper.RemoveStoredExceptionFile(managedErrorLog.Id);
			}
			return result;
		}
		return null;
	}

	internal static void PlatformUnsetInstance()
	{
		Instance = null;
	}
}
