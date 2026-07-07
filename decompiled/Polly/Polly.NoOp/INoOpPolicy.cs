namespace Polly.NoOp;

public interface INoOpPolicy : IsPolicy
{
}
public interface INoOpPolicy<TResult> : INoOpPolicy, IsPolicy
{
}
