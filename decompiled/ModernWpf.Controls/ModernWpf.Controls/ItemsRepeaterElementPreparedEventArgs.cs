using System;
using System.Windows;

namespace ModernWpf.Controls;

public sealed class ItemsRepeaterElementPreparedEventArgs : EventArgs
{
	public UIElement Element { get; private set; }

	public int Index { get; private set; }

	internal ItemsRepeaterElementPreparedEventArgs(UIElement element, int index)
	{
		Update(element, index);
	}

	internal void Update(UIElement element, int index)
	{
		Element = element;
		Index = index;
	}
}
