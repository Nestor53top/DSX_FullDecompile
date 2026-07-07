namespace Polly.Fallback;

public interface IFallbackPolicy : IsPolicy
{
}
public interface IFallbackPolicy<TResult> : IFallbackPolicy, IsPolicy
{
}
