namespace Polly.Wrap;

public interface IPolicyWrap : IsPolicy
{
	IsPolicy Outer { get; }

	IsPolicy Inner { get; }
}
public interface IPolicyWrap<TResult> : IPolicyWrap, IsPolicy
{
}
