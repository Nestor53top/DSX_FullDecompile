using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using NuGet.Resources;

namespace NuGet;

[CLSCompliant(false)]
internal class DataServiceQueryWrapper<T> : IDataServiceQuery<T>, IDataServiceQuery
{
	private const int MaxUrlLength = 2048;

	private readonly DataServiceQuery _query;

	private readonly IDataServiceContext _context;

	private readonly Type _concreteType;

	public DataServiceQueryWrapper(IDataServiceContext context, DataServiceQuery query)
		: this(context, query, typeof(T))
	{
	}

	public DataServiceQueryWrapper(IDataServiceContext context, DataServiceQuery query, Type concreteType)
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
		_concreteType = concreteType;
	}

	public bool RequiresBatch(Expression expression)
	{
		return GetRequestUri(expression).AbsoluteUri.Length >= 2048;
	}

	public DataServiceRequest GetRequest(Expression expression)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		return (DataServiceRequest)_query.Provider.CreateQuery(GetInnerExpression(expression));
	}

	public virtual Uri GetRequestUri(Expression expression)
	{
		return GetRequest(expression).RequestUri;
	}

	public TResult Execute<TResult>(Expression expression)
	{
		return Execute(() => _query.Provider.Execute<TResult>(GetInnerExpression(expression)));
	}

	public object Execute(Expression expression)
	{
		return Execute(() => _query.Provider.Execute(GetInnerExpression(expression)));
	}

	public IDataServiceQuery<TElement> CreateQuery<TElement>(Expression expression)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		expression = GetInnerExpression(expression);
		DataServiceQuery query = (DataServiceQuery)_query.Provider.CreateQuery<TElement>(expression);
		return new DataServiceQueryWrapper<TElement>(_context, query, typeof(T));
	}

	public IQueryable<T> AsQueryable()
	{
		return (IQueryable<T>)_query;
	}

	public IEnumerator<T> GetEnumerator()
	{
		return GetAll().GetEnumerator();
	}

	private IEnumerable<T> GetAll()
	{
		DataServiceQuery val = _query;
		if (typeof(T) == typeof(IPackage))
		{
			val = (DataServiceQuery)_query.Provider.CreateQuery<DataServicePackage>(_query.Expression).Cast<DataServicePackage>();
		}
		IEnumerable results = Execute((Func<IEnumerable>)val.Execute);
		DataServiceQueryContinuation continuation;
		do
		{
			lock (_context)
			{
				foreach (T item in results)
				{
					yield return item;
				}
			}
			continuation = ((QueryOperationResponse)results).GetContinuation();
			if (continuation != null)
			{
				results = _context.Execute<T>(_concreteType, continuation);
			}
		}
		while (continuation != null);
	}

	private Expression GetInnerExpression(Expression expression)
	{
		return QueryableUtility.ReplaceQueryableExpression((IQueryable)_query, expression);
	}

	public override string ToString()
	{
		return ((object)_query).ToString();
	}

	private TResult Execute<TResult>(Func<TResult> action)
	{
		try
		{
			return action();
		}
		catch (Exception ex)
		{
			string text = ExtractMessageFromClientException(ex);
			if (!string.IsNullOrEmpty(text))
			{
				throw new InvalidOperationException(text, ex);
			}
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.InvalidFeed, new object[1] { _context.BaseUri }), ex);
		}
	}

	private static string ExtractMessageFromClientException(Exception exception)
	{
		DataServiceQueryException ex = (DataServiceQueryException)(object)((exception is DataServiceQueryException) ? exception : null);
		if (ex != null && ((Exception)(object)ex).InnerException != null)
		{
			Exception? innerException = ((Exception)(object)ex).InnerException;
			DataServiceClientException ex2 = (DataServiceClientException)(object)((innerException is DataServiceClientException) ? innerException : null);
			if (ex != null && XmlUtility.TryParseDocument(((Exception)(object)ex2).Message, out var document) && document.Root.Name.LocalName.Equals("error", StringComparison.OrdinalIgnoreCase))
			{
				return ((XContainer)(object)document.Root).GetOptionalElementValue("message");
			}
		}
		return null;
	}
}
