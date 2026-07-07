using System;
using System.Collections.Generic;
using System.Windows;

namespace ModernWpf.Controls;

public class NonVirtualizingLayoutContext : LayoutContext
{
	private VirtualizingLayoutContext m_contextAdapter;

	public IReadOnlyList<UIElement> Children => ChildrenCore;

	protected virtual IReadOnlyList<UIElement> ChildrenCore
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal VirtualizingLayoutContext GetVirtualizingContextAdapter()
	{
		if (m_contextAdapter == null)
		{
			m_contextAdapter = new LayoutContextAdapter(this);
		}
		return m_contextAdapter;
	}
}
