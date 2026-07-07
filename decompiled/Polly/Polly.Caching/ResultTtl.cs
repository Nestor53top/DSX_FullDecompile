using System;

namespace Polly.Caching;

public class ResultTtl<TResult> : ITtlStrategy<TResult>
{
	private readonly Func<Context, TResult, Ttl> _ttlFunc;

	public ResultTtl(Func<TResult, Ttl> ttlFunc)
	{
		if (ttlFunc == null)
		{
			throw new ArgumentNullException("ttlFunc");
		}
		_ttlFunc = (Context context, TResult result) => ttlFunc(result);
	}

	public ResultTtl(Func<Context, TResult, Ttl> ttlFunc)
	{
		_ttlFunc = ttlFunc ?? throw new ArgumentNullException("ttlFunc");
	}

	public Ttl GetTtl(Context context, TResult result)
	{
		return _ttlFunc(context, result);
	}
}
