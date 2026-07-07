using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly.Retry;

namespace Polly;

public static class AsyncRetryTResultSyntax
{
	public static AsyncRetryPolicy<TResult> RetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder)
	{
		return policyBuilder.RetryAsync(1);
	}

	public static AsyncRetryPolicy<TResult> RetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount)
	{
		Action<DelegateResult<TResult>, int> onRetry = delegate
		{
		};
		return policyBuilder.RetryAsync(retryCount, onRetry);
	}

	public static AsyncRetryPolicy<TResult> RetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Action<DelegateResult<TResult>, int> onRetry)
	{
		return policyBuilder.RetryAsync(1, async delegate(DelegateResult<TResult> outcome, int i, Context ctx)
		{
			onRetry(outcome, i);
		});
	}

	public static AsyncRetryPolicy<TResult> RetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<DelegateResult<TResult>, int, Task> onRetryAsync)
	{
		return policyBuilder.RetryAsync(1, (DelegateResult<TResult> outcome, int i, Context ctx) => onRetryAsync(outcome, i));
	}

	public static AsyncRetryPolicy<TResult> RetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Action<DelegateResult<TResult>, int> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.RetryAsync(retryCount, async delegate(DelegateResult<TResult> outcome, int i, Context ctx)
		{
			onRetry(outcome, i);
		});
	}

	public static AsyncRetryPolicy<TResult> RetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<DelegateResult<TResult>, int, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.RetryAsync(retryCount, (DelegateResult<TResult> outcome, int i, Context ctx) => onRetryAsync(outcome, i));
	}

	public static AsyncRetryPolicy<TResult> RetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Action<DelegateResult<TResult>, int, Context> onRetry)
	{
		return policyBuilder.RetryAsync(1, onRetry);
	}

	public static AsyncRetryPolicy<TResult> RetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<DelegateResult<TResult>, int, Context, Task> onRetryAsync)
	{
		return policyBuilder.RetryAsync(1, onRetryAsync);
	}

	public static AsyncRetryPolicy<TResult> RetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Action<DelegateResult<TResult>, int, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.RetryAsync(retryCount, async delegate(DelegateResult<TResult> outcome, int i, Context ctx)
		{
			onRetry(outcome, i, ctx);
		});
	}

	public static AsyncRetryPolicy<TResult> RetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<DelegateResult<TResult>, int, Context, Task> onRetryAsync)
	{
		if (retryCount < 0)
		{
			throw new ArgumentOutOfRangeException("retryCount", "Value must be greater than or equal to zero.");
		}
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return new AsyncRetryPolicy<TResult>(policyBuilder, (DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, i, ctx), retryCount);
	}

	public static AsyncRetryPolicy<TResult> RetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder)
	{
		Action<DelegateResult<TResult>> onRetry = delegate
		{
		};
		return policyBuilder.RetryForeverAsync(onRetry);
	}

	public static AsyncRetryPolicy<TResult> RetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Action<DelegateResult<TResult>> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.RetryForeverAsync(async delegate(DelegateResult<TResult> outcome, Context ctx)
		{
			onRetry(outcome);
		});
	}

	public static AsyncRetryPolicy<TResult> RetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Action<DelegateResult<TResult>, int> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.RetryForeverAsync(async delegate(DelegateResult<TResult> outcome, int i, Context context)
		{
			onRetry(outcome, i);
		});
	}

	public static AsyncRetryPolicy<TResult> RetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<DelegateResult<TResult>, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.RetryForeverAsync((DelegateResult<TResult> outcome, Context ctx) => onRetryAsync(outcome));
	}

	public static AsyncRetryPolicy<TResult> RetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<DelegateResult<TResult>, int, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.RetryForeverAsync((DelegateResult<TResult> outcome, int i, Context context) => onRetryAsync(outcome, i));
	}

	public static AsyncRetryPolicy<TResult> RetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Action<DelegateResult<TResult>, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.RetryForeverAsync(async delegate(DelegateResult<TResult> outcome, Context ctx)
		{
			onRetry(outcome, ctx);
		});
	}

	public static AsyncRetryPolicy<TResult> RetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Action<DelegateResult<TResult>, int, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.RetryForeverAsync(async delegate(DelegateResult<TResult> outcome, int i, Context ctx)
		{
			onRetry(outcome, i, ctx);
		});
	}

	public static AsyncRetryPolicy<TResult> RetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<DelegateResult<TResult>, Context, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return new AsyncRetryPolicy<TResult>(policyBuilder, (DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, ctx));
	}

	public static AsyncRetryPolicy<TResult> RetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<DelegateResult<TResult>, int, Context, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return new AsyncRetryPolicy<TResult>(policyBuilder, (DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, i, ctx));
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider)
	{
		Action<DelegateResult<TResult>, TimeSpan> onRetry = delegate
		{
		};
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, onRetry);
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, async delegate(DelegateResult<TResult> outcome, TimeSpan span, int i, Context ctx)
		{
			onRetry(outcome, span);
		});
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Func<DelegateResult<TResult>, TimeSpan, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, (DelegateResult<TResult> outcome, TimeSpan span, int i, Context ctx) => onRetryAsync(outcome, span));
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, async delegate(DelegateResult<TResult> outcome, TimeSpan span, int i, Context ctx)
		{
			onRetry(outcome, span, ctx);
		});
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Func<DelegateResult<TResult>, TimeSpan, Context, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, (DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, timespan, ctx));
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan, int, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, async delegate(DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx)
		{
			onRetry(outcome, timespan, i, ctx);
		});
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Func<DelegateResult<TResult>, TimeSpan, int, Context, Task> onRetryAsync)
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
		return new AsyncRetryPolicy<TResult>(policyBuilder, onRetryAsync, retryCount, sleepDurationsEnumerable);
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, Context, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, async delegate(DelegateResult<TResult> outcome, TimeSpan span, int i, Context ctx)
		{
			onRetry(outcome, span, ctx);
		});
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, Context, TimeSpan> sleepDurationProvider, Func<DelegateResult<TResult>, TimeSpan, Context, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, (DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, timespan, ctx));
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, Context, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan, int, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, sleepDurationProvider, async delegate(DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx)
		{
			onRetry(outcome, timespan, i, ctx);
		});
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, Context, TimeSpan> sleepDurationProvider, Func<DelegateResult<TResult>, TimeSpan, int, Context, Task> onRetryAsync)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		return policyBuilder.WaitAndRetryAsync(retryCount, (int i, DelegateResult<TResult> outcome, Context ctx) => sleepDurationProvider(i, ctx), onRetryAsync);
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, DelegateResult<TResult>, Context, TimeSpan> sleepDurationProvider, Func<DelegateResult<TResult>, TimeSpan, int, Context, Task> onRetryAsync)
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
		return new AsyncRetryPolicy<TResult>(policyBuilder, onRetryAsync, retryCount, null, sleepDurationProvider);
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, IEnumerable<TimeSpan> sleepDurations)
	{
		Action<DelegateResult<TResult>, TimeSpan> onRetry = delegate
		{
		};
		return policyBuilder.WaitAndRetryAsync(sleepDurations, onRetry);
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, IEnumerable<TimeSpan> sleepDurations, Action<DelegateResult<TResult>, TimeSpan> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(sleepDurations, async delegate(DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx)
		{
			onRetry(outcome, timespan);
		});
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, IEnumerable<TimeSpan> sleepDurations, Func<DelegateResult<TResult>, TimeSpan, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.WaitAndRetryAsync(sleepDurations, (DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, timespan));
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, IEnumerable<TimeSpan> sleepDurations, Action<DelegateResult<TResult>, TimeSpan, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(sleepDurations, async delegate(DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx)
		{
			onRetry(outcome, timespan, ctx);
		});
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, IEnumerable<TimeSpan> sleepDurations, Func<DelegateResult<TResult>, TimeSpan, Context, Task> onRetryAsync)
	{
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.WaitAndRetryAsync(sleepDurations, (DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, timespan, ctx));
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, IEnumerable<TimeSpan> sleepDurations, Action<DelegateResult<TResult>, TimeSpan, int, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryAsync(sleepDurations, async delegate(DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx)
		{
			onRetry(outcome, timespan, i, ctx);
		});
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, IEnumerable<TimeSpan> sleepDurations, Func<DelegateResult<TResult>, TimeSpan, int, Context, Task> onRetryAsync)
	{
		if (sleepDurations == null)
		{
			throw new ArgumentNullException("sleepDurations");
		}
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return new AsyncRetryPolicy<TResult>(policyBuilder, onRetryAsync, int.MaxValue, sleepDurations);
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, TimeSpan> sleepDurationProvider)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		Action<DelegateResult<TResult>, TimeSpan> onRetry = delegate
		{
		};
		return policyBuilder.WaitAndRetryForeverAsync(sleepDurationProvider, onRetry);
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, Context, TimeSpan> sleepDurationProvider)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		Action<DelegateResult<TResult>, TimeSpan, Context> onRetry = delegate
		{
		};
		return policyBuilder.WaitAndRetryForeverAsync(sleepDurationProvider, onRetry);
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan> onRetry)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryForeverAsync((int retryCount, Context context) => sleepDurationProvider(retryCount), delegate(DelegateResult<TResult> outcome, TimeSpan timespan, Context context)
		{
			onRetry(outcome, timespan);
		});
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, int, TimeSpan> onRetry)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryForeverAsync((int retryCount, Context context) => sleepDurationProvider(retryCount), delegate(DelegateResult<TResult> outcome, int i, TimeSpan timespan, Context context)
		{
			onRetry(outcome, i, timespan);
		});
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, TimeSpan> sleepDurationProvider, Func<DelegateResult<TResult>, TimeSpan, Task> onRetryAsync)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.WaitAndRetryForeverAsync((int retryCount, Context context) => sleepDurationProvider(retryCount), (DelegateResult<TResult> outcome, TimeSpan timespan, Context context) => onRetryAsync(outcome, timespan));
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, TimeSpan> sleepDurationProvider, Func<DelegateResult<TResult>, int, TimeSpan, Task> onRetryAsync)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return policyBuilder.WaitAndRetryForeverAsync((int retryCount, Context context) => sleepDurationProvider(retryCount), (DelegateResult<TResult> outcome, int i, TimeSpan timespan, Context context) => onRetryAsync(outcome, i, timespan));
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, Context, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryForeverAsync(sleepDurationProvider, async delegate(DelegateResult<TResult> outcome, TimeSpan timespan, Context ctx)
		{
			onRetry(outcome, timespan, ctx);
		});
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, Context, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, int, TimeSpan, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryForeverAsync(sleepDurationProvider, async delegate(DelegateResult<TResult> outcome, int i, TimeSpan timespan, Context ctx)
		{
			onRetry(outcome, i, timespan, ctx);
		});
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, Context, TimeSpan> sleepDurationProvider, Func<DelegateResult<TResult>, TimeSpan, Context, Task> onRetryAsync)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		return policyBuilder.WaitAndRetryForeverAsync((int i, DelegateResult<TResult> outcome, Context ctx) => sleepDurationProvider(i, ctx), onRetryAsync);
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, Context, TimeSpan> sleepDurationProvider, Func<DelegateResult<TResult>, int, TimeSpan, Context, Task> onRetryAsync)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		return policyBuilder.WaitAndRetryForeverAsync((int i, DelegateResult<TResult> outcome, Context ctx) => sleepDurationProvider(i, ctx), onRetryAsync);
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, DelegateResult<TResult>, Context, TimeSpan> sleepDurationProvider, Func<DelegateResult<TResult>, TimeSpan, Context, Task> onRetryAsync)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return new AsyncRetryPolicy<TResult>(policyBuilder, (DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx) => onRetryAsync(outcome, timespan, ctx), int.MaxValue, null, sleepDurationProvider);
	}

	public static AsyncRetryPolicy<TResult> WaitAndRetryForeverAsync<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, DelegateResult<TResult>, Context, TimeSpan> sleepDurationProvider, Func<DelegateResult<TResult>, int, TimeSpan, Context, Task> onRetryAsync)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetryAsync == null)
		{
			throw new ArgumentNullException("onRetryAsync");
		}
		return new AsyncRetryPolicy<TResult>(policyBuilder, (DelegateResult<TResult> exception, TimeSpan timespan, int i, Context ctx) => onRetryAsync(exception, i, timespan, ctx), int.MaxValue, null, sleepDurationProvider);
	}
}
