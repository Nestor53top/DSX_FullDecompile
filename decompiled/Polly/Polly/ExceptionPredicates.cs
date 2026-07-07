using System;
using System.Collections.Generic;
using System.Linq;

namespace Polly;

public class ExceptionPredicates
{
	private List<ExceptionPredicate> _predicates;

	public static readonly ExceptionPredicates None = new ExceptionPredicates();

	internal void Add(ExceptionPredicate predicate)
	{
		_predicates = _predicates ?? new List<ExceptionPredicate>();
		_predicates.Add(predicate);
	}

	public Exception FirstMatchOrDefault(Exception ex)
	{
		return _predicates?.Select((ExceptionPredicate predicate) => predicate(ex)).FirstOrDefault((Exception e) => e != null);
	}
}
