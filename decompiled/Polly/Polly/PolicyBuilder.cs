using System;
using System.ComponentModel;

namespace Polly;

public sealed class PolicyBuilder
{
	internal ExceptionPredicates ExceptionPredicates { get; }

	internal PolicyBuilder(ExceptionPredicate exceptionPredicate)
	{
		ExceptionPredicates = new ExceptionPredicates();
		ExceptionPredicates.Add(exceptionPredicate);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override string ToString()
	{
		return base.ToString();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Type GetType()
	{
		return base.GetType();
	}

	public PolicyBuilder Or<TException>() where TException : Exception
	{
		ExceptionPredicates.Add((Exception exception) => (!(exception is TException)) ? null : exception);
		return this;
	}

	public PolicyBuilder Or<TException>(Func<TException, bool> exceptionPredicate) where TException : Exception
	{
		ExceptionPredicates.Add((Exception exception) => (!(exception is TException arg) || !exceptionPredicate(arg)) ? null : exception);
		return this;
	}

	public PolicyBuilder OrInner<TException>() where TException : Exception
	{
		ExceptionPredicates.Add(HandleInner((Exception ex) => ex is TException));
		return this;
	}

	public PolicyBuilder OrInner<TException>(Func<TException, bool> exceptionPredicate) where TException : Exception
	{
		ExceptionPredicates.Add(HandleInner((Exception exception) => exception is TException arg && exceptionPredicate(arg)));
		return this;
	}

	internal static ExceptionPredicate HandleInner(Func<Exception, bool> predicate)
	{
		return delegate(Exception exception)
		{
			if (exception is AggregateException ex)
			{
				foreach (Exception innerException in ex.Flatten().InnerExceptions)
				{
					Exception ex2 = HandleInnerNested(predicate, innerException);
					if (ex2 != null)
					{
						return ex2;
					}
				}
			}
			return HandleInnerNested(predicate, exception);
		};
	}

	private static Exception HandleInnerNested(Func<Exception, bool> predicate, Exception current)
	{
		if (current == null)
		{
			return null;
		}
		if (predicate(current))
		{
			return current;
		}
		return HandleInnerNested(predicate, current.InnerException);
	}

	public PolicyBuilder<TResult> OrResult<TResult>(Func<TResult, bool> resultPredicate)
	{
		return new PolicyBuilder<TResult>(ExceptionPredicates).OrResult(resultPredicate);
	}

	public PolicyBuilder<TResult> OrResult<TResult>(TResult result)
	{
		return OrResult((TResult r) => (r != null && r.Equals(result)) || (r == null && result == null));
	}
}
public sealed class PolicyBuilder<TResult>
{
	internal ExceptionPredicates ExceptionPredicates { get; }

	internal ResultPredicates<TResult> ResultPredicates { get; }

	private PolicyBuilder()
	{
		ExceptionPredicates = new ExceptionPredicates();
		ResultPredicates = new ResultPredicates<TResult>();
	}

	internal PolicyBuilder(Func<TResult, bool> resultPredicate)
		: this()
	{
		OrResult(resultPredicate);
	}

	internal PolicyBuilder(ExceptionPredicate predicate)
		: this()
	{
		ExceptionPredicates.Add(predicate);
	}

	internal PolicyBuilder(ExceptionPredicates exceptionPredicates)
		: this()
	{
		ExceptionPredicates = exceptionPredicates;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override string ToString()
	{
		return base.ToString();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Type GetType()
	{
		return base.GetType();
	}

	public PolicyBuilder<TResult> OrResult(Func<TResult, bool> resultPredicate)
	{
		ResultPredicate<TResult> predicate = (TResult result) => resultPredicate(result);
		ResultPredicates.Add(predicate);
		return this;
	}

	public PolicyBuilder<TResult> OrResult(TResult result)
	{
		return OrResult((TResult r) => (r != null && r.Equals(result)) || (r == null && result == null));
	}

	public PolicyBuilder<TResult> Or<TException>() where TException : Exception
	{
		ExceptionPredicates.Add((Exception exception) => (!(exception is TException)) ? null : exception);
		return this;
	}

	public PolicyBuilder<TResult> Or<TException>(Func<TException, bool> exceptionPredicate) where TException : Exception
	{
		ExceptionPredicates.Add((Exception exception) => (!(exception is TException arg) || !exceptionPredicate(arg)) ? null : exception);
		return this;
	}

	public PolicyBuilder<TResult> OrInner<TException>() where TException : Exception
	{
		ExceptionPredicates.Add(PolicyBuilder.HandleInner((Exception ex) => ex is TException));
		return this;
	}

	public PolicyBuilder<TResult> OrInner<TException>(Func<TException, bool> exceptionPredicate) where TException : Exception
	{
		ExceptionPredicates.Add(PolicyBuilder.HandleInner((Exception ex) => ex is TException arg && exceptionPredicate(arg)));
		return this;
	}
}
