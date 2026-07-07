namespace Polly.Caching;

internal static class TtlStrategyExtensions
{
	internal static ITtlStrategy<TResult> For<TResult>(this ITtlStrategy ttlStrategy)
	{
		return new GenericTtlStrategy<TResult>(ttlStrategy);
	}
}
