using System;
using System.Collections.Concurrent;
using System.Net;

namespace NuGet;

internal class ProxyCache : IProxyCache
{
	private const string HostKey = "http_proxy";

	private const string UserKey = "http_proxy.user";

	private const string PasswordKey = "http_proxy.password";

	private static readonly IWebProxy _originalSystemProxy = WebRequest.GetSystemWebProxy();

	private readonly ConcurrentDictionary<Uri, WebProxy> _cache = new ConcurrentDictionary<Uri, WebProxy>();

	private static readonly Lazy<ProxyCache> _instance = new Lazy<ProxyCache>(() => new ProxyCache(Settings.LoadDefaultSettings(null, null, null), new EnvironmentVariableWrapper()));

	private readonly ISettings _settings;

	private readonly IEnvironmentVariableReader _environment;

	internal static ProxyCache Instance => _instance.Value;

	public ProxyCache(ISettings settings, IEnvironmentVariableReader environment)
	{
		_settings = settings;
		_environment = environment;
	}

	public IWebProxy GetProxy(Uri uri)
	{
		WebProxy userConfiguredProxy = GetUserConfiguredProxy();
		if (userConfiguredProxy != null)
		{
			if (_cache.TryGetValue(userConfiguredProxy.Address, out var value))
			{
				return value;
			}
			return userConfiguredProxy;
		}
		if (!IsSystemProxySet(uri))
		{
			return null;
		}
		WebProxy systemProxy = GetSystemProxy(uri);
		if (_cache.TryGetValue(systemProxy.Address, out var value2))
		{
			return value2;
		}
		return systemProxy;
	}

	internal WebProxy GetUserConfiguredProxy()
	{
		string configValue = _settings.GetConfigValue("http_proxy");
		if (!string.IsNullOrEmpty(configValue))
		{
			WebProxy webProxy = new WebProxy(configValue);
			string configValue2 = _settings.GetConfigValue("http_proxy.user");
			string configValue3 = _settings.GetConfigValue("http_proxy.password", decrypt: true);
			if (!string.IsNullOrEmpty(configValue2) && !string.IsNullOrEmpty(configValue3))
			{
				webProxy.Credentials = new NetworkCredential(configValue2, configValue3);
			}
			return webProxy;
		}
		configValue = _environment.GetEnvironmentVariable("http_proxy");
		if (!string.IsNullOrEmpty(configValue) && Uri.TryCreate(configValue, UriKind.Absolute, out Uri result))
		{
			WebProxy webProxy2 = new WebProxy(result.GetComponents(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped));
			if (!string.IsNullOrEmpty(result.UserInfo))
			{
				string[] array = result.UserInfo.Split(new char[1] { ':' });
				if (array.Length > 1)
				{
					webProxy2.Credentials = new NetworkCredential(array[0], array[1]);
				}
			}
			return webProxy2;
		}
		return null;
	}

	public void Add(IWebProxy proxy)
	{
		if (proxy is WebProxy webProxy)
		{
			_cache.TryAdd(webProxy.Address, webProxy);
		}
	}

	private static WebProxy GetSystemProxy(Uri uri)
	{
		return new WebProxy(_originalSystemProxy.GetProxy(uri));
	}

	private static bool IsSystemProxySet(Uri uri)
	{
		IWebProxy webProxy = WebRequest.DefaultWebProxy;
		if (webProxy != null)
		{
			Uri proxy = webProxy.GetProxy(uri);
			if (proxy != null)
			{
				Uri uri2 = new Uri(proxy.AbsoluteUri);
				if (string.Equals(uri2.AbsoluteUri, uri.AbsoluteUri))
				{
					return false;
				}
				if (webProxy.IsBypassed(uri))
				{
					return false;
				}
				webProxy = new WebProxy(uri2);
			}
		}
		return webProxy != null;
	}
}
