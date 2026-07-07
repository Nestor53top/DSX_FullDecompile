using System;
using System.Data.Services.Client;
using System.Net;

namespace NuGet;

internal sealed class HttpShim
{
	private static HttpShim _instance;

	private Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage> _dataServiceHandler;

	private Func<WebRequest, WebResponse> _webHandler;

	public static HttpShim Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new HttpShim();
			}
			return _instance;
		}
	}

	internal HttpShim()
	{
	}

	internal WebResponse ShimWebRequest(WebRequest request)
	{
		WebResponse webResponse = null;
		InitializeRequest(request);
		if (_webHandler != null)
		{
			return _webHandler(request);
		}
		return request.GetResponse();
	}

	internal DataServiceClientRequestMessage ShimDataServiceRequest(DataServiceClientRequestMessageArgs args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		DataServiceClientRequestMessage val = null;
		val = (DataServiceClientRequestMessage)((_dataServiceHandler == null) ? ((object)new HttpWebRequestMessage(args)) : ((object)_dataServiceHandler(args)));
		InitializeMessage(val);
		return val;
	}

	public void SetWebRequestHandler(Func<WebRequest, WebResponse> handler)
	{
		_webHandler = handler;
	}

	public void SetDataServiceRequestHandler(Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage> handler)
	{
		_dataServiceHandler = handler;
	}

	public void ClearHandlers()
	{
		_dataServiceHandler = null;
		_webHandler = null;
	}

	private static void InitializeMessage(DataServiceClientRequestMessage message)
	{
		IShimWebRequest shimWebRequest = message as IShimWebRequest;
		HttpWebRequestMessage val = (HttpWebRequestMessage)(object)((message is HttpWebRequestMessage) ? message : null);
		if (val != null)
		{
			InitializeRequest(val.HttpWebRequest);
		}
		else if (shimWebRequest != null)
		{
			InitializeRequest(shimWebRequest.Request);
		}
	}

	private static void InitializeRequest(WebRequest request)
	{
		try
		{
			SetCredentialsAndProxy(request);
			InitializeRequestProperties(request);
		}
		catch (InvalidOperationException)
		{
		}
	}

	private static void SetCredentialsAndProxy(WebRequest request)
	{
		if (request.Credentials == null)
		{
			request.Credentials = CredentialStore.Instance.GetCredentials(request.RequestUri);
		}
		if (request.Proxy == null)
		{
			request.Proxy = ProxyCache.Instance.GetProxy(request.RequestUri);
		}
		STSAuthHelper.PrepareSTSRequest(request);
	}

	private static void InitializeRequestProperties(WebRequest request)
	{
		if (request is HttpWebRequest httpWebRequest)
		{
			httpWebRequest.UserAgent = HttpUtility.CreateUserAgentString("NuGet Shim");
			httpWebRequest.CookieContainer = new CookieContainer();
			httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
		}
	}
}
