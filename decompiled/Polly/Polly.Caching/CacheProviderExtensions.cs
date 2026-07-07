namespace Polly.Caching;

public static class CacheProviderExtensions
{
	public static ISyncCacheProvider<TCacheFormat> For<TCacheFormat>(this ISyncCacheProvider nonGenericCacheProvider)
	{
		return new GenericCacheProvider<TCacheFormat>(nonGenericCacheProvider);
	}

	public static IAsyncCacheProvider<TCacheFormat> AsyncFor<TCacheFormat>(this IAsyncCacheProvider nonGenericCacheProvider)
	{
		return new AsyncGenericCacheProvider<TCacheFormat>(nonGenericCacheProvider);
	}

	public static SerializingCacheProvider<TSerialized> WithSerializer<TSerialized>(this ISyncCacheProvider<TSerialized> cacheProvider, ICacheItemSerializer<object, TSerialized> serializer)
	{
		return new SerializingCacheProvider<TSerialized>(cacheProvider, serializer);
	}

	public static SerializingCacheProvider<TResult, TSerialized> WithSerializer<TResult, TSerialized>(this ISyncCacheProvider<TSerialized> cacheProvider, ICacheItemSerializer<TResult, TSerialized> serializer)
	{
		return new SerializingCacheProvider<TResult, TSerialized>(cacheProvider, serializer);
	}

	public static AsyncSerializingCacheProvider<TSerialized> WithSerializer<TSerialized>(this IAsyncCacheProvider<TSerialized> cacheProvider, ICacheItemSerializer<object, TSerialized> serializer)
	{
		return new AsyncSerializingCacheProvider<TSerialized>(cacheProvider, serializer);
	}

	public static AsyncSerializingCacheProvider<TResult, TSerialized> WithSerializer<TResult, TSerialized>(this IAsyncCacheProvider<TSerialized> cacheProvider, ICacheItemSerializer<TResult, TSerialized> serializer)
	{
		return new AsyncSerializingCacheProvider<TResult, TSerialized>(cacheProvider, serializer);
	}
}
