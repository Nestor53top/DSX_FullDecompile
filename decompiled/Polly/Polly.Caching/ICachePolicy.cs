namespace Polly.Caching;

public interface ICachePolicy : IsPolicy
{
}
public interface ICachePolicy<TResult> : ICachePolicy, IsPolicy
{
}
