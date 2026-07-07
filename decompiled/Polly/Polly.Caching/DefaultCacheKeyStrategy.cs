namespace Polly.Caching;

public class DefaultCacheKeyStrategy : ICacheKeyStrategy
{
	public static readonly ICacheKeyStrategy Instance = new DefaultCacheKeyStrategy();

	public string GetCacheKey(Context context)
	{
		return context.OperationKey;
	}
}
