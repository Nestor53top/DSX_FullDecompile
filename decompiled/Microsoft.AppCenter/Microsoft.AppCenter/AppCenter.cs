using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Ingestion.Http;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Windows.Shared.Utils;

namespace Microsoft.AppCenter;

public class AppCenter
{
	private const string PlatformIdentifier = "windowsdesktop";

	internal const string EnabledKey = "AppCenterEnabled";

	internal const string InstallIdKey = "AppCenterInstallId";

	internal const string AllowedNetworkRequestsKey = "AppCenterAllowedNetworkRequests";

	private const string ConfigurationErrorMessage = "Failed to configure App Center.";

	private const string StartErrorMessage = "Failed to start services.";

	private const string NotConfiguredMessage = "App Center hasn't been configured. You need to call AppCenter.Start with appSecret or AppCenter.Configure first.";

	private const string ChannelName = "core";

	private const long MinimumStorageSize = 24576L;

	private const long DefaultStorageMaxSize = 10485760L;

	private static readonly object AppCenterLock = new object();

	private static IApplicationSettingsFactory _applicationSettingsFactory;

	private static IChannelGroupFactory _channelGroupFactory;

	private readonly IApplicationSettings _applicationSettings;

	private INetworkStateAdapter _networkStateAdapter;

	private IChannelGroup _channelGroup;

	private IChannelUnit _channel;

	private readonly HashSet<IAppCenterService> _services = new HashSet<IAppCenterService>();

	private List<string> _startedServiceNames;

	private string _logUrl;

	private bool _instanceConfigured;

	private string _appSecret;

	private long _storageMaxSize;

	private TaskCompletionSource<bool> _storageTaskCompletionSource;

	private static volatile AppCenter _instanceField;

	private const string SecretDelimiter = ";";

	private const string PlatformKeyValueDelimiter = "=";

	private const string TargetKeyName = "target";

	private const string TargetKeyNameUpper = "Target";

	private const string AppSecretKeyName = "appsecret";

	private const string SecretsPattern = "([^;=]+)=([^;]+)(?:;\\s*)?";

	private static readonly Regex _secretsRegex = new Regex("([^;=]+)=([^;]+)(?:;\\s*)?", RegexOptions.Compiled);

	internal static AppCenter Instance
	{
		get
		{
			if (_instanceField != null)
			{
				return _instanceField;
			}
			lock (AppCenterLock)
			{
				return _instanceField ?? (_instanceField = new AppCenter());
			}
		}
		set
		{
			lock (AppCenterLock)
			{
				_instanceField = value;
			}
		}
	}

	private static LogLevel PlatformLogLevel
	{
		get
		{
			return AppCenterLog.Level;
		}
		set
		{
			AppCenterLog.Level = value;
		}
	}

	public static bool PlatformIsNetworkRequestsAllowed
	{
		get
		{
			lock (AppCenterLock)
			{
				return Instance._applicationSettings.GetValue("AppCenterAllowedNetworkRequests", defaultValue: true);
			}
		}
		set
		{
			lock (AppCenterLock)
			{
				if (PlatformIsNetworkRequestsAllowed == value)
				{
					AppCenterLog.Info(AppCenterLog.LogTag, "Network requests are already " + (value ? "allowed" : "disallowed"));
					return;
				}
				Instance._applicationSettings.SetValue("AppCenterAllowedNetworkRequests", value);
				if (Instance._channelGroup != null)
				{
					Instance._channelGroup.SetNetworkRequestAllowed(value);
				}
				AppCenterLog.Info(AppCenterLog.LogTag, "Set network requests " + (value ? "allowed" : "forbidden"));
			}
		}
	}

	private static bool PlatformConfigured
	{
		get
		{
			lock (AppCenterLock)
			{
				return Instance._instanceConfigured;
			}
		}
	}

	internal IApplicationSettings ApplicationSettings => _applicationSettings;

	internal INetworkStateAdapter NetworkStateAdapter => _networkStateAdapter;

	private bool InstanceEnabled => _applicationSettings.GetValue("AppCenterEnabled", defaultValue: true);

	public static LogLevel LogLevel
	{
		get
		{
			return PlatformLogLevel;
		}
		set
		{
			PlatformLogLevel = value;
		}
	}

	public static bool IsNetworkRequestsAllowed
	{
		get
		{
			return PlatformIsNetworkRequestsAllowed;
		}
		set
		{
			PlatformIsNetworkRequestsAllowed = value;
		}
	}

	public static string SdkVersion => "4.4.0";

	public static bool Configured => PlatformConfigured;

	public static void SetCountryCode(string countryCode)
	{
		if (countryCode != null && countryCode.Length != 2)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, "App Center accepts only the two-letter ISO country code.");
		}
		else
		{
			AbstractDeviceInformationHelper.SetCountryCode(countryCode);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete]
	public static void SetApplicationSettingsFactory(IApplicationSettingsFactory factory)
	{
		lock (AppCenterLock)
		{
			_applicationSettingsFactory = factory;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete]
	public static void SetChannelGroupFactory(IChannelGroupFactory factory)
	{
		lock (AppCenterLock)
		{
			_channelGroupFactory = factory;
		}
	}

	private static Task<bool> PlatformIsEnabledAsync()
	{
		lock (AppCenterLock)
		{
			return Task.FromResult(Instance.InstanceEnabled);
		}
	}

	private static void PlatformSetUserId(string userId)
	{
		if (userId == null || UserIdContext.CheckUserIdValidForAppCenter(userId))
		{
			UserIdContext.Instance.UserId = userId;
		}
	}

	private static Task PlatformSetEnabledAsync(bool enabled)
	{
		lock (AppCenterLock)
		{
			return Instance.SetInstanceEnabled(enabled);
		}
	}

	private static Task<Guid?> PlatformGetInstallIdAsync()
	{
		lock (AppCenterLock)
		{
			Guid? guid = Instance._applicationSettings.GetValue<Guid?>("AppCenterInstallId");
			if (!guid.HasValue)
			{
				guid = Guid.NewGuid();
				Instance._applicationSettings.SetValue("AppCenterInstallId", guid);
			}
			return Task.FromResult(guid);
		}
	}

	private static void PlatformSetLogUrl(string logUrl)
	{
		lock (AppCenterLock)
		{
			Instance.SetInstanceLogUrl(logUrl);
		}
	}

	internal static void PlatformSetCustomProperties(CustomProperties customProperties)
	{
		lock (AppCenterLock)
		{
			Instance.SetInstanceCustomProperties(customProperties);
		}
	}

	internal static void PlatformUnsetInstance()
	{
		Instance = null;
	}

	private static void PlatformConfigure(string appSecret)
	{
		lock (AppCenterLock)
		{
			try
			{
				Instance.InstanceConfigure(appSecret);
			}
			catch (AppCenterException exception)
			{
				AppCenterLog.Error(AppCenterLog.LogTag, "Failed to configure App Center.", exception);
			}
		}
	}

	private static void PlatformStart(params Type[] services)
	{
		lock (AppCenterLock)
		{
			try
			{
				Instance.StartInstance(services);
			}
			catch (AppCenterException exception)
			{
				AppCenterLog.Error(AppCenterLog.LogTag, "Failed to start services.", exception);
			}
		}
	}

	private static void PlatformStart(string appSecret, params Type[] services)
	{
		lock (AppCenterLock)
		{
			try
			{
				Instance.InstanceConfigure(appSecret);
			}
			catch (AppCenterException exception)
			{
				AppCenterLog.Error(AppCenterLog.LogTag, "Failed to configure App Center.", exception);
			}
			try
			{
				Instance.StartInstance(services);
			}
			catch (AppCenterException exception2)
			{
				AppCenterLog.Error(AppCenterLog.LogTag, "Failed to start services.", exception2);
			}
		}
	}

	private static Task<bool> PlatformSetMaxStorageSizeAsync(long sizeInBytes)
	{
		lock (AppCenterLock)
		{
			return Instance.SetInstanceStorageMaxSize(sizeInBytes);
		}
	}

	public static void SetWrapperSdk(WrapperSdk wrapperSdk)
	{
		AbstractDeviceInformationHelper.SetWrapperSdk(wrapperSdk);
	}

	private AppCenter()
	{
		lock (AppCenterLock)
		{
			_applicationSettings = _applicationSettingsFactory?.CreateApplicationSettings() ?? new DefaultApplicationSettings();
			LogSerializer.AddLogType("startService", typeof(StartServiceLog));
			LogSerializer.AddLogType("customProperties", typeof(CustomPropertyLog));
			ApplicationLifecycleHelper.Instance.UnhandledExceptionOccurred += OnUnhandledExceptionOccurred;
		}
	}

	private Task SetInstanceEnabled(bool value)
	{
		string text = (value ? "enabled" : "disabled");
		if (InstanceEnabled == value)
		{
			AppCenterLog.Info(AppCenterLog.LogTag, "App Center has already been " + text + ".");
			return Task.FromResult<object>(null);
		}
		_channelGroup?.SetEnabled(value);
		_applicationSettings.SetValue("AppCenterEnabled", value);
		foreach (IAppCenterService service in _services)
		{
			service.InstanceEnabled = value;
		}
		AppCenterLog.Info(AppCenterLog.LogTag, "App Center has been " + text + ".");
		if (_startedServiceNames != null && value)
		{
			StartServiceLog log = new StartServiceLog
			{
				Services = _startedServiceNames
			};
			_startedServiceNames = null;
			return _channel.EnqueueAsync(log);
		}
		return Task.FromResult<object>(null);
	}

	private void SetInstanceLogUrl(string logUrl)
	{
		_logUrl = logUrl;
		_channelGroup?.SetLogUrl(logUrl);
	}

	private Task<bool> SetInstanceStorageMaxSize(long storageMaxSize)
	{
		TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
		if (_instanceConfigured)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, "SetMaxStorageSize may not be called after App Center has been configured.");
			taskCompletionSource.SetResult(result: false);
			return taskCompletionSource.Task;
		}
		if (_storageMaxSize > 0)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, "SetMaxStorageSize may only be called once per app launch.");
			taskCompletionSource.SetResult(result: false);
			return taskCompletionSource.Task;
		}
		if (storageMaxSize < 24576)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, $"Maximum storage size must be at least {24576L} bytes.");
			taskCompletionSource.SetResult(result: false);
			return taskCompletionSource.Task;
		}
		_storageMaxSize = storageMaxSize;
		_storageTaskCompletionSource = taskCompletionSource;
		return _storageTaskCompletionSource.Task;
	}

	private void SetInstanceCustomProperties(CustomProperties customProperties)
	{
		if (!Configured)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, "App Center hasn't been configured. You need to call AppCenter.Start with appSecret or AppCenter.Configure first.");
			return;
		}
		if (customProperties == null || customProperties.Properties.Count == 0)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, "Custom properties may not be null or empty.");
			return;
		}
		_channel.EnqueueAsync(new CustomPropertyLog
		{
			Properties = customProperties.Properties
		});
	}

	private void OnUnhandledExceptionOccurred(object sender, UnhandledExceptionOccurredEventArgs args)
	{
		_channelGroup?.WaitStorageOperationsAsync().GetAwaiter().GetResult();
	}

	internal void InstanceConfigure(string appSecretOrSecrets)
	{
		if (_instanceConfigured)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "App Center may only be configured once.");
			return;
		}
		_appSecret = GetSecretAndTargetForPlatform(appSecretOrSecrets, "windowsdesktop");
		_networkStateAdapter = new NetworkStateAdapter();
		_channelGroup = _channelGroupFactory?.CreateChannelGroup(_appSecret, _networkStateAdapter) ?? new ChannelGroup(_appSecret, null, _networkStateAdapter);
		_channel = _channelGroup.AddChannel("core", 50, Constants.DefaultTriggerInterval, 3);
		_channel.SetEnabled(InstanceEnabled);
		if (_logUrl != null)
		{
			_channelGroup.SetLogUrl(_logUrl);
		}
		if (_storageMaxSize > 0)
		{
			_channelGroup.SetMaxStorageSizeAsync(_storageMaxSize).ContinueWith(delegate(Task<bool> task)
			{
				_storageTaskCompletionSource?.SetResult(task.Result);
			});
		}
		else
		{
			_channelGroup.SetMaxStorageSizeAsync(10485760L);
		}
		_instanceConfigured = true;
		AppCenterLog.Info(AppCenterLog.LogTag, "App Center SDK configured successfully.");
	}

	internal void StartInstance(params Type[] services)
	{
		if (services == null)
		{
			throw new AppCenterException("Services array is null.");
		}
		if (!_instanceConfigured)
		{
			throw new AppCenterException("App Center has not been configured.");
		}
		List<string> list = new List<string>();
		foreach (Type type in services)
		{
			if (type == null)
			{
				AppCenterLog.Warn(AppCenterLog.LogTag, "Skipping null service. Please check that you did not pass a null argument.");
				continue;
			}
			try
			{
				if (!(type.GetRuntimeProperty("Instance")?.GetValue(null) is IAppCenterService appCenterService))
				{
					throw new AppCenterException("Service type does not contain static 'Instance' property of type IAppCenterService. The service is either not an App Center service or it's unsupported on this platform or the SDK is used from a .NET standard library and the nuget was not also added to the UWP/WPF/WinForms project.");
				}
				StartService(appCenterService);
				list.Add(appCenterService.ServiceName);
			}
			catch (AppCenterException exception)
			{
				AppCenterLog.Error(AppCenterLog.LogTag, "Failed to start service '" + type.Name + "'; skipping it.", exception);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		if (InstanceEnabled)
		{
			_channel.EnqueueAsync(new StartServiceLog
			{
				Services = list
			}).ConfigureAwait(continueOnCapturedContext: false);
			return;
		}
		if (_startedServiceNames == null)
		{
			_startedServiceNames = new List<string>();
		}
		_startedServiceNames.AddRange(list);
	}

	private void StartService(IAppCenterService service)
	{
		if (service == null)
		{
			throw new AppCenterException("Attempted to start an invalid App Center service.");
		}
		if (_channelGroup == null)
		{
			throw new AppCenterException("Attempted to start a service after App Center has been shut down.");
		}
		if (_services.Contains(service))
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "App Center has already started the service with class name '" + service.GetType().Name + "'");
			return;
		}
		if (!InstanceEnabled && service.InstanceEnabled)
		{
			service.InstanceEnabled = false;
		}
		service.OnChannelGroupReady(_channelGroup, _appSecret);
		_services.Add(service);
		AppCenterLog.Info(AppCenterLog.LogTag, "'" + service.GetType().Name + "' service started.");
	}

	internal static string GetSecretAndTargetForPlatform(string secrets, string platformIdentifier)
	{
		string key = platformIdentifier + "Target";
		if (string.IsNullOrEmpty(secrets))
		{
			throw new AppCenterException("App secrets string is null or empty");
		}
		if (!secrets.Contains("="))
		{
			AppCenterLog.Debug(AppCenterLog.LogTag, "No named identifier found in appSecret; using as-is");
			return secrets;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (Match item in _secretsRegex.Matches(secrets))
		{
			dictionary[item.Groups[1].Value] = item.Groups[2].Value;
		}
		if (dictionary.ContainsKey("target"))
		{
			AppCenterLog.Debug(AppCenterLog.LogTag, "Found 'target=' identifier in the secret; using as-is.");
			return secrets;
		}
		if (dictionary.ContainsKey("appsecret"))
		{
			AppCenterLog.Debug(AppCenterLog.LogTag, "Found 'appSecret=' identifier in the secret; using as-is.");
			return secrets;
		}
		string value = string.Empty;
		string value2 = string.Empty;
		if (dictionary.ContainsKey(platformIdentifier))
		{
			dictionary.TryGetValue(platformIdentifier, out value);
		}
		if (dictionary.ContainsKey(key))
		{
			dictionary.TryGetValue(key, out value2);
		}
		if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(value2))
		{
			throw new AppCenterException("Error parsing key for '" + platformIdentifier + "'");
		}
		if (!string.IsNullOrEmpty(value2))
		{
			if (!string.IsNullOrEmpty(value))
			{
				value = "appsecret=" + value + ";";
			}
			value = value + "target=" + value2;
		}
		return value;
	}

	public static void SetUserId(string userId)
	{
		PlatformSetUserId(userId);
	}

	public static Task<bool> IsEnabledAsync()
	{
		return PlatformIsEnabledAsync();
	}

	public static Task SetEnabledAsync(bool enabled)
	{
		return PlatformSetEnabledAsync(enabled);
	}

	public static Task<Guid?> GetInstallIdAsync()
	{
		return PlatformGetInstallIdAsync();
	}

	public static void SetLogUrl(string logUrl)
	{
		PlatformSetLogUrl(logUrl);
	}

	public static void Configure(string appSecret)
	{
		PlatformConfigure(appSecret);
	}

	public static void Start(params Type[] services)
	{
		PlatformStart(services);
	}

	public static void Start(string appSecret, params Type[] services)
	{
		PlatformStart(appSecret, services);
	}

	public static void SetCustomProperties(CustomProperties customProperties)
	{
		PlatformSetCustomProperties(customProperties);
	}

	public static Task<bool> SetMaxStorageSizeAsync(long sizeInBytes)
	{
		return PlatformSetMaxStorageSizeAsync(sizeInBytes);
	}

	internal static void UnsetInstance()
	{
		PlatformUnsetInstance();
	}
}
