namespace Polly;

public static class IAsyncPolicyExtensions
{
	public static IAsyncPolicy<TResult> AsAsyncPolicy<TResult>(this IAsyncPolicy policy)
	{
		return policy.WrapAsync(Policy.NoOpAsync<TResult>());
	}
}
