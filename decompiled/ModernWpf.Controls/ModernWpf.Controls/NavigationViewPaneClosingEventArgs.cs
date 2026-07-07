using System;

namespace ModernWpf.Controls;

public sealed class NavigationViewPaneClosingEventArgs : EventArgs
{
	private SplitViewPaneClosingEventArgs m_splitViewClosingArgs;

	private bool m_cancelled;

	public bool Cancel
	{
		get
		{
			return m_cancelled;
		}
		set
		{
			m_cancelled = value;
			SplitViewPaneClosingEventArgs splitViewClosingArgs = m_splitViewClosingArgs;
			if (splitViewClosingArgs != null)
			{
				splitViewClosingArgs.Cancel = value;
			}
		}
	}

	internal NavigationViewPaneClosingEventArgs()
	{
	}

	internal void SplitViewClosingArgs(SplitViewPaneClosingEventArgs value)
	{
		m_splitViewClosingArgs = value;
	}
}
