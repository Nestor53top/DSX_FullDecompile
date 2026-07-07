using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;

namespace Microsoft.AppCenter.Ingestion.Http;

internal sealed class IngestionHttp : IIngestion, IDisposable
{
	internal const string DefaultBaseUrl = "https://in.appcenter.ms";

	internal const string ApiVersion = "/logs?api-version=1.0.0";

	internal const string AppSecret = "App-Secret";

	internal const string InstallId = "Install-ID";

	private const int MaximumCharactersDisplayedForAppSecret = 8;

	private string _baseLogUrl;

	private readonly IHttpNetworkAdapter _httpNetwork;

	public bool IsEnabled => AppCenter.PlatformIsNetworkRequestsAllowed;

	public IngestionHttp(IHttpNetworkAdapter httpNetwork)
	{
		_httpNetwork = httpNetwork;
	}

	public IServiceCall Call(string appSecret, Guid installId, IList<Log> logs)
	{
		ServiceCall call = new ServiceCall(appSecret, installId, logs);
		if (!IsEnabled)
		{
			call.SetException(new NetworkIngestionException(new Exception("SDK is in offline mode.")));
			return call;
		}
		CallAsync(appSecret, installId, logs, call.CancellationToken).ContinueWith(delegate(Task<string> task)
		{
			if (!task.IsCanceled)
			{
				if (task.IsFaulted)
				{
					call.SetException(task.Exception?.InnerException);
				}
				else
				{
					call.SetResult(task.Result);
				}
			}
		});
		return call;
	}

	private async Task<string> CallAsync(string appSecret, Guid installId, IList<Log> logs, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		string text = (string.IsNullOrEmpty(_baseLogUrl) ? "https://in.appcenter.ms" : _baseLogUrl);
		AppCenterLog.Verbose(AppCenterLog.LogTag, "Calling " + text + "/logs?api-version=1.0.0...");
		IDictionary<string, string> headers = CreateHeaders(appSecret, installId);
		AppCenterLog.Verbose(AppCenterLog.LogTag, "Headers: App-Secret=" + GetRedactedAppSecret(appSecret) + ", " + string.Format("{0}={1}", "Install-ID", installId));
		string text2 = CreateLogsContent(logs);
		AppCenterLog.Verbose(AppCenterLog.LogTag, text2);
		return await _httpNetwork.SendAsync(text + "/logs?api-version=1.0.0", "POST", headers, text2, token).ConfigureAwait(continueOnCapturedContext: false);
	}

	public void Close()
	{
	}

	public void SetLogUrl(string logUrl)
	{
		_baseLogUrl = logUrl;
	}

	private static string GetRedactedAppSecret(string appSecret)
	{
		int num = Math.Max(appSecret.Length - 8, 0);
		string text = "";
		for (int i = 0; i < num; i++)
		{
			text += "*";
		}
		return text + appSecret.Substring(num);
	}

	internal IDictionary<string, string> CreateHeaders(string appSecret, Guid installId)
	{
		return new Dictionary<string, string>
		{
			{ "App-Secret", appSecret },
			{
				"Install-ID",
				installId.ToString()
			}
		};
	}

	private string CreateLogsContent(IList<Log> logs)
	{
		return LogSerializer.Serialize(new LogContainer(logs));
	}

	public void Dispose()
	{
		_httpNetwork.Dispose();
	}
}
