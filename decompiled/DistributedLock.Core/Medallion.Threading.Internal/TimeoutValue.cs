using System;
using System.Threading;

namespace Medallion.Threading.Internal;

internal readonly struct TimeoutValue : IEquatable<TimeoutValue>, IComparable<TimeoutValue>
{
	public int InMilliseconds { get; }

	public int InSeconds
	{
		get
		{
			if (!IsInfinite)
			{
				return InMilliseconds / 1000;
			}
			throw new InvalidOperationException("infinite timeout cannot be converted to seconds");
		}
	}

	public bool IsInfinite => InMilliseconds == -1;

	public bool IsZero => InMilliseconds == 0;

	public TimeSpan TimeSpan => TimeSpan.FromMilliseconds((double)InMilliseconds);

	public TimeoutValue(TimeSpan? timeout, string paramName = "timeout")
	{
		if (timeout.HasValue)
		{
			TimeSpan valueOrDefault = timeout.GetValueOrDefault();
			long num = (long)valueOrDefault.TotalMilliseconds;
			if (num < -1 || num > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException(paramName, valueOrDefault, string.Format("Must be {0}.{1} ({2}) or a non-negative value <= {3})", new object[4]
				{
					"Timeout",
					"InfiniteTimeSpan",
					Timeout.InfiniteTimeSpan,
					TimeSpan.FromMilliseconds(2147483647.0)
				}));
			}
			InMilliseconds = (int)num;
		}
		else
		{
			InMilliseconds = -1;
		}
	}

	public bool Equals(TimeoutValue that)
	{
		return InMilliseconds == that.InMilliseconds;
	}

	public override bool Equals(object? obj)
	{
		if (obj is TimeoutValue that)
		{
			return Equals(that);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return InMilliseconds;
	}

	public int CompareTo(TimeoutValue that)
	{
		if (!IsInfinite)
		{
			if (!that.IsInfinite)
			{
				return InMilliseconds.CompareTo(that.InMilliseconds);
			}
			return -1;
		}
		if (!that.IsInfinite)
		{
			return 1;
		}
		return 0;
	}

	public static bool operator ==(TimeoutValue a, TimeoutValue b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(TimeoutValue a, TimeoutValue b)
	{
		return !(a == b);
	}

	public static implicit operator TimeoutValue(TimeSpan? timeout)
	{
		return new TimeoutValue(timeout);
	}

	public override string ToString()
	{
		if (!IsInfinite)
		{
			if (!IsZero)
			{
				return TimeSpan.ToString();
			}
			return "0";
		}
		return "∞";
	}
}
