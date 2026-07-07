using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ModernWpf.Controls.Primitives;

public class CommandBarPanel : ToolBarPanel
{
	private bool m_hasChildren;

	internal bool HasChildren
	{
		get
		{
			return m_hasChildren;
		}
		set
		{
			if (m_hasChildren != value)
			{
				m_hasChildren = value;
				this.HasChildrenChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	internal event EventHandler HasChildrenChanged;

	protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
	{
		((Panel)this).OnVisualChildrenChanged(visualAdded, visualRemoved);
		UpdateHasChildren();
	}

	private void UpdateHasChildren()
	{
		bool hasChildren = false;
		for (int i = 0; i < ((Panel)this).InternalChildren.Count; i++)
		{
			if (((Panel)this).InternalChildren[i] != null)
			{
				hasChildren = true;
				break;
			}
		}
		HasChildren = hasChildren;
	}
}
