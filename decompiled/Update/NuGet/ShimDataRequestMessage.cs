using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Globalization;
using System.IO;
using System.Net;
using Microsoft.Data.OData;

namespace NuGet;

internal class ShimDataRequestMessage : IODataRequestMessage
{
	private SendingRequest2EventArgs _args;

	public HttpWebRequest WebRequest { get; private set; }

	public IEnumerable<KeyValuePair<string, string>> Headers
	{
		get
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			string[] allKeys = WebRequest.Headers.AllKeys;
			foreach (string text in allKeys)
			{
				list.Add(new KeyValuePair<string, string>(text, WebRequest.Headers.Get(text)));
			}
			return list;
		}
	}

	public Uri Url
	{
		get
		{
			return WebRequest.RequestUri;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public string Method
	{
		get
		{
			return WebRequest.Method;
		}
		set
		{
			WebRequest.Method = value;
		}
	}

	public ShimDataRequestMessage(SendingRequest2EventArgs args)
	{
		_args = args;
		WebRequest = ShimWebHelpers.AddHeaders(System.Net.WebRequest.CreateHttp(_args.RequestMessage.Url), _args.RequestMessage.Headers);
		WebRequest.Method = _args.RequestMessage.Method;
	}

	public ShimDataRequestMessage(DataServiceClientRequestMessageArgs args)
	{
		WebRequest = ShimWebHelpers.AddHeaders(System.Net.WebRequest.CreateHttp(args.RequestUri), args.Headers);
		WebRequest.Method = args.Method;
	}

	public string GetHeader(string headerName)
	{
		return WebRequest.Headers.Get(headerName);
	}

	public Stream GetStream()
	{
		return WebRequest.GetRequestStream();
	}

	public void SetHeader(string headerName, string headerValue)
	{
		if (StringComparer.OrdinalIgnoreCase.Equals(headerName, "Content-Length"))
		{
			WebRequest.ContentLength = long.Parse(headerValue, CultureInfo.InvariantCulture.NumberFormat);
		}
		else
		{
			WebRequest.Headers.Set(headerName, headerValue);
		}
	}
}
