using System.Collections;
using System.Collections.Generic;

namespace NuGet;

internal class LazyQueue<TVal> : IEnumerable<TVal>, IEnumerable
{
	private readonly IEnumerator<TVal> _enumerator;

	private TVal _peeked;

	public LazyQueue(IEnumerator<TVal> enumerator)
	{
		_enumerator = enumerator;
	}

	public bool TryPeek(out TVal element)
	{
		element = default(TVal);
		if (_peeked != null)
		{
			element = _peeked;
			return true;
		}
		bool num = _enumerator.MoveNext();
		if (num)
		{
			element = _enumerator.Current;
			_peeked = element;
		}
		return num;
	}

	public void Dequeue()
	{
		_peeked = default(TVal);
	}

	public IEnumerator<TVal> GetEnumerator()
	{
		return _enumerator;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
