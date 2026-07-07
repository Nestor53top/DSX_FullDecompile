using System;
using System.Globalization;
using System.Net;

namespace NuGet;

internal static class HttpUtility
{
	private const string UserAgentTemplate = "{0}/{1} ({2})";

	private const string UserAgentWithHostTemplate = "{0}/{1} ({2}, {3})";

	private const string ProjectGuidsHeader = "NuGet-ProjectGuids";

	public static string CreateUserAgentString(string client)
	{
		if (client == null)
		{
			throw new ArgumentNullException("client");
		}
		Version version = typeof(HttpUtility).Assembly.GetName().Version;
		return string.Format(CultureInfo.InvariantCulture, "{0}/{1} ({2})", new object[3]
		{
			client,
			version,
			Environment.OSVersion
		});
	}

	public static string CreateUserAgentString(string client, string host)
	{
		if (client == null)
		{
			throw new ArgumentNullException("client");
		}
		if (host == null)
		{
			throw new ArgumentNullException("host");
		}
		Version version = typeof(HttpUtility).Assembly.GetName().Version;
		return string.Format(CultureInfo.InvariantCulture, "{0}/{1} ({2}, {3})", new object[4]
		{
			client,
			version,
			Environment.OSVersion,
			host
		});
	}

	public static void SetUserAgent(WebRequest request, string userAgent, string projectGuids = null)
	{
		if (request == null)
		{
			throw new ArgumentNullException("request");
		}
		if (userAgent == null)
		{
			throw new ArgumentNullException("userAgent");
		}
		if (request is HttpWebRequest httpWebRequest)
		{
			httpWebRequest.UserAgent = userAgent;
		}
		else
		{
			request.Headers[HttpRequestHeader.UserAgent] = userAgent;
		}
		if (!string.IsNullOrEmpty(projectGuids))
		{
			request.Headers["NuGet-ProjectGuids"] = projectGuids;
		}
		else
		{
			request.Headers.Remove("NuGet-ProjectGuids");
		}
	}
}
