using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Polly.Bulkhead;
using Polly.Caching;
using Polly.NoOp;
using Polly.Timeout;
using Polly.Utilities;
using Polly.Wrap;

namespace Polly;

public abstract class Policy : PolicyBase, ISyncPolicy, IsPolicy
{
	public static AsyncBulkheadPolicy BulkheadAsync(int maxParallelization)
	{
		Func<Context, Task> onBulkheadRejectedAsync = (Context _) => TaskHelper.EmptyTask;
		return BulkheadAsync(maxParallelization, 0, onBulkheadRejectedAsync);
	}

	public static AsyncBulkheadPolicy BulkheadAsync(int maxParallelization, Func<Context, Task> onBulkheadRejectedAsync)
	{
		return BulkheadAsync(maxParallelization, 0, onBulkheadRejectedAsync);
	}

	public static AsyncBulkheadPolicy BulkheadAsync(int maxParallelization, int maxQueuingActions)
	{
		Func<Context, Task> onBulkheadRejectedAsync = (Context _) => TaskHelper.EmptyTask;
		return BulkheadAsync(maxParallelization, maxQueuingActions, onBulkheadRejectedAsync);
	}

	public static AsyncBulkheadPolicy BulkheadAsync(int maxParallelization, int maxQueuingActions, Func<Context, Task> onBulkheadRejectedAsync)
	{
		if (maxParallelization <= 0)
		{
			throw new ArgumentOutOfRangeException("maxParallelization", "Value must be greater than zero.");
		}
		if (maxQueuingActions < 0)
		{
			throw new ArgumentOutOfRangeException("maxQueuingActions", "Value must be greater than or equal to zero.");
		}
		if (onBulkheadRejectedAsync == null)
		{
			throw new ArgumentNullException("onBulkheadRejectedAsync");
		}
		return new AsyncBulkheadPolicy(maxParallelization, maxQueuingActions, onBulkheadRejectedAsync);
	}

	public static AsyncBulkheadPolicy<TResult> BulkheadAsync<TResult>(int maxParallelization)
	{
		Func<Context, Task> onBulkheadRejectedAsync = (Context _) => TaskHelper.EmptyTask;
		return BulkheadAsync<TResult>(maxParallelization, 0, onBulkheadRejectedAsync);
	}

	public static AsyncBulkheadPolicy<TResult> BulkheadAsync<TResult>(int maxParallelization, Func<Context, Task> onBulkheadRejectedAsync)
	{
		return BulkheadAsync<TResult>(maxParallelization, 0, onBulkheadRejectedAsync);
	}

	public static AsyncBulkheadPolicy<TResult> BulkheadAsync<TResult>(int maxParallelization, int maxQueuingActions)
	{
		Func<Context, Task> onBulkheadRejectedAsync = (Context _) => TaskHelper.EmptyTask;
		return BulkheadAsync<TResult>(maxParallelization, maxQueuingActions, onBulkheadRejectedAsync);
	}

	public static AsyncBulkheadPolicy<TResult> BulkheadAsync<TResult>(int maxParallelization, int maxQueuingActions, Func<Context, Task> onBulkheadRejectedAsync)
	{
		if (maxParallelization <= 0)
		{
			throw new ArgumentOutOfRangeException("maxParallelization", "Value must be greater than zero.");
		}
		if (maxQueuingActions < 0)
		{
			throw new ArgumentOutOfRangeException("maxQueuingActions", "Value must be greater than or equal to zero.");
		}
		if (onBulkheadRejectedAsync == null)
		{
			throw new ArgumentNullException("onBulkheadRejectedAsync");
		}
		return new AsyncBulkheadPolicy<TResult>(maxParallelization, maxQueuingActions, onBulkheadRejectedAsync);
	}

	public static BulkheadPolicy Bulkhead(int maxParallelization)
	{
		Action<Context> onBulkheadRejected = delegate
		{
		};
		return Bulkhead(maxParallelization, 0, onBulkheadRejected);
	}

	public static BulkheadPolicy Bulkhead(int maxParallelization, Action<Context> onBulkheadRejected)
	{
		return Bulkhead(maxParallelization, 0, onBulkheadRejected);
	}

	public static BulkheadPolicy Bulkhead(int maxParallelization, int maxQueuingActions)
	{
		Action<Context> onBulkheadRejected = delegate
		{
		};
		return Bulkhead(maxParallelization, maxQueuingActions, onBulkheadRejected);
	}

	public static BulkheadPolicy Bulkhead(int maxParallelization, int maxQueuingActions, Action<Context> onBulkheadRejected)
	{
		if (maxParallelization <= 0)
		{
			throw new ArgumentOutOfRangeException("maxParallelization", "Value must be greater than zero.");
		}
		if (maxQueuingActions < 0)
		{
			throw new ArgumentOutOfRangeException("maxQueuingActions", "Value must be greater than or equal to zero.");
		}
		if (onBulkheadRejected == null)
		{
			throw new ArgumentNullException("onBulkheadRejected");
		}
		return new BulkheadPolicy(maxParallelization, maxQueuingActions, onBulkheadRejected);
	}

	public static BulkheadPolicy<TResult> Bulkhead<TResult>(int maxParallelization)
	{
		Action<Context> onBulkheadRejected = delegate
		{
		};
		return Bulkhead<TResult>(maxParallelization, 0, onBulkheadRejected);
	}

	public static BulkheadPolicy<TResult> Bulkhead<TResult>(int maxParallelization, Action<Context> onBulkheadRejected)
	{
		return Bulkhead<TResult>(maxParallelization, 0, onBulkheadRejected);
	}

	public static BulkheadPolicy<TResult> Bulkhead<TResult>(int maxParallelization, int maxQueuingActions)
	{
		Action<Context> onBulkheadRejected = delegate
		{
		};
		return Bulkhead<TResult>(maxParallelization, maxQueuingActions, onBulkheadRejected);
	}

	public static BulkheadPolicy<TResult> Bulkhead<TResult>(int maxParallelization, int maxQueuingActions, Action<Context> onBulkheadRejected)
	{
		if (maxParallelization <= 0)
		{
			throw new ArgumentOutOfRangeException("maxParallelization", "Value must be greater than zero.");
		}
		if (maxQueuingActions < 0)
		{
			throw new ArgumentOutOfRangeException("maxQueuingActions", "Value must be greater than or equal to zero.");
		}
		if (onBulkheadRejected == null)
		{
			throw new ArgumentNullException("onBulkheadRejected");
		}
		return new BulkheadPolicy<TResult>(maxParallelization, maxQueuingActions, onBulkheadRejected);
	}

	public static AsyncCachePolicy CacheAsync(IAsyncCacheProvider cacheProvider, TimeSpan ttl, Action<Context, string, Exception> onCacheError = null)
	{
		return CacheAsync(cacheProvider, new RelativeTtl(ttl), DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheError);
	}

	public static AsyncCachePolicy CacheAsync(IAsyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		return CacheAsync(cacheProvider, ttlStrategy, DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheError);
	}

	public static AsyncCachePolicy CacheAsync(IAsyncCacheProvider cacheProvider, TimeSpan ttl, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		return CacheAsync(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy.GetCacheKey, onCacheError);
	}

	public static AsyncCachePolicy CacheAsync(IAsyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		if (ttlStrategy == null)
		{
			throw new ArgumentNullException("ttlStrategy");
		}
		if (cacheKeyStrategy == null)
		{
			throw new ArgumentNullException("cacheKeyStrategy");
		}
		onCacheError = onCacheError ?? ((Action<Context, string, Exception>)delegate
		{
		});
		Action<Context, string> action = delegate
		{
		};
		return new AsyncCachePolicy(cacheProvider, ttlStrategy, cacheKeyStrategy.GetCacheKey, action, action, action, onCacheError, onCacheError);
	}

	public static AsyncCachePolicy CacheAsync(IAsyncCacheProvider cacheProvider, TimeSpan ttl, Func<Context, string> cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		return CacheAsync(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy, onCacheError);
	}

	public static AsyncCachePolicy CacheAsync(IAsyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		if (ttlStrategy == null)
		{
			throw new ArgumentNullException("ttlStrategy");
		}
		if (cacheKeyStrategy == null)
		{
			throw new ArgumentNullException("cacheKeyStrategy");
		}
		onCacheError = onCacheError ?? ((Action<Context, string, Exception>)delegate
		{
		});
		Action<Context, string> action = delegate
		{
		};
		return new AsyncCachePolicy(cacheProvider, ttlStrategy, cacheKeyStrategy, action, action, action, onCacheError, onCacheError);
	}

	public static AsyncCachePolicy CacheAsync(IAsyncCacheProvider cacheProvider, TimeSpan ttl, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return CacheAsync(cacheProvider, new RelativeTtl(ttl), DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy CacheAsync(IAsyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return CacheAsync(cacheProvider, ttlStrategy, DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy CacheAsync(IAsyncCacheProvider cacheProvider, TimeSpan ttl, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return CacheAsync(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy CacheAsync(IAsyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategy.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy CacheAsync(IAsyncCacheProvider cacheProvider, TimeSpan ttl, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return CacheAsync(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy CacheAsync(IAsyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		if (ttlStrategy == null)
		{
			throw new ArgumentNullException("ttlStrategy");
		}
		if (cacheKeyStrategy == null)
		{
			throw new ArgumentNullException("cacheKeyStrategy");
		}
		if (onCacheGet == null)
		{
			throw new ArgumentNullException("onCacheGet");
		}
		if (onCacheMiss == null)
		{
			throw new ArgumentNullException("onCacheMiss");
		}
		if (onCachePut == null)
		{
			throw new ArgumentNullException("onCachePut");
		}
		if (onCachePutError == null)
		{
			throw new ArgumentNullException("onCachePutError");
		}
		if (onCachePutError == null)
		{
			throw new ArgumentNullException("onCachePutError");
		}
		return new AsyncCachePolicy(cacheProvider, ttlStrategy, cacheKeyStrategy, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider cacheProvider, TimeSpan ttl, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return CacheAsync(cacheProvider.AsyncFor<TResult>(), new RelativeTtl(ttl), DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return CacheAsync(cacheProvider.AsyncFor<TResult>(), ttlStrategy, DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider cacheProvider, TimeSpan ttl, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return CacheAsync(cacheProvider.AsyncFor<TResult>(), new RelativeTtl(ttl), cacheKeyStrategy.GetCacheKey, onCacheError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return CacheAsync(cacheProvider.AsyncFor<TResult>(), ttlStrategy, cacheKeyStrategy.GetCacheKey, onCacheError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider cacheProvider, TimeSpan ttl, Func<Context, string> cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return CacheAsync(cacheProvider.AsyncFor<TResult>(), new RelativeTtl(ttl), cacheKeyStrategy, onCacheError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return CacheAsync(cacheProvider.AsyncFor<TResult>(), ttlStrategy, cacheKeyStrategy, onCacheError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider cacheProvider, TimeSpan ttl, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return CacheAsync(cacheProvider.AsyncFor<TResult>(), new RelativeTtl(ttl), DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return CacheAsync(cacheProvider.AsyncFor<TResult>(), ttlStrategy, DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider cacheProvider, TimeSpan ttl, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return CacheAsync(cacheProvider.AsyncFor<TResult>(), new RelativeTtl(ttl), cacheKeyStrategy.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return CacheAsync(cacheProvider.AsyncFor<TResult>(), ttlStrategy, cacheKeyStrategy.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider cacheProvider, TimeSpan ttl, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return CacheAsync(cacheProvider.AsyncFor<TResult>(), new RelativeTtl(ttl), cacheKeyStrategy, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return CacheAsync(cacheProvider.AsyncFor<TResult>(), ttlStrategy, cacheKeyStrategy, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, TimeSpan ttl, Action<Context, string, Exception> onCacheError = null)
	{
		return CacheAsync(cacheProvider, new RelativeTtl(ttl), DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, ITtlStrategy ttlStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		return CacheAsync(cacheProvider, ttlStrategy, DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, ITtlStrategy<TResult> ttlStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		return CacheAsync(cacheProvider, ttlStrategy, DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, TimeSpan ttl, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		return CacheAsync(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy.GetCacheKey, onCacheError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, ITtlStrategy ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		onCacheError = onCacheError ?? ((Action<Context, string, Exception>)delegate
		{
		});
		Action<Context, string> action = delegate
		{
		};
		return CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategy.GetCacheKey, action, action, action, onCacheError, onCacheError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, ITtlStrategy<TResult> ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		onCacheError = onCacheError ?? ((Action<Context, string, Exception>)delegate
		{
		});
		Action<Context, string> action = delegate
		{
		};
		return CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategy.GetCacheKey, action, action, action, onCacheError, onCacheError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, TimeSpan ttl, Func<Context, string> cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		return CacheAsync(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy, onCacheError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, ITtlStrategy ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		onCacheError = onCacheError ?? ((Action<Context, string, Exception>)delegate
		{
		});
		Action<Context, string> action = delegate
		{
		};
		return CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategy, action, action, action, onCacheError, onCacheError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, ITtlStrategy<TResult> ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		onCacheError = onCacheError ?? ((Action<Context, string, Exception>)delegate
		{
		});
		Action<Context, string> action = delegate
		{
		};
		return CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategy, action, action, action, onCacheError, onCacheError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, TimeSpan ttl, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return CacheAsync(cacheProvider, new RelativeTtl(ttl), DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, ITtlStrategy ttlStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return CacheAsync(cacheProvider, ttlStrategy.For<TResult>(), DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, ITtlStrategy<TResult> ttlStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return CacheAsync(cacheProvider, ttlStrategy, DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, TimeSpan ttl, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return CacheAsync(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, ITtlStrategy ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return CacheAsync(cacheProvider, ttlStrategy.For<TResult>(), cacheKeyStrategy.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, ITtlStrategy<TResult> ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategy.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, TimeSpan ttl, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return CacheAsync(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, ITtlStrategy ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return CacheAsync(cacheProvider, ttlStrategy.For<TResult>(), cacheKeyStrategy, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncCachePolicy<TResult> CacheAsync<TResult>(IAsyncCacheProvider<TResult> cacheProvider, ITtlStrategy<TResult> ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		if (ttlStrategy == null)
		{
			throw new ArgumentNullException("ttlStrategy");
		}
		if (cacheKeyStrategy == null)
		{
			throw new ArgumentNullException("cacheKeyStrategy");
		}
		if (onCacheGet == null)
		{
			throw new ArgumentNullException("onCacheGet");
		}
		if (onCacheMiss == null)
		{
			throw new ArgumentNullException("onCacheMiss");
		}
		if (onCachePut == null)
		{
			throw new ArgumentNullException("onCachePut");
		}
		if (onCachePutError == null)
		{
			throw new ArgumentNullException("onCachePutError");
		}
		if (onCachePutError == null)
		{
			throw new ArgumentNullException("onCachePutError");
		}
		return new AsyncCachePolicy<TResult>(cacheProvider, ttlStrategy, cacheKeyStrategy, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy Cache(ISyncCacheProvider cacheProvider, TimeSpan ttl, Action<Context, string, Exception> onCacheError = null)
	{
		return Cache(cacheProvider, new RelativeTtl(ttl), DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheError);
	}

	public static CachePolicy Cache(ISyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		return Cache(cacheProvider, ttlStrategy, DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheError);
	}

	public static CachePolicy Cache(ISyncCacheProvider cacheProvider, TimeSpan ttl, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		return Cache(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy.GetCacheKey, onCacheError);
	}

	public static CachePolicy Cache(ISyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		if (ttlStrategy == null)
		{
			throw new ArgumentNullException("ttlStrategy");
		}
		if (cacheKeyStrategy == null)
		{
			throw new ArgumentNullException("cacheKeyStrategy");
		}
		onCacheError = onCacheError ?? ((Action<Context, string, Exception>)delegate
		{
		});
		Action<Context, string> action = delegate
		{
		};
		return Cache(cacheProvider, ttlStrategy, cacheKeyStrategy.GetCacheKey, action, action, action, onCacheError, onCacheError);
	}

	public static CachePolicy Cache(ISyncCacheProvider cacheProvider, TimeSpan ttl, Func<Context, string> cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		return Cache(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy, onCacheError);
	}

	public static CachePolicy Cache(ISyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		if (ttlStrategy == null)
		{
			throw new ArgumentNullException("ttlStrategy");
		}
		if (cacheKeyStrategy == null)
		{
			throw new ArgumentNullException("cacheKeyStrategy");
		}
		onCacheError = onCacheError ?? ((Action<Context, string, Exception>)delegate
		{
		});
		Action<Context, string> action = delegate
		{
		};
		return Cache(cacheProvider, ttlStrategy, cacheKeyStrategy, action, action, action, onCacheError, onCacheError);
	}

	public static CachePolicy Cache(ISyncCacheProvider cacheProvider, TimeSpan ttl, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return Cache(cacheProvider, new RelativeTtl(ttl), DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy Cache(ISyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return Cache(cacheProvider, ttlStrategy, DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy Cache(ISyncCacheProvider cacheProvider, TimeSpan ttl, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return Cache(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy Cache(ISyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return Cache(cacheProvider, ttlStrategy, cacheKeyStrategy.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy Cache(ISyncCacheProvider cacheProvider, TimeSpan ttl, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return Cache(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy Cache(ISyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		if (ttlStrategy == null)
		{
			throw new ArgumentNullException("ttlStrategy");
		}
		if (cacheKeyStrategy == null)
		{
			throw new ArgumentNullException("cacheKeyStrategy");
		}
		if (onCacheGet == null)
		{
			throw new ArgumentNullException("onCacheGet");
		}
		if (onCacheMiss == null)
		{
			throw new ArgumentNullException("onCacheMiss");
		}
		if (onCachePut == null)
		{
			throw new ArgumentNullException("onCachePut");
		}
		if (onCachePutError == null)
		{
			throw new ArgumentNullException("onCachePutError");
		}
		if (onCachePutError == null)
		{
			throw new ArgumentNullException("onCachePutError");
		}
		return new CachePolicy(cacheProvider, ttlStrategy, cacheKeyStrategy, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider cacheProvider, TimeSpan ttl, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return Cache(cacheProvider.For<TResult>(), new RelativeTtl(ttl), DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return Cache(cacheProvider.For<TResult>(), ttlStrategy, DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider cacheProvider, TimeSpan ttl, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return Cache(cacheProvider.For<TResult>(), new RelativeTtl(ttl), cacheKeyStrategy.GetCacheKey, onCacheError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return Cache(cacheProvider.For<TResult>(), ttlStrategy, cacheKeyStrategy.GetCacheKey, onCacheError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider cacheProvider, TimeSpan ttl, Func<Context, string> cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return Cache(cacheProvider.For<TResult>(), new RelativeTtl(ttl), cacheKeyStrategy, onCacheError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return Cache(cacheProvider.For<TResult>(), ttlStrategy, cacheKeyStrategy, onCacheError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider cacheProvider, TimeSpan ttl, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return Cache(cacheProvider.For<TResult>(), new RelativeTtl(ttl), DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return Cache(cacheProvider.For<TResult>(), ttlStrategy, DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider cacheProvider, TimeSpan ttl, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return Cache(cacheProvider.For<TResult>(), new RelativeTtl(ttl), cacheKeyStrategy.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return Cache(cacheProvider.For<TResult>(), ttlStrategy, cacheKeyStrategy.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider cacheProvider, TimeSpan ttl, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return Cache(cacheProvider.For<TResult>(), new RelativeTtl(ttl), cacheKeyStrategy, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider cacheProvider, ITtlStrategy ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		return Cache(cacheProvider.For<TResult>(), ttlStrategy, cacheKeyStrategy, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, TimeSpan ttl, Action<Context, string, Exception> onCacheError = null)
	{
		return Cache(cacheProvider, new RelativeTtl(ttl), DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, ITtlStrategy ttlStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		return Cache(cacheProvider, ttlStrategy, DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, ITtlStrategy<TResult> ttlStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		return Cache(cacheProvider, ttlStrategy, DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, TimeSpan ttl, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		return Cache(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy.GetCacheKey, onCacheError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, ITtlStrategy ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		onCacheError = onCacheError ?? ((Action<Context, string, Exception>)delegate
		{
		});
		Action<Context, string> action = delegate
		{
		};
		return Cache(cacheProvider, ttlStrategy.For<TResult>(), cacheKeyStrategy.GetCacheKey, action, action, action, onCacheError, onCacheError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, ITtlStrategy<TResult> ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		onCacheError = onCacheError ?? ((Action<Context, string, Exception>)delegate
		{
		});
		Action<Context, string> action = delegate
		{
		};
		return Cache(cacheProvider, ttlStrategy, cacheKeyStrategy, action, action, action, onCacheError, onCacheError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, TimeSpan ttl, Func<Context, string> cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		return Cache(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy, onCacheError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, ITtlStrategy ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		onCacheError = onCacheError ?? ((Action<Context, string, Exception>)delegate
		{
		});
		Action<Context, string> action = delegate
		{
		};
		return Cache(cacheProvider, ttlStrategy.For<TResult>(), cacheKeyStrategy, action, action, action, onCacheError, onCacheError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, ITtlStrategy<TResult> ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string, Exception> onCacheError = null)
	{
		onCacheError = onCacheError ?? ((Action<Context, string, Exception>)delegate
		{
		});
		Action<Context, string> action = delegate
		{
		};
		return Cache(cacheProvider, ttlStrategy, cacheKeyStrategy, action, action, action, onCacheError, onCacheError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, TimeSpan ttl, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return Cache(cacheProvider, new RelativeTtl(ttl), DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, ITtlStrategy ttlStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return Cache(cacheProvider, ttlStrategy, DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, ITtlStrategy<TResult> ttlStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return Cache(cacheProvider, ttlStrategy, DefaultCacheKeyStrategy.Instance.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, TimeSpan ttl, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return Cache(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, ITtlStrategy ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return Cache(cacheProvider, ttlStrategy, cacheKeyStrategy.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, ITtlStrategy<TResult> ttlStrategy, ICacheKeyStrategy cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return Cache(cacheProvider, ttlStrategy, cacheKeyStrategy.GetCacheKey, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, TimeSpan ttl, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return Cache(cacheProvider, new RelativeTtl(ttl), cacheKeyStrategy, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, ITtlStrategy ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		return Cache(cacheProvider, ttlStrategy.For<TResult>(), cacheKeyStrategy, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static CachePolicy<TResult> Cache<TResult>(ISyncCacheProvider<TResult> cacheProvider, ITtlStrategy<TResult> ttlStrategy, Func<Context, string> cacheKeyStrategy, Action<Context, string> onCacheGet, Action<Context, string> onCacheMiss, Action<Context, string> onCachePut, Action<Context, string, Exception> onCacheGetError, Action<Context, string, Exception> onCachePutError)
	{
		if (cacheProvider == null)
		{
			throw new ArgumentNullException("cacheProvider");
		}
		if (ttlStrategy == null)
		{
			throw new ArgumentNullException("ttlStrategy");
		}
		if (cacheKeyStrategy == null)
		{
			throw new ArgumentNullException("cacheKeyStrategy");
		}
		if (onCacheGet == null)
		{
			throw new ArgumentNullException("onCacheGet");
		}
		if (onCacheMiss == null)
		{
			throw new ArgumentNullException("onCacheMiss");
		}
		if (onCachePut == null)
		{
			throw new ArgumentNullException("onCachePut");
		}
		if (onCachePutError == null)
		{
			throw new ArgumentNullException("onCachePutError");
		}
		if (onCachePutError == null)
		{
			throw new ArgumentNullException("onCachePutError");
		}
		return new CachePolicy<TResult>(cacheProvider, ttlStrategy, cacheKeyStrategy, onCacheGet, onCacheMiss, onCachePut, onCacheGetError, onCachePutError);
	}

	public static AsyncNoOpPolicy NoOpAsync()
	{
		return new AsyncNoOpPolicy();
	}

	public static AsyncNoOpPolicy<TResult> NoOpAsync<TResult>()
	{
		return new AsyncNoOpPolicy<TResult>();
	}

	public static NoOpPolicy NoOp()
	{
		return new NoOpPolicy();
	}

	public static NoOpPolicy<TResult> NoOp<TResult>()
	{
		return new NoOpPolicy<TResult>();
	}

	public Policy WithPolicyKey(string policyKey)
	{
		if (policyKeyInternal != null)
		{
			throw PolicyBase.PolicyKeyMustBeImmutableException;
		}
		policyKeyInternal = policyKey;
		return this;
	}

	ISyncPolicy ISyncPolicy.WithPolicyKey(string policyKey)
	{
		if (policyKeyInternal != null)
		{
			throw PolicyBase.PolicyKeyMustBeImmutableException;
		}
		policyKeyInternal = policyKey;
		return this;
	}

	internal Policy(ExceptionPredicates exceptionPredicates)
		: base(exceptionPredicates)
	{
	}

	protected Policy(PolicyBuilder policyBuilder = null)
		: base(policyBuilder)
	{
	}

	[DebuggerStepThrough]
	public void Execute(Action action)
	{
		Execute(delegate
		{
			action();
		}, new Context(), DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public void Execute(Action<Context> action, IDictionary<string, object> contextData)
	{
		Execute(delegate(Context ctx, CancellationToken ct)
		{
			action(ctx);
		}, new Context(contextData), DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public void Execute(Action<Context> action, Context context)
	{
		Execute(delegate(Context ctx, CancellationToken ct)
		{
			action(ctx);
		}, context, DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public void Execute(Action<CancellationToken> action, CancellationToken cancellationToken)
	{
		Execute(delegate(Context ctx, CancellationToken ct)
		{
			action(ct);
		}, new Context(), cancellationToken);
	}

	[DebuggerStepThrough]
	public void Execute(Action<Context, CancellationToken> action, IDictionary<string, object> contextData, CancellationToken cancellationToken)
	{
		Execute(action, new Context(contextData), cancellationToken);
	}

	[DebuggerStepThrough]
	public void Execute(Action<Context, CancellationToken> action, Context context, CancellationToken cancellationToken)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		SetPolicyContext(context, out var priorPolicyWrapKey, out var priorPolicyKey);
		try
		{
			Implementation(action, context, cancellationToken);
		}
		finally
		{
			RestorePolicyContext(context, priorPolicyWrapKey, priorPolicyKey);
		}
	}

	[DebuggerStepThrough]
	public TResult Execute<TResult>(Func<TResult> action)
	{
		return Execute((Context ctx, CancellationToken ct) => action(), new Context(), DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public TResult Execute<TResult>(Func<Context, TResult> action, IDictionary<string, object> contextData)
	{
		return Execute((Context ctx, CancellationToken ct) => action(ctx), new Context(contextData), DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public TResult Execute<TResult>(Func<Context, TResult> action, Context context)
	{
		return Execute((Context ctx, CancellationToken ct) => action(ctx), context, DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public TResult Execute<TResult>(Func<CancellationToken, TResult> action, CancellationToken cancellationToken)
	{
		return Execute((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken);
	}

	[DebuggerStepThrough]
	public TResult Execute<TResult>(Func<Context, CancellationToken, TResult> action, IDictionary<string, object> contextData, CancellationToken cancellationToken)
	{
		return Execute(action, new Context(contextData), cancellationToken);
	}

	[DebuggerStepThrough]
	public TResult Execute<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		SetPolicyContext(context, out var priorPolicyWrapKey, out var priorPolicyKey);
		try
		{
			return Implementation(action, context, cancellationToken);
		}
		finally
		{
			RestorePolicyContext(context, priorPolicyWrapKey, priorPolicyKey);
		}
	}

	[DebuggerStepThrough]
	public PolicyResult ExecuteAndCapture(Action action)
	{
		return ExecuteAndCapture(delegate
		{
			action();
		}, new Context(), DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public PolicyResult ExecuteAndCapture(Action<Context> action, IDictionary<string, object> contextData)
	{
		return ExecuteAndCapture(delegate(Context ctx, CancellationToken ct)
		{
			action(ctx);
		}, new Context(contextData), DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public PolicyResult ExecuteAndCapture(Action<Context> action, Context context)
	{
		return ExecuteAndCapture(delegate(Context ctx, CancellationToken ct)
		{
			action(ctx);
		}, context, DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public PolicyResult ExecuteAndCapture(Action<CancellationToken> action, CancellationToken cancellationToken)
	{
		return ExecuteAndCapture(delegate(Context ctx, CancellationToken ct)
		{
			action(ct);
		}, new Context(), cancellationToken);
	}

	[DebuggerStepThrough]
	public PolicyResult ExecuteAndCapture(Action<Context, CancellationToken> action, IDictionary<string, object> contextData, CancellationToken cancellationToken)
	{
		return ExecuteAndCapture(action, new Context(contextData), cancellationToken);
	}

	[DebuggerStepThrough]
	public PolicyResult ExecuteAndCapture(Action<Context, CancellationToken> action, Context context, CancellationToken cancellationToken)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		try
		{
			Execute(action, context, cancellationToken);
			return PolicyResult.Successful(context);
		}
		catch (Exception exception)
		{
			return PolicyResult.Failure(exception, PolicyBase.GetExceptionType(base.ExceptionPredicates, exception), context);
		}
	}

	[DebuggerStepThrough]
	public PolicyResult<TResult> ExecuteAndCapture<TResult>(Func<TResult> action)
	{
		return ExecuteAndCapture((Context ctx, CancellationToken ct) => action(), new Context(), DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public PolicyResult<TResult> ExecuteAndCapture<TResult>(Func<Context, TResult> action, IDictionary<string, object> contextData)
	{
		return ExecuteAndCapture((Context ctx, CancellationToken ct) => action(ctx), new Context(contextData), DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public PolicyResult<TResult> ExecuteAndCapture<TResult>(Func<Context, TResult> action, Context context)
	{
		return ExecuteAndCapture((Context ctx, CancellationToken ct) => action(ctx), context, DefaultCancellationToken);
	}

	public PolicyResult<TResult> ExecuteAndCapture<TResult>(Func<CancellationToken, TResult> action, CancellationToken cancellationToken)
	{
		return ExecuteAndCapture((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken);
	}

	[DebuggerStepThrough]
	public PolicyResult<TResult> ExecuteAndCapture<TResult>(Func<Context, CancellationToken, TResult> action, IDictionary<string, object> contextData, CancellationToken cancellationToken)
	{
		return ExecuteAndCapture(action, new Context(contextData), cancellationToken);
	}

	[DebuggerStepThrough]
	public PolicyResult<TResult> ExecuteAndCapture<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		try
		{
			return PolicyResult<TResult>.Successful(Execute(action, context, cancellationToken), context);
		}
		catch (Exception exception)
		{
			return PolicyResult<TResult>.Failure(exception, PolicyBase.GetExceptionType(base.ExceptionPredicates, exception), context);
		}
	}

	public static PolicyBuilder Handle<TException>() where TException : Exception
	{
		return new PolicyBuilder((Exception exception) => (!(exception is TException)) ? null : exception);
	}

	public static PolicyBuilder Handle<TException>(Func<TException, bool> exceptionPredicate) where TException : Exception
	{
		return new PolicyBuilder((Exception exception) => (!(exception is TException arg) || !exceptionPredicate(arg)) ? null : exception);
	}

	public static PolicyBuilder HandleInner<TException>() where TException : Exception
	{
		return new PolicyBuilder(PolicyBuilder.HandleInner((Exception ex) => ex is TException));
	}

	public static PolicyBuilder HandleInner<TException>(Func<TException, bool> exceptionPredicate) where TException : Exception
	{
		return new PolicyBuilder(PolicyBuilder.HandleInner((Exception ex) => ex is TException arg && exceptionPredicate(arg)));
	}

	public static PolicyBuilder<TResult> HandleResult<TResult>(Func<TResult, bool> resultPredicate)
	{
		return new PolicyBuilder<TResult>(resultPredicate);
	}

	public static PolicyBuilder<TResult> HandleResult<TResult>(TResult result)
	{
		return HandleResult((TResult r) => (r != null && r.Equals(result)) || (r == null && result == null));
	}

	[DebuggerStepThrough]
	protected virtual void Implementation(Action<Context, CancellationToken> action, Context context, CancellationToken cancellationToken)
	{
		Implementation(delegate(Context ctx, CancellationToken token)
		{
			action(ctx, token);
			return EmptyStruct.Instance;
		}, context, cancellationToken);
	}

	protected abstract TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken);

	public static AsyncTimeoutPolicy TimeoutAsync(int seconds)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		return TimeoutAsync(onTimeoutAsync: (Context _, TimeSpan __, Task ___, Exception ____) => TaskHelper.EmptyTask, timeoutProvider: (Context ctx) => TimeSpan.FromSeconds((double)seconds), timeoutStrategy: TimeoutStrategy.Optimistic);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(int seconds, TimeoutStrategy timeoutStrategy)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		return TimeoutAsync(onTimeoutAsync: (Context _, TimeSpan __, Task ___, Exception ____) => TaskHelper.EmptyTask, timeoutProvider: (Context ctx) => TimeSpan.FromSeconds((double)seconds), timeoutStrategy: timeoutStrategy);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(int seconds, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		if (onTimeoutAsync == null)
		{
			throw new ArgumentNullException("onTimeoutAsync");
		}
		return TimeoutAsync((Context ctx) => TimeSpan.FromSeconds((double)seconds), TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(int seconds, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		if (seconds <= 0)
		{
			throw new ArgumentOutOfRangeException("seconds");
		}
		if (onTimeoutAsync == null)
		{
			throw new ArgumentNullException("onTimeoutAsync");
		}
		return TimeoutAsync((Context ctx) => TimeSpan.FromSeconds((double)seconds), TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(int seconds, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		return TimeoutAsync((Context ctx) => TimeSpan.FromSeconds((double)seconds), timeoutStrategy, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(int seconds, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		if (seconds <= 0)
		{
			throw new ArgumentOutOfRangeException("seconds");
		}
		return TimeoutAsync((Context ctx) => TimeSpan.FromSeconds((double)seconds), timeoutStrategy, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(TimeSpan timeout)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return TimeoutAsync(onTimeoutAsync: (Context _, TimeSpan __, Task ___, Exception ____) => TaskHelper.EmptyTask, timeoutProvider: (Context ctx) => timeout, timeoutStrategy: TimeoutStrategy.Optimistic);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(TimeSpan timeout, TimeoutStrategy timeoutStrategy)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return TimeoutAsync(onTimeoutAsync: (Context _, TimeSpan __, Task ___, Exception ____) => TaskHelper.EmptyTask, timeoutProvider: (Context ctx) => timeout, timeoutStrategy: timeoutStrategy);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(TimeSpan timeout, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return TimeoutAsync((Context ctx) => timeout, TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(TimeSpan timeout, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return TimeoutAsync((Context ctx) => timeout, TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(TimeSpan timeout, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return TimeoutAsync((Context ctx) => timeout, timeoutStrategy, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(TimeSpan timeout, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return TimeoutAsync((Context ctx) => timeout, timeoutStrategy, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(Func<TimeSpan> timeoutProvider)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return TimeoutAsync(onTimeoutAsync: (Context _, TimeSpan __, Task ___, Exception ____) => TaskHelper.EmptyTask, timeoutProvider: (Context ctx) => timeoutProvider(), timeoutStrategy: TimeoutStrategy.Optimistic);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(Func<TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return TimeoutAsync(onTimeoutAsync: (Context _, TimeSpan __, Task ___, Exception ____) => TaskHelper.EmptyTask, timeoutProvider: (Context ctx) => timeoutProvider(), timeoutStrategy: timeoutStrategy);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(Func<TimeSpan> timeoutProvider, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return TimeoutAsync((Context ctx) => timeoutProvider(), TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(Func<TimeSpan> timeoutProvider, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return TimeoutAsync((Context ctx) => timeoutProvider(), TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(Func<TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return TimeoutAsync((Context ctx) => timeoutProvider(), timeoutStrategy, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(Func<TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return TimeoutAsync((Context ctx) => timeoutProvider(), timeoutStrategy, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(Func<Context, TimeSpan> timeoutProvider)
	{
		Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync = (Context _, TimeSpan __, Task ___, Exception ____) => TaskHelper.EmptyTask;
		return TimeoutAsync(timeoutProvider, TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy)
	{
		Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync = (Context _, TimeSpan __, Task ___, Exception ____) => TaskHelper.EmptyTask;
		return TimeoutAsync(timeoutProvider, timeoutStrategy, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(Func<Context, TimeSpan> timeoutProvider, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		return TimeoutAsync(timeoutProvider, TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(Func<Context, TimeSpan> timeoutProvider, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		return TimeoutAsync(timeoutProvider, TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy TimeoutAsync(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		if (onTimeoutAsync == null)
		{
			throw new ArgumentNullException("onTimeoutAsync");
		}
		return TimeoutAsync(timeoutProvider, timeoutStrategy, (Context ctx, TimeSpan timeout, Task task, Exception ex) => onTimeoutAsync(ctx, timeout, task));
	}

	public static AsyncTimeoutPolicy TimeoutAsync(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		if (onTimeoutAsync == null)
		{
			throw new ArgumentNullException("onTimeoutAsync");
		}
		return new AsyncTimeoutPolicy(timeoutProvider, timeoutStrategy, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(int seconds)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		return TimeoutAsync<TResult>(onTimeoutAsync: (Context _, TimeSpan __, Task ___, Exception ____) => Task.FromResult(default(TResult)), timeoutProvider: (Context ctx) => TimeSpan.FromSeconds((double)seconds), timeoutStrategy: TimeoutStrategy.Optimistic);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(int seconds, TimeoutStrategy timeoutStrategy)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		return TimeoutAsync<TResult>(onTimeoutAsync: (Context _, TimeSpan __, Task ___, Exception ____) => Task.FromResult(default(TResult)), timeoutProvider: (Context ctx) => TimeSpan.FromSeconds((double)seconds), timeoutStrategy: timeoutStrategy);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(int seconds, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		return TimeoutAsync<TResult>((Context ctx) => TimeSpan.FromSeconds((double)seconds), TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(int seconds, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		if (seconds <= 0)
		{
			throw new ArgumentOutOfRangeException("seconds");
		}
		return TimeoutAsync<TResult>((Context ctx) => TimeSpan.FromSeconds((double)seconds), TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(int seconds, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		return TimeoutAsync<TResult>((Context ctx) => TimeSpan.FromSeconds((double)seconds), timeoutStrategy, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(int seconds, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		if (seconds <= 0)
		{
			throw new ArgumentOutOfRangeException("seconds");
		}
		return TimeoutAsync<TResult>((Context ctx) => TimeSpan.FromSeconds((double)seconds), timeoutStrategy, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(TimeSpan timeout)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return TimeoutAsync<TResult>(onTimeoutAsync: (Context _, TimeSpan __, Task ___, Exception ____) => Task.FromResult(default(TResult)), timeoutProvider: (Context ctx) => timeout, timeoutStrategy: TimeoutStrategy.Optimistic);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(TimeSpan timeout, TimeoutStrategy timeoutStrategy)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return TimeoutAsync<TResult>(onTimeoutAsync: (Context _, TimeSpan __, Task ___, Exception ____) => Task.FromResult(default(TResult)), timeoutProvider: (Context ctx) => timeout, timeoutStrategy: timeoutStrategy);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(TimeSpan timeout, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		if (onTimeoutAsync == null)
		{
			throw new ArgumentNullException("onTimeoutAsync");
		}
		return TimeoutAsync<TResult>((Context ctx) => timeout, TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(TimeSpan timeout, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		if (timeout <= TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException("timeout");
		}
		if (onTimeoutAsync == null)
		{
			throw new ArgumentNullException("onTimeoutAsync");
		}
		return TimeoutAsync<TResult>((Context ctx) => timeout, TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(TimeSpan timeout, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return TimeoutAsync<TResult>((Context ctx) => timeout, timeoutStrategy, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(TimeSpan timeout, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		if (timeout <= TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException("timeout");
		}
		return TimeoutAsync<TResult>((Context ctx) => timeout, timeoutStrategy, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(Func<TimeSpan> timeoutProvider)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return TimeoutAsync<TResult>(onTimeoutAsync: (Context _, TimeSpan __, Task ___, Exception ____) => Task.FromResult(default(TResult)), timeoutProvider: (Context ctx) => timeoutProvider(), timeoutStrategy: TimeoutStrategy.Optimistic);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(Func<TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return TimeoutAsync<TResult>(onTimeoutAsync: (Context _, TimeSpan __, Task ___, Exception ____) => Task.FromResult(default(TResult)), timeoutProvider: (Context ctx) => timeoutProvider(), timeoutStrategy: timeoutStrategy);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(Func<TimeSpan> timeoutProvider, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return TimeoutAsync<TResult>((Context ctx) => timeoutProvider(), TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(Func<TimeSpan> timeoutProvider, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return TimeoutAsync<TResult>((Context ctx) => timeoutProvider(), TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(Func<TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return TimeoutAsync<TResult>((Context ctx) => timeoutProvider(), timeoutStrategy, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(Func<TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return TimeoutAsync<TResult>((Context ctx) => timeoutProvider(), timeoutStrategy, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(Func<Context, TimeSpan> timeoutProvider)
	{
		Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync = (Context _, TimeSpan __, Task ___, Exception ____) => Task.FromResult(default(TResult));
		return TimeoutAsync<TResult>(timeoutProvider, TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy)
	{
		Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync = (Context _, TimeSpan __, Task ___, Exception ____) => Task.FromResult(default(TResult));
		return TimeoutAsync<TResult>(timeoutProvider, timeoutStrategy, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(Func<Context, TimeSpan> timeoutProvider, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		return TimeoutAsync<TResult>(timeoutProvider, TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(Func<Context, TimeSpan> timeoutProvider, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		return TimeoutAsync<TResult>(timeoutProvider, TimeoutStrategy.Optimistic, onTimeoutAsync);
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Task> onTimeoutAsync)
	{
		if (onTimeoutAsync == null)
		{
			throw new ArgumentNullException("onTimeoutAsync");
		}
		return TimeoutAsync<TResult>(timeoutProvider, timeoutStrategy, (Context ctx, TimeSpan timeout, Task task, Exception ex) => onTimeoutAsync(ctx, timeout, task));
	}

	public static AsyncTimeoutPolicy<TResult> TimeoutAsync<TResult>(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Func<Context, TimeSpan, Task, Exception, Task> onTimeoutAsync)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		if (onTimeoutAsync == null)
		{
			throw new ArgumentNullException("onTimeoutAsync");
		}
		return new AsyncTimeoutPolicy<TResult>(timeoutProvider, timeoutStrategy, onTimeoutAsync);
	}

	public static TimeoutPolicy Timeout(int seconds)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		return Timeout(onTimeout: (Action<Context, TimeSpan, Task, Exception>)delegate
		{
		}, timeoutProvider: (Func<Context, TimeSpan>)((Context ctx) => TimeSpan.FromSeconds((double)seconds)), timeoutStrategy: TimeoutStrategy.Optimistic);
	}

	public static TimeoutPolicy Timeout(int seconds, TimeoutStrategy timeoutStrategy)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		return Timeout(onTimeout: (Action<Context, TimeSpan, Task, Exception>)delegate
		{
		}, timeoutProvider: (Func<Context, TimeSpan>)((Context ctx) => TimeSpan.FromSeconds((double)seconds)), timeoutStrategy: timeoutStrategy);
	}

	public static TimeoutPolicy Timeout(int seconds, Action<Context, TimeSpan, Task> onTimeout)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		return Timeout((Context ctx) => TimeSpan.FromSeconds((double)seconds), TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy Timeout(int seconds, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		if (seconds <= 0)
		{
			throw new ArgumentOutOfRangeException("seconds");
		}
		return Timeout((Context ctx) => TimeSpan.FromSeconds((double)seconds), TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy Timeout(int seconds, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task> onTimeout)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		return Timeout((Context ctx) => TimeSpan.FromSeconds((double)seconds), timeoutStrategy, onTimeout);
	}

	public static TimeoutPolicy Timeout(int seconds, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		if (seconds <= 0)
		{
			throw new ArgumentOutOfRangeException("seconds");
		}
		return Timeout((Context ctx) => TimeSpan.FromSeconds((double)seconds), timeoutStrategy, onTimeout);
	}

	public static TimeoutPolicy Timeout(TimeSpan timeout)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return Timeout(onTimeout: (Action<Context, TimeSpan, Task, Exception>)delegate
		{
		}, timeoutProvider: (Func<Context, TimeSpan>)((Context ctx) => timeout), timeoutStrategy: TimeoutStrategy.Optimistic);
	}

	public static TimeoutPolicy Timeout(TimeSpan timeout, TimeoutStrategy timeoutStrategy)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return Timeout(onTimeout: (Action<Context, TimeSpan, Task, Exception>)delegate
		{
		}, timeoutProvider: (Func<Context, TimeSpan>)((Context ctx) => timeout), timeoutStrategy: timeoutStrategy);
	}

	public static TimeoutPolicy Timeout(TimeSpan timeout, Action<Context, TimeSpan, Task> onTimeout)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return Timeout((Context ctx) => timeout, TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy Timeout(TimeSpan timeout, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return Timeout((Context ctx) => timeout, TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy Timeout(TimeSpan timeout, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task> onTimeout)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return Timeout((Context ctx) => timeout, timeoutStrategy, onTimeout);
	}

	public static TimeoutPolicy Timeout(TimeSpan timeout, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return Timeout((Context ctx) => timeout, timeoutStrategy, onTimeout);
	}

	public static TimeoutPolicy Timeout(Func<TimeSpan> timeoutProvider)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return Timeout(onTimeout: (Action<Context, TimeSpan, Task, Exception>)delegate
		{
		}, timeoutProvider: (Func<Context, TimeSpan>)((Context ctx) => timeoutProvider()), timeoutStrategy: TimeoutStrategy.Optimistic);
	}

	public static TimeoutPolicy Timeout(Func<TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return Timeout(onTimeout: (Action<Context, TimeSpan, Task, Exception>)delegate
		{
		}, timeoutProvider: (Func<Context, TimeSpan>)((Context ctx) => timeoutProvider()), timeoutStrategy: timeoutStrategy);
	}

	public static TimeoutPolicy Timeout(Func<TimeSpan> timeoutProvider, Action<Context, TimeSpan, Task> onTimeout)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return Timeout((Context ctx) => timeoutProvider(), TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy Timeout(Func<TimeSpan> timeoutProvider, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return Timeout((Context ctx) => timeoutProvider(), TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy Timeout(Func<TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task> onTimeout)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return Timeout((Context ctx) => timeoutProvider(), timeoutStrategy, onTimeout);
	}

	public static TimeoutPolicy Timeout(Func<TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return Timeout((Context ctx) => timeoutProvider(), timeoutStrategy, onTimeout);
	}

	public static TimeoutPolicy Timeout(Func<Context, TimeSpan> timeoutProvider)
	{
		Action<Context, TimeSpan, Task, Exception> onTimeout = delegate
		{
		};
		return Timeout(timeoutProvider, TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy Timeout(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy)
	{
		Action<Context, TimeSpan, Task, Exception> onTimeout = delegate
		{
		};
		return Timeout(timeoutProvider, timeoutStrategy, onTimeout);
	}

	public static TimeoutPolicy Timeout(Func<Context, TimeSpan> timeoutProvider, Action<Context, TimeSpan, Task> onTimeout)
	{
		return Timeout(timeoutProvider, TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy Timeout(Func<Context, TimeSpan> timeoutProvider, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		return Timeout(timeoutProvider, TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy Timeout(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task> onTimeout)
	{
		if (onTimeout == null)
		{
			throw new ArgumentNullException("onTimeout");
		}
		return Timeout(timeoutProvider, timeoutStrategy, delegate(Context ctx, TimeSpan timeout, Task task, Exception ex)
		{
			onTimeout(ctx, timeout, task);
		});
	}

	public static TimeoutPolicy Timeout(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		if (onTimeout == null)
		{
			throw new ArgumentNullException("onTimeout");
		}
		return new TimeoutPolicy(timeoutProvider, timeoutStrategy, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(int seconds)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		return Policy.Timeout<TResult>(onTimeout: (Action<Context, TimeSpan, Task, Exception>)delegate
		{
		}, timeoutProvider: (Func<Context, TimeSpan>)((Context ctx) => TimeSpan.FromSeconds((double)seconds)), timeoutStrategy: TimeoutStrategy.Optimistic);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(int seconds, TimeoutStrategy timeoutStrategy)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		return Policy.Timeout<TResult>(onTimeout: (Action<Context, TimeSpan, Task, Exception>)delegate
		{
		}, timeoutProvider: (Func<Context, TimeSpan>)((Context ctx) => TimeSpan.FromSeconds((double)seconds)), timeoutStrategy: timeoutStrategy);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(int seconds, Action<Context, TimeSpan, Task> onTimeout)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		return Timeout<TResult>((Context ctx) => TimeSpan.FromSeconds((double)seconds), TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(int seconds, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		if (seconds <= 0)
		{
			throw new ArgumentOutOfRangeException("seconds");
		}
		return Timeout<TResult>((Context ctx) => TimeSpan.FromSeconds((double)seconds), TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(int seconds, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task> onTimeout)
	{
		TimeoutValidator.ValidateSecondsTimeout(seconds);
		return Timeout<TResult>((Context ctx) => TimeSpan.FromSeconds((double)seconds), timeoutStrategy, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(int seconds, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		if (seconds <= 0)
		{
			throw new ArgumentOutOfRangeException("seconds");
		}
		return Timeout<TResult>((Context ctx) => TimeSpan.FromSeconds((double)seconds), timeoutStrategy, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(TimeSpan timeout)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return Policy.Timeout<TResult>(onTimeout: (Action<Context, TimeSpan, Task, Exception>)delegate
		{
		}, timeoutProvider: (Func<Context, TimeSpan>)((Context ctx) => timeout), timeoutStrategy: TimeoutStrategy.Optimistic);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(TimeSpan timeout, TimeoutStrategy timeoutStrategy)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return Policy.Timeout<TResult>(onTimeout: (Action<Context, TimeSpan, Task, Exception>)delegate
		{
		}, timeoutProvider: (Func<Context, TimeSpan>)((Context ctx) => timeout), timeoutStrategy: timeoutStrategy);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(TimeSpan timeout, Action<Context, TimeSpan, Task> onTimeout)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return Timeout<TResult>((Context ctx) => timeout, TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(TimeSpan timeout, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return Timeout<TResult>((Context ctx) => timeout, TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(TimeSpan timeout, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task> onTimeout)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return Timeout<TResult>((Context ctx) => timeout, timeoutStrategy, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(TimeSpan timeout, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		TimeoutValidator.ValidateTimeSpanTimeout(timeout);
		return Timeout<TResult>((Context ctx) => timeout, timeoutStrategy, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(Func<TimeSpan> timeoutProvider)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return Policy.Timeout<TResult>(onTimeout: (Action<Context, TimeSpan, Task, Exception>)delegate
		{
		}, timeoutProvider: (Func<Context, TimeSpan>)((Context ctx) => timeoutProvider()), timeoutStrategy: TimeoutStrategy.Optimistic);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(Func<TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return Policy.Timeout<TResult>(onTimeout: (Action<Context, TimeSpan, Task, Exception>)delegate
		{
		}, timeoutProvider: (Func<Context, TimeSpan>)((Context ctx) => timeoutProvider()), timeoutStrategy: timeoutStrategy);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(Func<TimeSpan> timeoutProvider, Action<Context, TimeSpan, Task> onTimeout)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return Timeout<TResult>((Context ctx) => timeoutProvider(), TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(Func<TimeSpan> timeoutProvider, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return Timeout<TResult>((Context ctx) => timeoutProvider(), TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(Func<TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task> onTimeout)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return Timeout<TResult>((Context ctx) => timeoutProvider(), timeoutStrategy, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(Func<TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		return Timeout<TResult>((Context ctx) => timeoutProvider(), timeoutStrategy, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(Func<Context, TimeSpan> timeoutProvider)
	{
		Action<Context, TimeSpan, Task, Exception> onTimeout = delegate
		{
		};
		return Timeout<TResult>(timeoutProvider, TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy)
	{
		Action<Context, TimeSpan, Task, Exception> onTimeout = delegate
		{
		};
		return Timeout<TResult>(timeoutProvider, timeoutStrategy, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(Func<Context, TimeSpan> timeoutProvider, Action<Context, TimeSpan, Task> onTimeout)
	{
		return Timeout<TResult>(timeoutProvider, TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(Func<Context, TimeSpan> timeoutProvider, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		return Timeout<TResult>(timeoutProvider, TimeoutStrategy.Optimistic, onTimeout);
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task> onTimeout)
	{
		if (onTimeout == null)
		{
			throw new ArgumentNullException("onTimeout");
		}
		return Timeout<TResult>(timeoutProvider, timeoutStrategy, delegate(Context ctx, TimeSpan timeout, Task task, Exception ex)
		{
			onTimeout(ctx, timeout, task);
		});
	}

	public static TimeoutPolicy<TResult> Timeout<TResult>(Func<Context, TimeSpan> timeoutProvider, TimeoutStrategy timeoutStrategy, Action<Context, TimeSpan, Task, Exception> onTimeout)
	{
		if (timeoutProvider == null)
		{
			throw new ArgumentNullException("timeoutProvider");
		}
		if (onTimeout == null)
		{
			throw new ArgumentNullException("onTimeout");
		}
		return new TimeoutPolicy<TResult>(timeoutProvider, timeoutStrategy, onTimeout);
	}

	public static AsyncPolicyWrap WrapAsync(params IAsyncPolicy[] policies)
	{
		switch (policies.Length)
		{
		case 0:
		case 1:
			throw new ArgumentException("The enumerable of policies to form the wrap must contain at least two policies.", "policies");
		case 2:
			return new AsyncPolicyWrap((AsyncPolicy)policies[0], policies[1]);
		default:
			return WrapAsync(policies[0], WrapAsync(policies.Skip(1).ToArray()));
		}
	}

	public static AsyncPolicyWrap<TResult> WrapAsync<TResult>(params IAsyncPolicy<TResult>[] policies)
	{
		switch (policies.Length)
		{
		case 0:
		case 1:
			throw new ArgumentException("The enumerable of policies to form the wrap must contain at least two policies.", "policies");
		case 2:
			return new AsyncPolicyWrap<TResult>((AsyncPolicy<TResult>)policies[0], policies[1]);
		default:
			return WrapAsync<TResult>(policies[0], WrapAsync(policies.Skip(1).ToArray()));
		}
	}

	public PolicyWrap Wrap(ISyncPolicy innerPolicy)
	{
		if (innerPolicy == null)
		{
			throw new ArgumentNullException("innerPolicy");
		}
		return new PolicyWrap(this, innerPolicy);
	}

	public PolicyWrap<TResult> Wrap<TResult>(ISyncPolicy<TResult> innerPolicy)
	{
		if (innerPolicy == null)
		{
			throw new ArgumentNullException("innerPolicy");
		}
		return new PolicyWrap<TResult>(this, innerPolicy);
	}

	public static PolicyWrap Wrap(params ISyncPolicy[] policies)
	{
		switch (policies.Length)
		{
		case 0:
		case 1:
			throw new ArgumentException("The enumerable of policies to form the wrap must contain at least two policies.", "policies");
		case 2:
			return new PolicyWrap((Policy)policies[0], policies[1]);
		default:
			return Wrap(policies[0], Wrap(policies.Skip(1).ToArray()));
		}
	}

	public static PolicyWrap<TResult> Wrap<TResult>(params ISyncPolicy<TResult>[] policies)
	{
		switch (policies.Length)
		{
		case 0:
		case 1:
			throw new ArgumentException("The enumerable of policies to form the wrap must contain at least two policies.", "policies");
		case 2:
			return new PolicyWrap<TResult>((Policy<TResult>)policies[0], policies[1]);
		default:
			return Wrap<TResult>(policies[0], Wrap(policies.Skip(1).ToArray()));
		}
	}
}
public abstract class Policy<TResult> : PolicyBase<TResult>, ISyncPolicy<TResult>, IsPolicy
{
	public Policy<TResult> WithPolicyKey(string policyKey)
	{
		if (policyKeyInternal != null)
		{
			throw PolicyBase.PolicyKeyMustBeImmutableException;
		}
		policyKeyInternal = policyKey;
		return this;
	}

	ISyncPolicy<TResult> ISyncPolicy<TResult>.WithPolicyKey(string policyKey)
	{
		if (policyKeyInternal != null)
		{
			throw PolicyBase.PolicyKeyMustBeImmutableException;
		}
		policyKeyInternal = policyKey;
		return this;
	}

	public static PolicyBuilder<TResult> Handle<TException>() where TException : Exception
	{
		return new PolicyBuilder<TResult>((Exception exception) => (!(exception is TException)) ? null : exception);
	}

	public static PolicyBuilder<TResult> Handle<TException>(Func<TException, bool> exceptionPredicate) where TException : Exception
	{
		return new PolicyBuilder<TResult>((Exception exception) => (!(exception is TException arg) || !exceptionPredicate(arg)) ? null : exception);
	}

	public static PolicyBuilder<TResult> HandleInner<TException>() where TException : Exception
	{
		return new PolicyBuilder<TResult>(PolicyBuilder.HandleInner((Exception ex) => ex is TException));
	}

	public static PolicyBuilder<TResult> HandleInner<TException>(Func<TException, bool> exceptionPredicate) where TException : Exception
	{
		return new PolicyBuilder<TResult>(PolicyBuilder.HandleInner((Exception ex) => ex is TException arg && exceptionPredicate(arg)));
	}

	public static PolicyBuilder<TResult> HandleResult(Func<TResult, bool> resultPredicate)
	{
		return new PolicyBuilder<TResult>(resultPredicate);
	}

	public static PolicyBuilder<TResult> HandleResult(TResult result)
	{
		return HandleResult((TResult r) => (r != null && r.Equals(result)) || (r == null && result == null));
	}

	protected abstract TResult Implementation(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken);

	internal Policy(ExceptionPredicates exceptionPredicates, ResultPredicates<TResult> resultPredicates)
		: base(exceptionPredicates, resultPredicates)
	{
	}

	protected Policy(PolicyBuilder<TResult> policyBuilder = null)
		: base(policyBuilder)
	{
	}

	[DebuggerStepThrough]
	public TResult Execute(Func<TResult> action)
	{
		return Execute((Context ctx, CancellationToken ct) => action(), new Context(), DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public TResult Execute(Func<Context, TResult> action, IDictionary<string, object> contextData)
	{
		return Execute((Context ctx, CancellationToken ct) => action(ctx), new Context(contextData), DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public TResult Execute(Func<Context, TResult> action, Context context)
	{
		return Execute((Context ctx, CancellationToken ct) => action(ctx), context, DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public TResult Execute(Func<CancellationToken, TResult> action, CancellationToken cancellationToken)
	{
		return Execute((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken);
	}

	[DebuggerStepThrough]
	public TResult Execute(Func<Context, CancellationToken, TResult> action, IDictionary<string, object> contextData, CancellationToken cancellationToken)
	{
		return Execute(action, new Context(contextData), cancellationToken);
	}

	[DebuggerStepThrough]
	public TResult Execute(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		SetPolicyContext(context, out var priorPolicyWrapKey, out var priorPolicyKey);
		try
		{
			return Implementation(action, context, cancellationToken);
		}
		finally
		{
			RestorePolicyContext(context, priorPolicyWrapKey, priorPolicyKey);
		}
	}

	[DebuggerStepThrough]
	public PolicyResult<TResult> ExecuteAndCapture(Func<TResult> action)
	{
		return ExecuteAndCapture((Context ctx, CancellationToken ct) => action(), new Context(), DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public PolicyResult<TResult> ExecuteAndCapture(Func<Context, TResult> action, IDictionary<string, object> contextData)
	{
		return ExecuteAndCapture((Context ctx, CancellationToken ct) => action(ctx), new Context(contextData), DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public PolicyResult<TResult> ExecuteAndCapture(Func<Context, TResult> action, Context context)
	{
		return ExecuteAndCapture((Context ctx, CancellationToken ct) => action(ctx), context, DefaultCancellationToken);
	}

	[DebuggerStepThrough]
	public PolicyResult<TResult> ExecuteAndCapture(Func<CancellationToken, TResult> action, CancellationToken cancellationToken)
	{
		return ExecuteAndCapture((Context ctx, CancellationToken ct) => action(ct), new Context(), cancellationToken);
	}

	[DebuggerStepThrough]
	public PolicyResult<TResult> ExecuteAndCapture(Func<Context, CancellationToken, TResult> action, IDictionary<string, object> contextData, CancellationToken cancellationToken)
	{
		return ExecuteAndCapture(action, new Context(contextData), cancellationToken);
	}

	[DebuggerStepThrough]
	public PolicyResult<TResult> ExecuteAndCapture(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		try
		{
			TResult val = Execute(action, context, cancellationToken);
			if (base.ResultPredicates.AnyMatch(val))
			{
				return PolicyResult<TResult>.Failure(val, context);
			}
			return PolicyResult<TResult>.Successful(val, context);
		}
		catch (Exception exception)
		{
			return PolicyResult<TResult>.Failure(exception, PolicyBase.GetExceptionType(base.ExceptionPredicates, exception), context);
		}
	}

	public PolicyWrap<TResult> Wrap(ISyncPolicy innerPolicy)
	{
		if (innerPolicy == null)
		{
			throw new ArgumentNullException("innerPolicy");
		}
		return new PolicyWrap<TResult>(this, innerPolicy);
	}

	public PolicyWrap<TResult> Wrap(ISyncPolicy<TResult> innerPolicy)
	{
		if (innerPolicy == null)
		{
			throw new ArgumentNullException("innerPolicy");
		}
		return new PolicyWrap<TResult>(this, innerPolicy);
	}
}
