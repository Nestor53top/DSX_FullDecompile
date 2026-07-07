using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace NuGet;

internal class AggregateQuery<TVal> : IQueryable<TVal>, IEnumerable<TVal>, IEnumerable, IQueryable, IQueryProvider, IOrderedQueryable<TVal>, IOrderedQueryable
{
	private class TaskResult
	{
		public LazyQueue<TVal> Queue { get; set; }

		public bool HasValue { get; set; }

		public TVal Value { get; set; }
	}

	private const int QueryCacheSize = 30;

	private readonly IEnumerable<IQueryable<TVal>> _queryables;

	private readonly Expression _expression;

	private readonly IEqualityComparer<TVal> _equalityComparer;

	private readonly IList<IEnumerable<TVal>> _subQueries;

	private readonly bool _ignoreFailures;

	private readonly ILogger _logger;

	public Type ElementType => typeof(TVal);

	public Expression Expression => _expression;

	public IQueryProvider Provider => this;

	public AggregateQuery(IEnumerable<IQueryable<TVal>> queryables, IEqualityComparer<TVal> equalityComparer, ILogger logger, bool ignoreFailures)
	{
		_queryables = queryables;
		_equalityComparer = equalityComparer;
		_expression = Expression.Constant(this);
		_ignoreFailures = ignoreFailures;
		_logger = logger;
		_subQueries = GetSubQueries(_expression);
	}

	private AggregateQuery(IEnumerable<IQueryable<TVal>> queryables, IEqualityComparer<TVal> equalityComparer, IList<IEnumerable<TVal>> subQueries, Expression expression, ILogger logger, bool ignoreInvalidRepositories)
	{
		_queryables = queryables;
		_equalityComparer = equalityComparer;
		_expression = expression;
		_subQueries = subQueries;
		_ignoreFailures = ignoreInvalidRepositories;
		_logger = logger;
	}

	public IEnumerator<TVal> GetEnumerator()
	{
		IQueryable<TVal> queryable = GetAggregateEnumerable().AsQueryable();
		Expression expression = RewriteForAggregation(queryable, Expression);
		return queryable.Provider.CreateQuery<TVal>(expression).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
	{
		return (IQueryable<TElement>)CreateQuery(typeof(TElement), expression);
	}

	public IQueryable CreateQuery(Expression expression)
	{
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		Type type = QueryableUtility.FindGenericType(typeof(IQueryable<>), expression.Type);
		if (type == null)
		{
			throw new ArgumentException(string.Empty, "expression");
		}
		return CreateQuery(type, expression);
	}

	public TResult Execute<TResult>(Expression expression)
	{
		IQueryable<TResult> queryable = _queryables.Select((IQueryable<TVal> queryable2) => TryExecute<TResult>(queryable2, expression)).AsQueryable();
		if (QueryableUtility.IsQueryableMethod(expression, "Count"))
		{
			return (TResult)(object)queryable.Cast<int>().Sum();
		}
		return TryExecute<TResult>(queryable, expression);
	}

	public object Execute(Expression expression)
	{
		return Execute<object>(expression);
	}

	private IEnumerable<TVal> GetAggregateEnumerable()
	{
		OrderingComparer<TVal> orderingComparer = new OrderingComparer<TVal>(Expression);
		if (!orderingComparer.CanCompare)
		{
			return _subQueries.SelectMany((IEnumerable<TVal> query) => (!_ignoreFailures) ? query : query.SafeIterate()).Distinct(_equalityComparer);
		}
		return ReadOrderedQueues(orderingComparer);
	}

	private IEnumerable<TVal> ReadOrderedQueues(IComparer<TVal> comparer)
	{
		List<LazyQueue<TVal>> lazyQueues = _subQueries.Select((IEnumerable<TVal> query) => new LazyQueue<TVal>(query.GetEnumerator())).ToList();
		HashSet<TVal> seen = new HashSet<TVal>(_equalityComparer);
		do
		{
			TVal val = default(TVal);
			LazyQueue<TVal> minQueue = null;
			Task[] tasks;
			Task[] array = (tasks = lazyQueues.Select((LazyQueue<TVal> queue) => Task.Factory.StartNew(() => ReadQueue(queue))).ToArray());
			Task.WaitAll(tasks);
			Task<TaskResult>[] array2 = (Task<TaskResult>[])array;
			foreach (Task<TaskResult> task in array2)
			{
				if (task.Result.HasValue)
				{
					if (val == null || comparer.Compare(task.Result.Value, val) < 0)
					{
						val = task.Result.Value;
						minQueue = task.Result.Queue;
					}
				}
				else
				{
					lazyQueues.Remove(task.Result.Queue);
				}
			}
			if (lazyQueues.Any())
			{
				if (seen.Add(val))
				{
					yield return val;
				}
				minQueue.Dequeue();
			}
		}
		while (lazyQueues.Count > 0);
	}

	private TaskResult ReadQueue(LazyQueue<TVal> queue)
	{
		TaskResult taskResult = new TaskResult
		{
			Queue = queue
		};
		TVal element;
		if (_ignoreFailures)
		{
			try
			{
				taskResult.HasValue = queue.TryPeek(out element);
			}
			catch (Exception ex)
			{
				LogWarning(ex);
				element = default(TVal);
			}
		}
		else
		{
			taskResult.HasValue = queue.TryPeek(out element);
		}
		taskResult.Value = element;
		return taskResult;
	}

	private IList<IEnumerable<TVal>> GetSubQueries(Expression expression)
	{
		return _queryables.Select((IQueryable<TVal> query) => GetSubQuery(query, expression)).ToList();
	}

	private IQueryable CreateQuery(Type elementType, Expression expression)
	{
		ConstructorInfo constructorInfo = typeof(AggregateQuery<>).MakeGenericType(elementType).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single();
		IList<IEnumerable<TVal>> subQueries = _subQueries;
		if (QueryableUtility.IsQueryableMethod(expression, "Where") || QueryableUtility.IsOrderingMethod(expression))
		{
			subQueries = GetSubQueries(expression);
		}
		return (IQueryable)constructorInfo.Invoke(new object[6] { _queryables, _equalityComparer, subQueries, expression, _logger, _ignoreFailures });
	}

	private void LogWarning(Exception ex)
	{
		_logger.Log(MessageLevel.Warning, ExceptionUtility.Unwrap(ex).Message);
	}

	private static IEnumerable<TVal> GetSubQuery(IQueryable queryable, Expression expression)
	{
		expression = Rewrite(queryable, expression);
		return new BufferedEnumerable<TVal>(queryable.Provider.CreateQuery<TVal>(expression), 30);
	}

	private static TResult Execute<TResult>(IQueryable queryable, Expression expression)
	{
		return queryable.Provider.Execute<TResult>(Rewrite(queryable, expression));
	}

	private TResult TryExecute<TResult>(IQueryable queryable, Expression expression)
	{
		if (_ignoreFailures)
		{
			try
			{
				return Execute<TResult>(queryable, expression);
			}
			catch (Exception ex)
			{
				LogWarning(ex);
				return default(TResult);
			}
		}
		return Execute<TResult>(queryable, expression);
	}

	private static Expression RewriteForAggregation(IQueryable queryable, Expression expression)
	{
		return new ExpressionRewriter(queryable, new string[5] { "Where", "OrderBy", "OrderByDescending", "ThenBy", "ThenByDescending" }).Visit(expression);
	}

	private static Expression Rewrite(IQueryable queryable, Expression expression)
	{
		return new ExpressionRewriter(queryable, new string[2] { "Skip", "Take" }).Visit(expression);
	}
}
