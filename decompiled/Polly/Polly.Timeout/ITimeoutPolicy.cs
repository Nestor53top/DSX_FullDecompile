namespace Polly.Timeout;

public interface ITimeoutPolicy : IsPolicy
{
}
public interface ITimeoutPolicy<TResult> : ITimeoutPolicy, IsPolicy
{
}
