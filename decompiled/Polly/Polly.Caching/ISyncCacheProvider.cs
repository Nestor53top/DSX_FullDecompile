namespace Polly.Caching;

public interface ISyncCacheProvider
{
	(bool, object) TryGet(string key);

	void Put(string key, object value, Ttl ttl);
}
public interface ISyncCacheProvider<TResult>
{
	(bool, TResult) TryGet(string key);

	void Put(string key, TResult value, Ttl ttl);
}
