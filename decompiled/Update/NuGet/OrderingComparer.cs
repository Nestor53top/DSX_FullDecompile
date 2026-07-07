using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NuGet.Resources;

namespace NuGet;

internal class OrderingComparer<TElement> : ExpressionVisitor, IComparer<TElement>
{
	private class Ordering<T>
	{
		public Func<T, IComparable> Extractor { get; set; }

		public bool Descending { get; set; }
	}

	private readonly Expression _expression;

	private readonly Dictionary<ParameterExpression, ParameterExpression> _parameters = new Dictionary<ParameterExpression, ParameterExpression>();

	private bool _inOrderExpression;

	private Stack<Ordering<TElement>> _orderings;

	public bool CanCompare
	{
		get
		{
			EnsureOrderings();
			return _orderings.Count > 0;
		}
	}

	public OrderingComparer(Expression expression)
	{
		_expression = expression;
	}

	protected override Expression VisitMethodCall(MethodCallExpression node)
	{
		if (QueryableUtility.IsOrderingMethod(node))
		{
			_inOrderExpression = true;
			Expression<Func<TElement, IComparable>> expression = (Expression<Func<TElement, IComparable>>)((UnaryExpression)Visit(node.Arguments[1])).Operand;
			_orderings.Push(new Ordering<TElement>
			{
				Descending = node.Method.Name.EndsWith("Descending", StringComparison.OrdinalIgnoreCase),
				Extractor = expression.Compile()
			});
			_inOrderExpression = false;
		}
		return base.VisitMethodCall(node);
	}

	protected override Expression VisitLambda<T>(Expression<T> node)
	{
		if (_inOrderExpression)
		{
			UnaryExpression body = Expression.Convert(Visit(node.Body), typeof(IComparable));
			IEnumerable<ParameterExpression> source = node.Parameters.Select(Visit).Cast<ParameterExpression>();
			return Expression.Lambda<Func<TElement, IComparable>>(body, source.ToArray());
		}
		return base.VisitLambda(node);
	}

	protected override Expression VisitParameter(ParameterExpression node)
	{
		if (_inOrderExpression)
		{
			if (!_parameters.TryGetValue(node, out var value))
			{
				value = Expression.Parameter(node.Type);
				_parameters[node] = value;
			}
			return value;
		}
		return base.VisitParameter(node);
	}

	public int Compare(TElement x, TElement y)
	{
		if (!CanCompare)
		{
			throw new InvalidOperationException(NuGetResources.AggregateQueriesRequireOrder);
		}
		int num = 0;
		foreach (Ordering<TElement> ordering in _orderings)
		{
			IComparable comparable = ordering.Extractor(x);
			IComparable comparable2 = ordering.Extractor(y);
			if (comparable == null && comparable2 == null)
			{
				continue;
			}
			num = comparable.CompareTo(comparable2);
			if (num != 0)
			{
				if (ordering.Descending)
				{
					return -num;
				}
				return num;
			}
		}
		return num;
	}

	private void EnsureOrderings()
	{
		if (_orderings == null)
		{
			_orderings = new Stack<Ordering<TElement>>();
			Visit(_expression);
		}
	}
}
