using System;
using System.Collections.Concurrent;
using System.Threading;

namespace NuGet;

internal sealed class MemoryCache : IDisposable
{
	private sealed class CacheItem
	{
		private readonly Lazy<object> _valueFactory;

		private readonly bool _absoluteExpiration;

		private long _expires;

		public object Value => _valueFactory.Value;

		public bool Expired
		{
			get
			{
				long ticks = DateTime.UtcNow.Ticks;
				long num = Interlocked.Read(in _expires);
				return ticks > num;
			}
		}

		public CacheItem(Func<object> valueFactory, TimeSpan expires, bool absoluteExpiration)
		{
			_valueFactory = new Lazy<object>(valueFactory);
			_absoluteExpiration = absoluteExpiration;
			_expires = DateTime.UtcNow.Ticks + expires.Ticks;
		}

		public void UpdateUsage(TimeSpan slidingExpiration)
		{
			if (!_absoluteExpiration)
			{
				_expires = DateTime.UtcNow.Ticks + slidingExpiration.Ticks;
			}
		}
	}

	private static readonly Lazy<MemoryCache> _instance = new Lazy<MemoryCache>(() => new MemoryCache());

	private static readonly TimeSpan _cleanupInterval = TimeSpan.FromSeconds(10.0);

	private readonly ConcurrentDictionary<object, CacheItem> _cache = new ConcurrentDictionary<object, CacheItem>();

	private readonly Timer _timer;

	internal static MemoryCache Instance => _instance.Value;

	internal MemoryCache()
	{
		_timer = new Timer(RemoveExpiredEntries, null, _cleanupInterval, _cleanupInterval);
	}

	internal T GetOrAdd<T>(object cacheKey, Func<T> factory, TimeSpan expiration, bool absoluteExpiration = false) where T : class
	{
		CacheItem value = new CacheItem(factory, expiration, absoluteExpiration);
		CacheItem orAdd = _cache.GetOrAdd(cacheKey, value);
		orAdd.UpdateUsage(expiration);
		return (T)orAdd.Value;
	}

	internal bool TryGetValue<T>(object cacheKey, out T value) where T : class
	{
		if (_cache.TryGetValue(cacheKey, out var value2))
		{
			value = (T)value2.Value;
			return true;
		}
		value = null;
		return false;
	}

	internal void Remove(object cacheKey)
	{
		_cache.TryRemove(cacheKey, out var _);
	}

	private void RemoveExpiredEntries(object state)
	{
		foreach (object key in _cache.Keys)
		{
			if (_cache.TryGetValue(key, out var value) && value.Expired)
			{
				_cache.TryRemove(key, out value);
			}
		}
	}

	public void Dispose()
	{
		if (_timer != null)
		{
			_timer.Dispose();
		}
	}
}
