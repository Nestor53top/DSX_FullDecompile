namespace Polly.Retry;

public interface IRetryPolicy : IsPolicy
{
}
public interface IRetryPolicy<TResult> : IRetryPolicy, IsPolicy
{
}
