using System.Collections.Generic;
using System.Linq;

namespace Polly;

public class ResultPredicates<TResult>
{
	private List<ResultPredicate<TResult>> _predicates;

	public static readonly ResultPredicates<TResult> None = new ResultPredicates<TResult>();

	internal void Add(ResultPredicate<TResult> predicate)
	{
		_predicates = _predicates ?? new List<ResultPredicate<TResult>>();
		_predicates.Add(predicate);
	}

	public bool AnyMatch(TResult result)
	{
		if (_predicates == null)
		{
			return false;
		}
		return _predicates.Any((ResultPredicate<TResult> predicate) => predicate(result));
	}
}
