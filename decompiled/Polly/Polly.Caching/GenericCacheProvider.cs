using System;

namespace Polly.Caching;

internal class GenericCacheProvider<TCacheFormat> : ISyncCacheProvider<TCacheFormat>
{
	private readonly ISyncCacheProvider _wrappedCacheProvider;

	internal GenericCacheProvider(ISyncCacheProvider nonGenericCacheProvider)
	{
		_wrappedCacheProvider = nonGenericCacheProvider ?? throw new ArgumentNullException("nonGenericCacheProvider");
	}

	(bool, TCacheFormat) ISyncCacheProvider<TCacheFormat>.TryGet(string key)
	{
		var (item, obj) = _wrappedCacheProvider.TryGet(key);
		return (item, (TCacheFormat)(obj ?? ((object)default(TCacheFormat))));
	}

	void ISyncCacheProvider<TCacheFormat>.Put(string key, TCacheFormat value, Ttl ttl)
	{
		_wrappedCacheProvider.Put(key, value, ttl);
	}
}
