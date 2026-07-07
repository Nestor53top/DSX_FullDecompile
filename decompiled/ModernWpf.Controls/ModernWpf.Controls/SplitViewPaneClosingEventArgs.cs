using System;

namespace ModernWpf.Controls;

public sealed class SplitViewPaneClosingEventArgs : EventArgs
{
	public bool Cancel { get; set; }

	internal SplitViewPaneClosingEventArgs()
	{
	}
}
