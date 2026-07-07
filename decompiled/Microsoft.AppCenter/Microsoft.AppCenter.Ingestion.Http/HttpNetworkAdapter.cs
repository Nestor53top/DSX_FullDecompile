using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Ingestion.Http;

public sealed class HttpNetworkAdapter : IHttpNetworkAdapter, IDisposable
{
	internal const string ContentTypeValue = "application/json; charset=utf-8";

	private HttpClient _httpClient;

	private readonly object _lockObject = new object();

	internal static readonly Func<HttpMessageHandler> HttpMessageHandlerOverride;

	private static readonly uint[] NetworkUnavailableCodes;

	private HttpClient HttpClient
	{
		get
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			lock (_lockObject)
			{
				if (_httpClient != null)
				{
					return _httpClient;
				}
				_httpClient = ((HttpMessageHandlerOverride != null) ? new HttpClient(HttpMessageHandlerOverride()) : new HttpClient());
				return _httpClient;
			}
		}
	}

	static HttpNetworkAdapter()
	{
		NetworkUnavailableCodes = new uint[4] { 2147954407u, 2147954429u, 2147954430u, 2147954431u };
		if ((ServicePointManager.SecurityProtocol & SecurityProtocolType.Tls12) != SecurityProtocolType.Tls12)
		{
			ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
			AppCenterLog.Debug(AppCenterLog.LogTag, "Enabled TLS 1.2 explicitly as it was disabled.");
		}
	}

	public HttpNetworkAdapter()
	{
	}

	internal HttpNetworkAdapter(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<string> SendAsync(string uri, string method, IDictionary<string, string> headers, string jsonContent, CancellationToken cancellationToken)
	{
		HttpRequestMessage request = CreateRequest(uri, method, headers, jsonContent);
		try
		{
			HttpResponseMessage response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				if (response == null)
				{
					throw new IngestionException("Null response received");
				}
				string text = "(null)";
				string contentType = null;
				if (response.Content != null)
				{
					text = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false);
					IEnumerable<string> source = default(IEnumerable<string>);
					if (((HttpHeaders)response.Content.Headers).TryGetValues("Content-Type", ref source))
					{
						contentType = source.FirstOrDefault();
					}
				}
				string message = string.Format(arg2: (contentType != null && !contentType.StartsWith("text/") && !contentType.StartsWith("application/")) ? "<binary>" : text, format: "HTTP response status={0} ({1}) payload={2}", arg0: (int)response.StatusCode, arg1: response.StatusCode);
				AppCenterLog.Verbose(AppCenterLog.LogTag, message);
				if (response.StatusCode < HttpStatusCode.OK || response.StatusCode >= HttpStatusCode.MultipleChoices)
				{
					throw new HttpIngestionException(message)
					{
						Method = ((object)request.Method).ToString(),
						RequestUri = request.RequestUri,
						StatusCode = (int)response.StatusCode,
						RequestContent = jsonContent,
						ResponseContent = text
					};
				}
				return text;
			}
			finally
			{
				((IDisposable)response)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)request)?.Dispose();
		}
	}

	internal HttpRequestMessage CreateRequest(string uri, string method, IDictionary<string, string> headers, string jsonContent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		HttpRequestMessage val = new HttpRequestMessage
		{
			Method = new HttpMethod(method),
			RequestUri = new Uri(uri)
		};
		foreach (KeyValuePair<string, string> header in headers)
		{
			((HttpHeaders)val.Headers).Add(header.Key, header.Value);
		}
		val.Content = (HttpContent)new StringContent(jsonContent, Encoding.UTF8);
		val.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
		return val;
	}

	private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		try
		{
			return await ((HttpMessageInvoker)HttpClient).SendAsync(request, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (HttpRequestException ex)
		{
			throw new NetworkIngestionException((Exception)ex);
		}
		catch (InvalidOperationException innerException)
		{
			throw new IngestionException(innerException);
		}
		catch (Exception ex2)
		{
			Exception ex3 = ex2;
			Exception e = ex3;
			if (Array.Exists(NetworkUnavailableCodes, (uint code) => code == (uint)e.HResult))
			{
				throw new NetworkIngestionException(e);
			}
			throw;
		}
	}

	public void Dispose()
	{
		lock (_lockObject)
		{
			HttpClient httpClient = _httpClient;
			if (httpClient != null)
			{
				((HttpMessageInvoker)httpClient).Dispose();
			}
			_httpClient = null;
		}
	}
}
