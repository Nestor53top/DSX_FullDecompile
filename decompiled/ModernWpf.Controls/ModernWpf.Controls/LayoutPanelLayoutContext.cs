using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls;

internal class LayoutPanelLayoutContext : NonVirtualizingLayoutContext
{
	private class UIElementCollectionView : IReadOnlyList<UIElement>, IReadOnlyCollection<UIElement>, IEnumerable<UIElement>, IEnumerable
	{
		private readonly UIElementCollection m_collection;

		public UIElement this[int index] => m_collection[index];

		public int Count => m_collection.Count;

		public UIElementCollectionView(UIElementCollection collection)
		{
			m_collection = collection;
		}

		public IEnumerator<UIElement> GetEnumerator()
		{
			foreach (UIElement item in m_collection)
			{
				yield return item;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_collection.GetEnumerator();
		}
	}

	private readonly WeakReference<LayoutPanel> m_owner;

	protected override IReadOnlyList<UIElement> ChildrenCore => new UIElementCollectionView(((Panel)GetOwner()).Children);

	protected override object LayoutStateCore
	{
		get
		{
			return GetOwner().LayoutState;
		}
		set
		{
			GetOwner().LayoutState = value;
		}
	}

	public LayoutPanelLayoutContext(LayoutPanel owner)
	{
		m_owner = new WeakReference<LayoutPanel>(owner);
	}

	private LayoutPanel GetOwner()
	{
		m_owner.TryGetTarget(out var target);
		return target;
	}
}
