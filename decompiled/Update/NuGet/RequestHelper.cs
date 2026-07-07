using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace NuGet;

internal class RequestHelper
{
	private class HttpWebResponseWrapper : IHttpWebResponse, IDisposable
	{
		private readonly HttpWebResponse _response;

		public string AuthType => _response.Headers[HttpResponseHeader.WwwAuthenticate];

		public HttpStatusCode StatusCode => _response.StatusCode;

		public Uri ResponseUri => _response.ResponseUri;

		public NameValueCollection Headers => _response.Headers;

		public HttpWebResponseWrapper(HttpWebResponse response)
		{
			_response = response;
		}

		public void Dispose()
		{
			if (_response != null)
			{
				_response.Close();
			}
		}
	}

	private Func<WebRequest> _createRequest;

	private Action<WebRequest> _prepareRequest;

	private IProxyCache _proxyCache;

	private ICredentialCache _credentialCache;

	private ICredentialProvider _credentialProvider;

	private HttpWebRequest _previousRequest;

	private IHttpWebResponse _previousResponse;

	private HttpStatusCode? _previousStatusCode;

	private int _credentialsRetryCount;

	private bool _usingSTSAuth;

	private bool _continueIfFailed;

	private int _proxyCredentialsRetryCount;

	private bool _basicAuthIsUsedInPreviousRequest;

	private bool _disableBuffering;

	public RequestHelper(Func<WebRequest> createRequest, Action<WebRequest> prepareRequest, IProxyCache proxyCache, ICredentialCache credentialCache, ICredentialProvider credentialProvider, bool disableBuffering)
	{
		_createRequest = createRequest;
		_prepareRequest = prepareRequest;
		_proxyCache = proxyCache;
		_credentialCache = credentialCache;
		_credentialProvider = credentialProvider;
		_disableBuffering = disableBuffering;
	}

	public WebResponse GetResponse()
	{
		_previousRequest = null;
		_previousResponse = null;
		_previousStatusCode = null;
		_usingSTSAuth = false;
		_continueIfFailed = true;
		_proxyCredentialsRetryCount = 0;
		_credentialsRetryCount = 0;
		int num = 0;
		while (true)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)_createRequest();
			ConfigureRequest(httpWebRequest);
			try
			{
				if (_disableBuffering)
				{
					httpWebRequest.AllowWriteStreamBuffering = false;
					bool flag = _previousResponse != null && _previousResponse.AuthType != null && _previousResponse.AuthType.IndexOf("Basic", StringComparison.OrdinalIgnoreCase) != -1;
					NetworkCredential credential = httpWebRequest.Credentials.GetCredential(httpWebRequest.RequestUri, "Basic");
					if (credential != null && flag)
					{
						string s = credential.UserName + ":" + credential.Password;
						s = Convert.ToBase64String(Encoding.Default.GetBytes(s));
						httpWebRequest.Headers["Authorization"] = "Basic " + s;
						_basicAuthIsUsedInPreviousRequest = true;
					}
				}
				_prepareRequest(httpWebRequest);
				WebResponse webResponse = HttpShim.Instance.ShimWebRequest(httpWebRequest);
				_proxyCache.Add(httpWebRequest.Proxy);
				ICredentials credentials = httpWebRequest.Credentials;
				_credentialCache.Add(httpWebRequest.RequestUri, credentials);
				_credentialCache.Add(webResponse.ResponseUri, credentials);
				return webResponse;
			}
			catch (WebException ex)
			{
				num++;
				if (num >= 10)
				{
					throw;
				}
				using IHttpWebResponse httpWebResponse = GetResponse(ex.Response);
				if (httpWebResponse == null && ex.Status != WebExceptionStatus.SecureChannelFailure)
				{
					throw;
				}
				if (ex.Status == WebExceptionStatus.SecureChannelFailure)
				{
					if (_continueIfFailed)
					{
						_previousStatusCode = HttpStatusCode.Unauthorized;
						continue;
					}
					throw;
				}
				if (_previousStatusCode == HttpStatusCode.ProxyAuthenticationRequired && httpWebResponse.StatusCode != HttpStatusCode.ProxyAuthenticationRequired)
				{
					_proxyCache.Add(httpWebRequest.Proxy);
				}
				else if (_previousStatusCode == HttpStatusCode.Unauthorized && httpWebResponse.StatusCode != HttpStatusCode.Unauthorized)
				{
					_credentialCache.Add(httpWebRequest.RequestUri, httpWebRequest.Credentials);
					_credentialCache.Add(httpWebResponse.ResponseUri, httpWebRequest.Credentials);
				}
				_usingSTSAuth = STSAuthHelper.TryRetrieveSTSToken(httpWebRequest.RequestUri, httpWebResponse);
				if (!IsAuthenticationResponse(httpWebResponse) || !_continueIfFailed)
				{
					throw;
				}
				if (!EnvironmentUtility.IsNet45Installed && !httpWebRequest.AllowWriteStreamBuffering && httpWebResponse.AuthType != null && IsNtlmOrKerberos(httpWebResponse.AuthType))
				{
					throw;
				}
				_previousRequest = httpWebRequest;
				_previousResponse = httpWebResponse;
				_previousStatusCode = _previousResponse.StatusCode;
			}
		}
	}

	private void ConfigureRequest(HttpWebRequest request)
	{
		request.Proxy = _proxyCache.GetProxy(request.RequestUri);
		if (request.Proxy != null && request.Proxy.Credentials == null)
		{
			request.Proxy.Credentials = CredentialCache.DefaultCredentials;
		}
		if (_previousResponse == null || ShouldKeepAliveBeUsedInRequest(_previousRequest, _previousResponse))
		{
			request.Credentials = _credentialCache.GetCredentials(request.RequestUri);
			if (request.Credentials == null)
			{
				request.UseDefaultCredentials = true;
			}
		}
		else if (_previousStatusCode == HttpStatusCode.ProxyAuthenticationRequired)
		{
			request.Proxy.Credentials = _credentialProvider.GetCredentials(request, CredentialType.ProxyCredentials, _proxyCredentialsRetryCount > 0);
			_continueIfFailed = request.Proxy.Credentials != null;
			_proxyCredentialsRetryCount++;
		}
		else if (_previousStatusCode == HttpStatusCode.Unauthorized)
		{
			SetCredentialsOnAuthorizationError(request);
		}
		SetKeepAliveHeaders(request, _previousResponse);
		if (_usingSTSAuth)
		{
			STSAuthHelper.PrepareSTSRequest(request);
		}
		request.Credentials = request.Credentials.AsCredentialCache(request.RequestUri);
	}

	private void SetCredentialsOnAuthorizationError(HttpWebRequest request)
	{
		if (!_usingSTSAuth)
		{
			bool flag = _previousResponse.AuthType != null && _previousResponse.AuthType.IndexOf("Basic", StringComparison.OrdinalIgnoreCase) != -1;
			if (_disableBuffering && flag && !_basicAuthIsUsedInPreviousRequest)
			{
				request.Credentials = _credentialCache.GetCredentials(request.RequestUri);
			}
			if (request.Credentials == null)
			{
				request.Credentials = _credentialProvider.GetCredentials(request, CredentialType.RequestCredentials, _credentialsRetryCount > 0);
			}
			_continueIfFailed = request.Credentials != null;
			_credentialsRetryCount++;
		}
	}

	private static IHttpWebResponse GetResponse(WebResponse response)
	{
		if (!(response is IHttpWebResponse result))
		{
			if (!(response is HttpWebResponse response2))
			{
				return null;
			}
			return new HttpWebResponseWrapper(response2);
		}
		return result;
	}

	private static bool IsAuthenticationResponse(IHttpWebResponse response)
	{
		if (response.StatusCode != HttpStatusCode.Unauthorized)
		{
			return response.StatusCode == HttpStatusCode.ProxyAuthenticationRequired;
		}
		return true;
	}

	private static void SetKeepAliveHeaders(HttpWebRequest request, IHttpWebResponse previousResponse)
	{
		if (previousResponse == null || !IsNtlmOrKerberos(previousResponse.AuthType))
		{
			request.KeepAlive = false;
			request.ProtocolVersion = HttpVersion.Version10;
		}
	}

	private static bool ShouldKeepAliveBeUsedInRequest(HttpWebRequest request, IHttpWebResponse response)
	{
		if (request == null)
		{
			throw new ArgumentNullException("request");
		}
		if (response == null)
		{
			throw new ArgumentNullException("response");
		}
		if (!request.KeepAlive)
		{
			return IsNtlmOrKerberos(response.AuthType);
		}
		return false;
	}

	private static bool IsNtlmOrKerberos(string authType)
	{
		if (string.IsNullOrEmpty(authType))
		{
			return false;
		}
		if (authType.IndexOf("NTLM", StringComparison.OrdinalIgnoreCase) == -1)
		{
			return authType.IndexOf("Kerberos", StringComparison.OrdinalIgnoreCase) != -1;
		}
		return true;
	}
}
