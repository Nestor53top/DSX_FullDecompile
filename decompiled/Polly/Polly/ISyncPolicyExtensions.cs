namespace Polly;

public static class ISyncPolicyExtensions
{
	public static ISyncPolicy<TResult> AsPolicy<TResult>(this ISyncPolicy policy)
	{
		return policy.Wrap(Policy.NoOp<TResult>());
	}
}
