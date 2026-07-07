using System;
using System.Globalization;
using System.Net;
using NuGet.Resources;

namespace NuGet;

internal class RedirectedHttpClient : HttpClient
{
	private const string RedirectedClientCacheKey = "RedirectedHttpClient|";

	private readonly Uri _originalUri;

	private readonly MemoryCache _memoryCache;

	public override Uri Uri => CachedClient.Uri;

	public override Uri OriginalUri => _originalUri;

	internal IHttpClient CachedClient
	{
		get
		{
			string cacheKey = GetCacheKey();
			try
			{
				return _memoryCache.GetOrAdd(cacheKey, EnsureClient, TimeSpan.FromHours(1.0));
			}
			catch (Exception)
			{
				_memoryCache.Remove(cacheKey);
				throw;
			}
		}
	}

	public RedirectedHttpClient(Uri uri)
		: this(uri, MemoryCache.Instance)
	{
	}

	public RedirectedHttpClient(Uri uri, MemoryCache memoryCache)
		: base(uri)
	{
		_originalUri = uri;
		_memoryCache = memoryCache;
	}

	public override WebResponse GetResponse()
	{
		return CachedClient.GetResponse();
	}

	private string GetCacheKey()
	{
		return "RedirectedHttpClient|" + _originalUri.OriginalString;
	}

	protected internal virtual IHttpClient EnsureClient()
	{
		HttpClient client = new HttpClient(_originalUri);
		return new HttpClient(GetResponseUri(client));
	}

	private Uri GetResponseUri(HttpClient client)
	{
		using WebResponse webResponse = client.GetResponse();
		if (webResponse == null)
		{
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.UnableToResolveUri, new object[1] { Uri }));
		}
		return webResponse.ResponseUri;
	}
}
