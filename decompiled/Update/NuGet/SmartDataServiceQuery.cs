using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NuGet;

[CLSCompliant(false)]
internal class SmartDataServiceQuery<T> : IQueryable<T>, IEnumerable<T>, IEnumerable, IQueryable, IQueryProvider, IOrderedQueryable<T>, IOrderedQueryable
{
	private readonly IDataServiceContext _context;

	private readonly IDataServiceQuery _query;

	public Type ElementType => typeof(T);

	public Expression Expression { get; private set; }

	public IQueryProvider Provider => this;

	public SmartDataServiceQuery(IDataServiceContext context, string entitySetName)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		if (string.IsNullOrEmpty(entitySetName))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "entitySetName");
		}
		_context = context;
		_query = context.CreateQuery<T>(entitySetName);
		Expression = Expression.Constant(this);
	}

	public SmartDataServiceQuery(IDataServiceContext context, IDataServiceQuery query)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		if (query == null)
		{
			throw new ArgumentNullException("query");
		}
		_context = context;
		_query = query;
		Expression = Expression.Constant(this);
	}

	private SmartDataServiceQuery(IDataServiceContext context, IDataServiceQuery query, Expression expression)
	{
		_context = context;
		_query = query;
		Expression = expression;
	}

	public IEnumerator<T> GetEnumerator()
	{
		DataServiceRequest request = _query.GetRequest(Expression);
		if (_query.RequiresBatch(Expression))
		{
			return _context.ExecuteBatch<T>(request).GetEnumerator();
		}
		return _query.CreateQuery<T>(Expression).GetEnumerator();
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
		DataServiceRequest request = _query.GetRequest(expression);
		if (_query.RequiresBatch(expression))
		{
			return _context.ExecuteBatch<TResult>(request).FirstOrDefault();
		}
		return _query.Execute<TResult>(expression);
	}

	public object Execute(Expression expression)
	{
		DataServiceRequest request = _query.GetRequest(expression);
		if (_query.RequiresBatch(expression))
		{
			return _context.ExecuteBatch<object>(request).FirstOrDefault();
		}
		return _query.Execute(expression);
	}

	private IQueryable CreateQuery(Type elementType, Expression expression)
	{
		return (IQueryable)typeof(SmartDataServiceQuery<>).MakeGenericType(elementType).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single()
			.Invoke(new object[3] { _context, _query, expression });
	}

	public override string ToString()
	{
		return _query.CreateQuery<T>(Expression).ToString();
	}
}
