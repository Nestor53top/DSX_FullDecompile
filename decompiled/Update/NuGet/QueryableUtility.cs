using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NuGet;

internal static class QueryableUtility
{
	private class ExpressionRewriter : ExpressionVisitor
	{
		private readonly IQueryable _query;

		public ExpressionRewriter(IQueryable query)
		{
			_query = query;
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			if (typeof(IQueryable).IsAssignableFrom(node.Type))
			{
				return _query.Expression;
			}
			return base.VisitConstant(node);
		}
	}

	private static readonly string[] _orderMethods = new string[4] { "OrderBy", "ThenBy", "OrderByDescending", "ThenByDescending" };

	private static readonly MethodInfo[] _methods = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public);

	private static MethodInfo GetQueryableMethod(Expression expression)
	{
		if (expression.NodeType == ExpressionType.Call)
		{
			MethodCallExpression methodCallExpression = (MethodCallExpression)expression;
			if (methodCallExpression.Method.IsStatic && methodCallExpression.Method.DeclaringType == typeof(Queryable))
			{
				return methodCallExpression.Method.GetGenericMethodDefinition();
			}
		}
		return null;
	}

	public static bool IsQueryableMethod(Expression expression, string method)
	{
		return _methods.Where((MethodInfo m) => m.Name == method).Contains(GetQueryableMethod(expression));
	}

	public static bool IsOrderingMethod(Expression expression)
	{
		return _orderMethods.Any((string method) => IsQueryableMethod(expression, method));
	}

	public static Expression ReplaceQueryableExpression(IQueryable query, Expression expression)
	{
		return new ExpressionRewriter(query).Visit(expression);
	}

	public static Type FindGenericType(Type definition, Type type)
	{
		while (type != null && type != typeof(object))
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == definition)
			{
				return type;
			}
			if (definition.IsInterface)
			{
				Type[] interfaces = type.GetInterfaces();
				foreach (Type type2 in interfaces)
				{
					Type type3 = FindGenericType(definition, type2);
					if (type3 != null)
					{
						return type3;
					}
				}
			}
			type = type.BaseType;
		}
		return null;
	}
}
