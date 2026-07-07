using System;
using System.Collections;
using System.Collections.Generic;

namespace ModernWpf.Controls;

internal class SelectedItems<T> : IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
{
	private class Enumerator : IEnumerator<T>, IDisposable, IEnumerator
	{
		private readonly IReadOnlyList<T> m_selectedItems;

		private int m_currentIndex = -1;

		public T Current
		{
			get
			{
				IReadOnlyList<T> selectedItems = m_selectedItems;
				if (m_currentIndex < selectedItems.Count)
				{
					return selectedItems[m_currentIndex];
				}
				throw new IndexOutOfRangeException();
			}
		}

		object IEnumerator.Current => Current;

		public Enumerator(IReadOnlyList<T> selectedItems)
		{
			m_selectedItems = selectedItems;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (m_currentIndex < m_selectedItems.Count)
			{
				m_currentIndex++;
				return m_currentIndex < m_selectedItems.Count;
			}
			throw new IndexOutOfRangeException();
		}

		public void Reset()
		{
			m_currentIndex = 1;
		}
	}

	private List<SelectedItemInfo> m_infos;

	private int m_totalCount;

	private Func<List<SelectedItemInfo>, int, T> m_getAtImpl;

	public int Count => m_totalCount;

	public T this[int index] => m_getAtImpl(m_infos, index);

	public SelectedItems(List<SelectedItemInfo> infos, Func<List<SelectedItemInfo>, int, T> getAtImpl)
	{
		m_infos = infos;
		m_getAtImpl = getAtImpl;
		foreach (SelectedItemInfo info in infos)
		{
			if (info.Node.TryGetTarget(out var target))
			{
				m_totalCount += target.SelectedCount;
				continue;
			}
			throw new Exception("Selection changed after the SelectedIndices/Items property was read.");
		}
	}

	~SelectedItems()
	{
		m_infos.Clear();
	}

	public IEnumerator<T> GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
