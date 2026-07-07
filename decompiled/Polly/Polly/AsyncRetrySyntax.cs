using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly.Retry;

namespace Polly;

public static class AsyncRetrySyntax
{
	public static AsyncRetryPolicy RetryAsync(this PolicyBuilder policyBuilder)
	{
		return policyBuilder.RetryAsync(1);
	}

	public static AsyncRetryPolicy RetryAsync(this PolicyBuilder policyBuilder, int retryCount)
	{
		Action<Exception, int, Context> onRetry = delegate
		{
		};
		return policyBuilder.RetryAsync(retryCount, onRetry);
	}

	public static AsyncRetryPolicy RetryAsync(this PolicyBuilder policyBuilder, Action<Exception, int> onRetry)
	{
		return policyBuilder.RetryAsync(1, async delegate(Exception outcome, int i, Context ctx)
		{
			onRetry(outcome, i);
		});
	}

	public static AsyncRetryPolicy RetryAsync(this PolicyBuilder policyBuilder, Func<Exception, int, Task> onRetryAsync)
	{
		return policyBuilder.RetryAsync(1, (Exception outcome, int i, Context ctx) => onRetryAsync(outcome, i));
	}

	public static AsyncRetryPolicy RetryAsync(this PolicyBuilder policyBuilder, int retryCount, Action<Exception, int> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.RetryAsync(retryCount, async delegate(Exception outcome, int i, Context ctx)
		{
			onRetry(outcome, i);
		});
	}

	public static AsyncRetryPolicy RetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<Exception, int, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.RetryAsync(retryCount, (Exception outcome, int i, Context ctx) => onRetryAsync(outcome, i));
	}

	public static AsyncRetryPolicy RetryAsync(this PolicyBuilder policyBuilder, Action<Exception, int, Context> onRetry)
	{
		return policyBuilder.RetryAsync(1, onRetry);
	}

	public static AsyncRetryPolicy RetryAsync(this PolicyBuilder policyBuilder, Func<Exception, int, Context, Task> onRetryAsync)
	{
		return policyBuilder.RetryAsync(1, onRetryAsync);
	}

	public static AsyncRetryPolicy RetryAsync(this PolicyBuilder policyBuilder, int retryCount, Action<Exception, int, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.RetryAsync(retryCount, async delegate(Exception outcome, int i, Context ctx)
		{
			onRetry(outcome, i, ctx);
		});
	}

	public static AsyncRetryPolicy RetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<Exception, int, Context, Task> onRetryAsync)
	{
		if (retryCount < 0)
		{
			throw new ArgumentOutOfRangeException("retryCount", "Value must be greater than or equal to zero.");
		}
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return new AsyncRetryPolicy(policyBuilder, (Exception outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, i, ctx), retryCount);
	}

	public static AsyncRetryPolicy RetryForeverAsync(this PolicyBuilder policyBuilder)
	{
		Action<Exception> onRetry = delegate
		{
		};
		return policyBuilder.RetryForeverAsync(onRetry);
	}

	public static AsyncRetryPolicy RetryForeverAsync(this PolicyBuilder policyBuilder, Action<Exception> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.RetryForeverAsync(async delegate(Exception outcome, Context ctx)
		{
			onRetry(outcome);
		});
	}

	public static AsyncRetryPolicy RetryForeverAsync(this PolicyBuilder policyBuilder, Action<Exception, int> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.RetryForeverAsync(async delegate(Exception outcome, int i, Context context)
		{
			onRetry(outcome, i);
		});
	}

	public static AsyncRetryPolicy RetryForeverAsync(this PolicyBuilder policyBuilder, Func<Exception, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.RetryForeverAsync((Exception outcome, Context ctx) => onRetryAsync(outcome));
	}

	public static AsyncRetryPolicy RetryForeverAsync(this PolicyBuilder policyBuilder, Func<Exception, int, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.RetryForeverAsync((Exception outcome, int i, Context context) => onRetryAsync(outcome, i));
	}

	public static AsyncRetryPolicy RetryForeverAsync(this PolicyBuilder policyBuilder, Action<Exception, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.RetryForeverAsync(async delegate(Exception outcome, Context ctx)
		{
			onRetry(outcome, ctx);
		});
	}

	public static AsyncRetryPolicy RetryForeverAsync(this PolicyBuilder policyBuilder, Action<Exception, int, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.RetryForeverAsync(async delegate(Exception outcome, int i, Context ctx)
		{
			onRetry(outcome, i, ctx);
		});
	}

	public static AsyncRetryPolicy RetryForeverAsync(this PolicyBuilder policyBuilder, Func<Exception, Context, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return new AsyncRetryPolicy(policyBuilder, (Exception outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, ctx));
	}

	public static AsyncRetryPolicy RetryForeverAsync(this PolicyBuilder policyBuilder, Func<Exception, int, Context, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return new AsyncRetryPolicy(policyBuilder, (Exception outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, i, ctx));
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider)
	{
		Action<Exception, TimeSpan> onRetry = delegate
		{
		};
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, onRetry);
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, async delegate(Exception outcome, TimeSpan span, int i, Context ctx)
		{
			onRetry(outcome, span);
		});
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, (Exception outcome, TimeSpan span, int i, Context ctx) => onRetryAsync(outcome, span));
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, async delegate(Exception outcome, TimeSpan span, int i, Context ctx)
		{
			onRetry(outcome, span, ctx);
		});
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, Context, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, (Exception outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, timespan, ctx));
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan, int, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, async delegate(Exception outcome, TimeSpan timespan, int i, Context ctx)
		{
			onRetry(outcome, timespan, i, ctx);
		});
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, int, Context, Task> onRetryAsync)
	{
		if (retryCount < 0)
		{
			throw new ArgumentOutOfRangeException("retryCount", "Value must be greater than or equal to zero.");
		}
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		IEnumerable<TimeSpan> sleepDurationsEnumerable = Enumerable.Range(1, retryCount).Select(sleepDurationProvider);
		return new AsyncRetryPolicy(policyBuilder, onRetryAsync, retryCount, sleepDurationsEnumerable);
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, Context, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, async delegate(Exception outcome, TimeSpan span, int i, Context ctx)
		{
			onRetry(outcome, span, ctx);
		});
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, Context, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, Context, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, (Exception outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, timespan, ctx));
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, Context, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan, int, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, async delegate(Exception outcome, TimeSpan timespan, int i, Context ctx)
		{
			onRetry(outcome, timespan, i, ctx);
		});
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, Context, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, int, Context, Task> onRetryAsync)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, (int i, Exception outcome, Context ctx) => sleepDurationProvider(i, ctx), onRetryAsync);
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, int retryCount, Func<int, Exception, Context, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, int, Context, Task> onRetryAsync)
	{
		if (retryCount < 0)
		{
			throw new ArgumentOutOfRangeException("retryCount", "Value must be greater than or equal to zero.");
		}
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return new AsyncRetryPolicy(policyBuilder, onRetryAsync, retryCount, null, sleepDurationProvider);
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, IEnumerable<TimeSpan> sleepDurations)
	{
		Action<Exception, TimeSpan, int, Context> onRetry = delegate
		{
		};
		return policyBuilder.WaitAndRetryAsync(sleepDurations, onRetry);
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, IEnumerable<TimeSpan> sleepDurations, Action<Exception, TimeSpan> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(sleepDurations, async delegate(Exception outcome, TimeSpan timespan, int i, Context ctx)
		{
			onRetry(outcome, timespan);
		});
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, IEnumerable<TimeSpan> sleepDurations, Func<Exception, TimeSpan, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.WaitAndRetryAsync(sleepDurations, (Exception outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, timespan));
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, IEnumerable<TimeSpan> sleepDurations, Action<Exception, TimeSpan, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(sleepDurations, async delegate(Exception outcome, TimeSpan timespan, int i, Context ctx)
		{
			onRetry(outcome, timespan, ctx);
		});
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, IEnumerable<TimeSpan> sleepDurations, Func<Exception, TimeSpan, Context, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.WaitAndRetryAsync(sleepDurations, (Exception outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, timespan, ctx));
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, IEnumerable<TimeSpan> sleepDurations, Action<Exception, TimeSpan, int, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(sleepDurations, async delegate(Exception outcome, TimeSpan timespan, int i, Context ctx)
		{
			onRetry(outcome, timespan, i, ctx);
		});
	}

	public static AsyncRetryPolicy WaitAndRetryAsync(this PolicyBuilder policyBuilder, IEnumerable<TimeSpan> sleepDurations, Func<Exception, TimeSpan, int, Context, Task> onRetryAsync)
	{
		if (sleepDurations == null)
		{
			throw new ArgumentNullException("sleepDurations");
		}
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return new AsyncRetryPolicy(policyBuilder, onRetryAsync, int.MaxValue, sleepDurations);
	}

	public static AsyncRetryPolicy WaitAndRetryForeverAsync(this PolicyBuilder policyBuilder, Func<int, TimeSpan> sleepDurationProvider)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		Action<Exception, TimeSpan> onRetry = delegate
		{
		};
		return policyBuilder.WaitAndRetryForeverAsync(sleepDurationProvider, onRetry);
	}

	public static AsyncRetryPolicy WaitAndRetryForeverAsync(this PolicyBuilder policyBuilder, Func<int, Context, TimeSpan> sleepDurationProvider)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		Action<Exception, TimeSpan, Context> onRetry = delegate
		{
		};
		return policyBuilder.WaitAndRetryForeverAsync(sleepDurationProvider, onRetry);
	}

	public static AsyncRetryPolicy WaitAndRetryForeverAsync(this PolicyBuilder policyBuilder, Func<int, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan> onRetry)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryForeverAsync((int retryCount, Context context) => sleepDurationProvider(retryCount), delegate(Exception exception, TimeSpan timespan, Context context)
		{
			onRetry(exception, timespan);
		});
	}

	public static AsyncRetryPolicy WaitAndRetryForeverAsync(this PolicyBuilder policyBuilder, Func<int, TimeSpan> sleepDurationProvider, Action<Exception, int, TimeSpan> onRetry)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryForeverAsync((int retryCount, Context context) => sleepDurationProvider(retryCount), delegate(Exception exception, int i, TimeSpan timespan, Context context)
		{
			onRetry(exception, i, timespan);
		});
	}

	public static AsyncRetryPolicy WaitAndRetryForeverAsync(this PolicyBuilder policyBuilder, Func<int, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, Task> onRetryAsync)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.WaitAndRetryForeverAsync((int retryCount, Context context) => sleepDurationProvider(retryCount), (Exception exception, TimeSpan timespan, Context context) => onRetryAsync(exception, timespan));
	}

	public static AsyncRetryPolicy WaitAndRetryForeverAsync(this PolicyBuilder policyBuilder, Func<int, TimeSpan> sleepDurationProvider, Func<Exception, int, TimeSpan, Task> onRetryAsync)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.WaitAndRetryForeverAsync((int retryCount, Context context) => sleepDurationProvider(retryCount), (Exception exception, int i, TimeSpan timespan, Context context) => onRetryAsync(exception, i, timespan));
	}

	public static AsyncRetryPolicy WaitAndRetryForeverAsync(this PolicyBuilder policyBuilder, Func<int, Context, TimeSpan> sleepDurationProvider, Action<Exception, TimeSpan, Context> onRetry)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryForeverAsync(sleepDurationProvider, async delegate(Exception exception, TimeSpan timespan, Context ctx)
		{
			onRetry(exception, timespan, ctx);
		});
	}

	public static AsyncRetryPolicy WaitAndRetryForeverAsync(this PolicyBuilder policyBuilder, Func<int, Context, TimeSpan> sleepDurationProvider, Action<Exception, int, TimeSpan, Context> onRetry)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryForeverAsync(sleepDurationProvider, async delegate(Exception exception, int i, TimeSpan timespan, Context ctx)
		{
			onRetry(exception, i, timespan, ctx);
		});
	}

	public static AsyncRetryPolicy WaitAndRetryForeverAsync(this PolicyBuilder policyBuilder, Func<int, Context, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, Context, Task> onRetryAsync)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		return policyBuilder.WaitAndRetryForeverAsync((int i, Exception outcome, Context ctx) => sleepDurationProvider(i, ctx), onRetryAsync);
	}

	public static AsyncRetryPolicy WaitAndRetryForeverAsync(this PolicyBuilder policyBuilder, Func<int, Context, TimeSpan> sleepDurationProvider, Func<Exception, int, TimeSpan, Context, Task> onRetryAsync)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		return policyBuilder.WaitAndRetryForeverAsync((int i, Exception outcome, Context ctx) => sleepDurationProvider(i, ctx), onRetryAsync);
	}

	public static AsyncRetryPolicy WaitAndRetryForeverAsync(this PolicyBuilder policyBuilder, Func<int, Exception, Context, TimeSpan> sleepDurationProvider, Func<Exception, TimeSpan, Context, Task> onRetryAsync)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return new AsyncRetryPolicy(policyBuilder, (Exception outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, timespan, ctx), int.MaxValue, null, sleepDurationProvider);
	}

	public static AsyncRetryPolicy WaitAndRetryForeverAsync(this PolicyBuilder policyBuilder, Func<int, Exception, Context, TimeSpan> sleepDurationProvider, Func<Exception, int, TimeSpan, Context, Task> onRetryAsync)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return new AsyncRetryPolicy(policyBuilder, (Exception exception, TimeSpan timespan, int i, Context ctx) => onRetryAsync(exception, i, timespan, ctx), int.MaxValue, null, sleepDurationProvider);
	}
}
