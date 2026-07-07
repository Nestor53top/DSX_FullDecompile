namespace Polly.Caching;

public interface ICacheKeyStrategy
{
	string GetCacheKey(Context context);
}
