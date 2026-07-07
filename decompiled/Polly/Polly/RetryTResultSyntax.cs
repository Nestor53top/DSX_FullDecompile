using System;
using System.Collections.Generic;
using System.Linq;
using Polly.Retry;

namespace Polly;

public static class RetryTResultSyntax
{
	public static RetryPolicy<TResult> Retry<TResult>(this PolicyBuilder<TResult> policyBuilder)
	{
		return policyBuilder.Retry(1);
	}

	public static RetryPolicy<TResult> Retry<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount)
	{
		Action<DelegateResult<TResult>, int> onRetry = delegate
		{
		};
		return policyBuilder.Retry(retryCount, onRetry);
	}

	public static RetryPolicy<TResult> Retry<TResult>(this PolicyBuilder<TResult> policyBuilder, Action<DelegateResult<TResult>, int> onRetry)
	{
		return policyBuilder.Retry(1, onRetry);
	}

	public static RetryPolicy<TResult> Retry<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Action<DelegateResult<TResult>, int> onRetry)
	{
		if (retryCount < 0)
		{
			throw new ArgumentOutOfRangeException("retryCount", "Value must be greater than or equal to zero.");
		}
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.Retry(retryCount, delegate(DelegateResult<TResult> outcome, int i, Context ctx)
		{
			onRetry(outcome, i);
		});
	}

	public static RetryPolicy<TResult> Retry<TResult>(this PolicyBuilder<TResult> policyBuilder, Action<DelegateResult<TResult>, int, Context> onRetry)
	{
		return policyBuilder.Retry(1, onRetry);
	}

	public static RetryPolicy<TResult> Retry<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Action<DelegateResult<TResult>, int, Context> onRetry)
	{
		if (retryCount < 0)
		{
			throw new ArgumentOutOfRangeException("retryCount", "Value must be greater than or equal to zero.");
		}
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return new RetryPolicy<TResult>(policyBuilder, delegate(DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx)
		{
			onRetry(outcome, i, ctx);
		}, retryCount);
	}

	public static RetryPolicy<TResult> RetryForever<TResult>(this PolicyBuilder<TResult> policyBuilder)
	{
		Action<DelegateResult<TResult>> onRetry = delegate
		{
		};
		return policyBuilder.RetryForever(onRetry);
	}

	public static RetryPolicy<TResult> RetryForever<TResult>(this PolicyBuilder<TResult> policyBuilder, Action<DelegateResult<TResult>> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.RetryForever(delegate(DelegateResult<TResult> outcome, Context ctx)
		{
			onRetry(outcome);
		});
	}

	public static RetryPolicy<TResult> RetryForever<TResult>(this PolicyBuilder<TResult> policyBuilder, Action<DelegateResult<TResult>, int> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.RetryForever(delegate(DelegateResult<TResult> outcome, int i, Context context)
		{
			onRetry(outcome, i);
		});
	}

	public static RetryPolicy<TResult> RetryForever<TResult>(this PolicyBuilder<TResult> policyBuilder, Action<DelegateResult<TResult>, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return new RetryPolicy<TResult>(policyBuilder, delegate(DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx)
		{
			onRetry(outcome, ctx);
		});
	}

	public static RetryPolicy<TResult> RetryForever<TResult>(this PolicyBuilder<TResult> policyBuilder, Action<DelegateResult<TResult>, int, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return new RetryPolicy<TResult>(policyBuilder, delegate(DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx)
		{
			onRetry(outcome, i, ctx);
		});
	}

	public static RetryPolicy<TResult> WaitAndRetry<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider)
	{
		Action<DelegateResult<TResult>, TimeSpan, int, Context> onRetry = delegate
		{
		};
		return policyBuilder.WaitAndRetry(retryCount, sleepDurationProvider, onRetry);
	}

	public static RetryPolicy<TResult> WaitAndRetry<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetry(retryCount, sleepDurationProvider, delegate(DelegateResult<TResult> outcome, TimeSpan span, int i, Context ctx)
		{
			onRetry(outcome, span);
		});
	}

	public static RetryPolicy<TResult> WaitAndRetry<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetry(retryCount, sleepDurationProvider, delegate(DelegateResult<TResult> outcome, TimeSpan span, int i, Context ctx)
		{
			onRetry(outcome, span, ctx);
		});
	}

	public static RetryPolicy<TResult> WaitAndRetry<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan, int, Context> onRetry)
	{
		if (retryCount < 0)
		{
			throw new ArgumentOutOfRangeException("retryCount", "Value must be greater than or equal to zero.");
		}
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		IEnumerable<TimeSpan> sleepDurationsEnumerable = Enumerable.Range(1, retryCount).Select(sleepDurationProvider);
		return new RetryPolicy<TResult>(policyBuilder, onRetry, retryCount, sleepDurationsEnumerable);
	}

	public static RetryPolicy<TResult> WaitAndRetry<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, Context, TimeSpan> sleepDurationProvider)
	{
		Action<DelegateResult<TResult>, TimeSpan, int, Context> onRetry = delegate
		{
		};
		return policyBuilder.WaitAndRetry(retryCount, sleepDurationProvider, onRetry);
	}

	public static RetryPolicy<TResult> WaitAndRetry<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, Context, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetry(retryCount, sleepDurationProvider, delegate(DelegateResult<TResult> outcome, TimeSpan span, int i, Context ctx)
		{
			onRetry(outcome, span, ctx);
		});
	}

	public static RetryPolicy<TResult> WaitAndRetry<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, Context, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan, int, Context> onRetry)
	{
		return policyBuilder.WaitAndRetry(retryCount, (int i, DelegateResult<TResult> outcome, Context ctx) => sleepDurationProvider(i, ctx), onRetry);
	}

	public static RetryPolicy<TResult> WaitAndRetry<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, DelegateResult<TResult>, Context, TimeSpan> sleepDurationProvider)
	{
		Action<DelegateResult<TResult>, TimeSpan, int, Context> onRetry = delegate
		{
		};
		return policyBuilder.WaitAndRetry(retryCount, sleepDurationProvider, onRetry);
	}

	public static RetryPolicy<TResult> WaitAndRetry<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, DelegateResult<TResult>, Context, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetry(retryCount, sleepDurationProvider, delegate(DelegateResult<TResult> outcome, TimeSpan span, int i, Context ctx)
		{
			onRetry(outcome, span, ctx);
		});
	}

	public static RetryPolicy<TResult> WaitAndRetry<TResult>(this PolicyBuilder<TResult> policyBuilder, int retryCount, Func<int, DelegateResult<TResult>, Context, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan, int, Context> onRetry)
	{
		if (retryCount < 0)
		{
			throw new ArgumentOutOfRangeException("retryCount", "Value must be greater than or equal to zero.");
		}
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return new RetryPolicy<TResult>(policyBuilder, onRetry, retryCount, null, sleepDurationProvider);
	}

	public static RetryPolicy<TResult> WaitAndRetry<TResult>(this PolicyBuilder<TResult> policyBuilder, IEnumerable<TimeSpan> sleepDurations)
	{
		Action<DelegateResult<TResult>, TimeSpan> onRetry = delegate
		{
		};
		return policyBuilder.WaitAndRetry(sleepDurations, onRetry);
	}

	public static RetryPolicy<TResult> WaitAndRetry<TResult>(this PolicyBuilder<TResult> policyBuilder, IEnumerable<TimeSpan> sleepDurations, Action<DelegateResult<TResult>, TimeSpan> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetry(sleepDurations, delegate(DelegateResult<TResult> outcome, TimeSpan span, int i, Context ctx)
		{
			onRetry(outcome, span);
		});
	}

	public static RetryPolicy<TResult> WaitAndRetry<TResult>(this PolicyBuilder<TResult> policyBuilder, IEnumerable<TimeSpan> sleepDurations, Action<DelegateResult<TResult>, TimeSpan, Context> onRetry)
	{
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetry(sleepDurations, delegate(DelegateResult<TResult> outcome, TimeSpan span, int i, Context ctx)
		{
			onRetry(outcome, span, ctx);
		});
	}

	public static RetryPolicy<TResult> WaitAndRetry<TResult>(this PolicyBuilder<TResult> policyBuilder, IEnumerable<TimeSpan> sleepDurations, Action<DelegateResult<TResult>, TimeSpan, int, Context> onRetry)
	{
		if (sleepDurations == null)
		{
			throw new ArgumentNullException("sleepDurations");
		}
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return new RetryPolicy<TResult>(policyBuilder, onRetry, int.MaxValue, sleepDurations);
	}

	public static RetryPolicy<TResult> WaitAndRetryForever<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, TimeSpan> sleepDurationProvider)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		Action<DelegateResult<TResult>, TimeSpan> onRetry = delegate
		{
		};
		return policyBuilder.WaitAndRetryForever(sleepDurationProvider, onRetry);
	}

	public static RetryPolicy<TResult> WaitAndRetryForever<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, Context, TimeSpan> sleepDurationProvider)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		Action<DelegateResult<TResult>, TimeSpan, Context> onRetry = delegate
		{
		};
		return policyBuilder.WaitAndRetryForever(sleepDurationProvider, onRetry);
	}

	public static RetryPolicy<TResult> WaitAndRetryForever<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan> onRetry)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryForever((int retryCount, Context context) => sleepDurationProvider(retryCount), delegate(DelegateResult<TResult> exception, TimeSpan timespan, Context context)
		{
			onRetry(exception, timespan);
		});
	}

	public static RetryPolicy<TResult> WaitAndRetryForever<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, int, TimeSpan> onRetry)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return policyBuilder.WaitAndRetryForever((int retryCount, DelegateResult<TResult> outcome, Context context) => sleepDurationProvider(retryCount), delegate(DelegateResult<TResult> outcome, int i, TimeSpan timespan, Context context)
		{
			onRetry(outcome, i, timespan);
		});
	}

	public static RetryPolicy<TResult> WaitAndRetryForever<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, Context, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan, Context> onRetry)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		return policyBuilder.WaitAndRetryForever((int i, DelegateResult<TResult> outcome, Context ctx) => sleepDurationProvider(i, ctx), onRetry);
	}

	public static RetryPolicy<TResult> WaitAndRetryForever<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, Context, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, int, TimeSpan, Context> onRetry)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		return policyBuilder.WaitAndRetryForever((int i, DelegateResult<TResult> outcome, Context ctx) => sleepDurationProvider(i, ctx), onRetry);
	}

	public static RetryPolicy<TResult> WaitAndRetryForever<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, DelegateResult<TResult>, Context, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, TimeSpan, Context> onRetry)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return new RetryPolicy<TResult>(policyBuilder, delegate(DelegateResult<TResult> outcome, TimeSpan timespan, int i, Context ctx)
		{
			onRetry(outcome, timespan, ctx);
		}, int.MaxValue, null, sleepDurationProvider);
	}

	public static RetryPolicy<TResult> WaitAndRetryForever<TResult>(this PolicyBuilder<TResult> policyBuilder, Func<int, DelegateResult<TResult>, Context, TimeSpan> sleepDurationProvider, Action<DelegateResult<TResult>, int, TimeSpan, Context> onRetry)
	{
		if (sleepDurationProvider == null)
		{
			throw new ArgumentNullException("sleepDurationProvider");
		}
		if (onRetry == null)
		{
			throw new ArgumentNullException("onRetry");
		}
		return new RetryPolicy<TResult>(policyBuilder, delegate(DelegateResult<TResult> exception, TimeSpan timespan, int i, Context ctx)
		{
			onRetry(exception, i, timespan, ctx);
		}, int.MaxValue, null, sleepDurationProvider);
	}
}
