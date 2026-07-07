using System;
using System.IO;
using System.Net;

namespace NuGet;

internal class HttpClient : IHttpClient, IHttpClientEvents, IProgressProvider
{
	private static ICredentialProvider _credentialProvider;

	private Uri _uri;

	public string UserAgent { get; set; }

	public virtual Uri Uri
	{
		get
		{
			return _uri;
		}
		set
		{
			_uri = value;
		}
	}

	public virtual Uri OriginalUri => _uri;

	public string Method { get; set; }

	public string ContentType { get; set; }

	public bool AcceptCompression { get; set; }

	public bool DisableBuffering { get; set; }

	public static ICredentialProvider DefaultCredentialProvider
	{
		get
		{
			return _credentialProvider ?? NullCredentialProvider.Instance;
		}
		set
		{
			_credentialProvider = value;
		}
	}

	public event EventHandler<ProgressEventArgs> ProgressAvailable = delegate
	{
	};

	public event EventHandler<WebRequestEventArgs> SendingRequest = delegate
	{
	};

	public HttpClient(Uri uri)
	{
		if (uri == null)
		{
			throw new ArgumentNullException("uri");
		}
		_uri = uri;
	}

	public virtual WebResponse GetResponse()
	{
		return new RequestHelper(delegate
		{
			WebRequest webRequest = WebRequest.Create(Uri);
			InitializeRequestProperties(webRequest);
			return webRequest;
		}, RaiseSendingRequest, ProxyCache.Instance, CredentialStore.Instance, DefaultCredentialProvider, DisableBuffering).GetResponse();
	}

	public void InitializeRequest(WebRequest request)
	{
		InitializeRequestProperties(request);
		TrySetCredentialsAndProxy(request);
		RaiseSendingRequest(request);
	}

	private void TrySetCredentialsAndProxy(WebRequest request)
	{
		request.Credentials = CredentialStore.Instance.GetCredentials(Uri);
		request.Proxy = ProxyCache.Instance.GetProxy(Uri);
		STSAuthHelper.PrepareSTSRequest(request);
	}

	private void InitializeRequestProperties(WebRequest request)
	{
		if (request is HttpWebRequest httpWebRequest)
		{
			httpWebRequest.UserAgent = UserAgent ?? HttpUtility.CreateUserAgentString("NuGet");
			httpWebRequest.CookieContainer = new CookieContainer();
			if (AcceptCompression)
			{
				httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			}
		}
		if (!string.IsNullOrEmpty(ContentType))
		{
			request.ContentType = ContentType;
		}
		if (!string.IsNullOrEmpty(Method))
		{
			request.Method = Method;
		}
	}

	public void DownloadData(Stream targetStream)
	{
		using WebResponse webResponse = GetResponse();
		int num = (int)webResponse.ContentLength;
		using Stream stream = webResponse.GetResponseStream();
		if (num < 0)
		{
			stream.CopyTo(targetStream);
			OnProgressAvailable(100);
			return;
		}
		int num2 = 0;
		byte[] buffer = new byte[4096];
		while (num2 < num)
		{
			int num3 = stream.Read(buffer, 0, Math.Min(num - num2, 4096));
			if (num3 == 0)
			{
				break;
			}
			targetStream.Write(buffer, 0, num3);
			num2 += num3;
			OnProgressAvailable(num2 * 100 / num);
		}
	}

	private void OnProgressAvailable(int percentage)
	{
		this.ProgressAvailable(this, new ProgressEventArgs(percentage));
	}

	private void RaiseSendingRequest(WebRequest webRequest)
	{
		this.SendingRequest(this, new WebRequestEventArgs(webRequest));
	}
}
