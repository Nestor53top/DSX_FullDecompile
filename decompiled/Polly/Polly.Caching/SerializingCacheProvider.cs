using System;

namespace Polly.Caching;

public class SerializingCacheProvider<TSerialized> : ISyncCacheProvider
{
	private readonly ISyncCacheProvider<TSerialized> _wrappedCacheProvider;

	private readonly ICacheItemSerializer<object, TSerialized> _serializer;

	public SerializingCacheProvider(ISyncCacheProvider<TSerialized> wrappedCacheProvider, ICacheItemSerializer<object, TSerialized> serializer)
	{
		_wrappedCacheProvider = wrappedCacheProvider ?? throw new ArgumentNullException("wrappedCacheProvider");
		_serializer = serializer ?? throw new ArgumentNullException("serializer");
	}

	public (bool, object) TryGet(string key)
	{
		var (flag, objectToDeserialize) = _wrappedCacheProvider.TryGet(key);
		return (flag, flag ? _serializer.Deserialize(objectToDeserialize) : null);
	}

	public void Put(string key, object value, Ttl ttl)
	{
		_wrappedCacheProvider.Put(key, _serializer.Serialize(value), ttl);
	}
}
public class SerializingCacheProvider<TResult, TSerialized> : ISyncCacheProvider<TResult>
{
	private readonly ISyncCacheProvider<TSerialized> _wrappedCacheProvider;

	private readonly ICacheItemSerializer<TResult, TSerialized> _serializer;

	public SerializingCacheProvider(ISyncCacheProvider<TSerialized> wrappedCacheProvider, ICacheItemSerializer<TResult, TSerialized> serializer)
	{
		_wrappedCacheProvider = wrappedCacheProvider ?? throw new ArgumentNullException("wrappedCacheProvider");
		_serializer = serializer ?? throw new ArgumentNullException("serializer");
	}

	public (bool, TResult) TryGet(string key)
	{
		var (flag, objectToDeserialize) = _wrappedCacheProvider.TryGet(key);
		return (flag, flag ? _serializer.Deserialize(objectToDeserialize) : default(TResult));
	}

	public void Put(string key, TResult value, Ttl ttl)
	{
		_wrappedCacheProvider.Put(key, _serializer.Serialize(value), ttl);
	}
}
