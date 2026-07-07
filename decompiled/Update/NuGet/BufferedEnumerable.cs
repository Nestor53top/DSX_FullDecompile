using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NuGet;

internal class BufferedEnumerable<TElement> : IEnumerable<TElement>, IEnumerable
{
	internal class BufferedEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
	{
		private readonly int _bufferSize;

		private IQueryable<T> _source;

		private QueryState<T> _state;

		private int _index = -1;

		public T Current => _state.Cache[_index];

		internal bool IsEmpty
		{
			get
			{
				if (_state.HasItems)
				{
					return _index == _state.Cache.Count - 1;
				}
				return false;
			}
		}

		object IEnumerator.Current => Current;

		public BufferedEnumerator(QueryState<T> state, IQueryable<T> source, int bufferSize)
		{
			_state = state;
			_source = source;
			_bufferSize = bufferSize;
		}

		public void Dispose()
		{
			_source = null;
			_state = null;
		}

		public bool MoveNext()
		{
			if (IsEmpty)
			{
				List<T> list = _source.Skip(_state.Cache.Count).Take(_bufferSize).ToList();
				_state.HasItems = _bufferSize == list.Count;
				_state.Cache.AddRange(list);
			}
			_index++;
			return _index < _state.Cache.Count;
		}

		public void Reset()
		{
			_index = -1;
		}

		public override string ToString()
		{
			return _source.ToString();
		}
	}

	internal class QueryState<T>
	{
		public List<T> Cache { get; private set; }

		public bool HasItems { get; set; }

		public QueryState(int bufferSize)
		{
			Cache = new List<T>(bufferSize);
			HasItems = true;
		}
	}

	private readonly IQueryable<TElement> _source;

	private readonly int _bufferSize;

	private readonly QueryState<TElement> _state;

	public BufferedEnumerable(IQueryable<TElement> source, int bufferSize)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		_state = new QueryState<TElement>(bufferSize);
		_source = source;
		_bufferSize = bufferSize;
	}

	public IEnumerator<TElement> GetEnumerator()
	{
		return new BufferedEnumerator<TElement>(_state, _source, _bufferSize);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public override string ToString()
	{
		return _source.ToString();
	}
}
