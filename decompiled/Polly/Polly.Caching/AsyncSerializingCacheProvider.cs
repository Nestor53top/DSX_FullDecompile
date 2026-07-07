using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Caching;

public class AsyncSerializingCacheProvider<TSerialized> : IAsyncCacheProvider
{
	private readonly IAsyncCacheProvider<TSerialized> _wrappedCacheProvider;

	private readonly ICacheItemSerializer<object, TSerialized> _serializer;

	public AsyncSerializingCacheProvider(IAsyncCacheProvider<TSerialized> wrappedCacheProvider, ICacheItemSerializer<object, TSerialized> serializer)
	{
		_wrappedCacheProvider = wrappedCacheProvider ?? throw new ArgumentNullException("wrappedCacheProvider");
		_serializer = serializer ?? throw new ArgumentNullException("serializer");
	}

	public async Task<(bool, object)> TryGetAsync(string key, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		var (flag, objectToDeserialize) = await _wrappedCacheProvider.TryGetAsync(key, cancellationToken, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext);
		return (flag, flag ? _serializer.Deserialize(objectToDeserialize) : null);
	}

	public async Task PutAsync(string key, object value, Ttl ttl, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		await _wrappedCacheProvider.PutAsync(key, _serializer.Serialize(value), ttl, cancellationToken, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext);
	}
}
public class AsyncSerializingCacheProvider<TResult, TSerialized> : IAsyncCacheProvider<TResult>
{
	private readonly IAsyncCacheProvider<TSerialized> _wrappedCacheProvider;

	private readonly ICacheItemSerializer<TResult, TSerialized> _serializer;

	public AsyncSerializingCacheProvider(IAsyncCacheProvider<TSerialized> wrappedCacheProvider, ICacheItemSerializer<TResult, TSerialized> serializer)
	{
		_wrappedCacheProvider = wrappedCacheProvider ?? throw new ArgumentNullException("wrappedCacheProvider");
		_serializer = serializer ?? throw new ArgumentNullException("serializer");
	}

	public async Task<(bool, TResult)> TryGetAsync(string key, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		var (flag, objectToDeserialize) = await _wrappedCacheProvider.TryGetAsync(key, cancellationToken, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext);
		return (flag, flag ? _serializer.Deserialize(objectToDeserialize) : default(TResult));
	}

	public async Task PutAsync(string key, TResult value, Ttl ttl, CancellationToken cancellationToken, bool continueOnCapturedContext)
	{
		await _wrappedCacheProvider.PutAsync(key, _serializer.Serialize(value), ttl, cancellationToken, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext);
	}
}
