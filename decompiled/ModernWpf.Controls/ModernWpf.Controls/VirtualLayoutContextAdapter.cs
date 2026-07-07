using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace ModernWpf.Controls;

internal class VirtualLayoutContextAdapter : NonVirtualizingLayoutContext
{
	private class ChildrenCollection<T> : IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable where T : UIElement
	{
		private class Iterator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private readonly IReadOnlyList<T> m_childCollection;

			private int m_currentIndex = -1;

			public T Current
			{
				get
				{
					if (m_currentIndex < m_childCollection.Count)
					{
						return m_childCollection[m_currentIndex];
					}
					throw new InvalidOperationException();
				}
			}

			object IEnumerator.Current => Current;

			public Iterator(IReadOnlyList<T> childCollection)
			{
				m_childCollection = childCollection;
			}

			~Iterator()
			{
			}

			public bool MoveNext()
			{
				if (m_currentIndex < m_childCollection.Count)
				{
					m_currentIndex++;
					return m_currentIndex < m_childCollection.Count;
				}
				throw new InvalidOperationException();
			}

			public void Reset()
			{
				m_currentIndex = -1;
			}

			public void Dispose()
			{
			}
		}

		private readonly VirtualizingLayoutContext m_context;

		public int Count => m_context.ItemCount;

		public T this[int index] => (T)(object)m_context.GetOrCreateElementAt(index, ElementRealizationOptions.None);

		public ChildrenCollection(VirtualizingLayoutContext context)
		{
			m_context = context;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new Iterator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	private readonly WeakReference<VirtualizingLayoutContext> m_virtualizingContext;

	private IReadOnlyList<UIElement> m_children;

	protected override object LayoutStateCore
	{
		get
		{
			if (m_virtualizingContext.TryGetTarget(out var target))
			{
				return target.LayoutState;
			}
			return null;
		}
		set
		{
			if (m_virtualizingContext.TryGetTarget(out var target))
			{
				target.LayoutState = value;
			}
		}
	}

	protected override IReadOnlyList<UIElement> ChildrenCore
	{
		get
		{
			if (m_children == null)
			{
				m_virtualizingContext.TryGetTarget(out var target);
				m_children = new ChildrenCollection<UIElement>(target);
			}
			return m_children;
		}
	}

	public VirtualLayoutContextAdapter(VirtualizingLayoutContext virtualizingContext)
	{
		m_virtualizingContext = new WeakReference<VirtualizingLayoutContext>(virtualizingContext);
	}
}
