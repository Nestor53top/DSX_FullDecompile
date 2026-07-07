namespace Polly.Caching;

public interface ITtlStrategy : ITtlStrategy<object>
{
}
public interface ITtlStrategy<TResult>
{
	Ttl GetTtl(Context context, TResult result);
}
