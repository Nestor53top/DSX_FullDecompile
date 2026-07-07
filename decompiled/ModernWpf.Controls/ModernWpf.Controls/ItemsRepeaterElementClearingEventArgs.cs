using System;
using System.Windows;

namespace ModernWpf.Controls;

public sealed class ItemsRepeaterElementClearingEventArgs : EventArgs
{
	public UIElement Element { get; private set; }

	internal ItemsRepeaterElementClearingEventArgs(UIElement element)
	{
		Update(element);
	}

	internal void Update(UIElement element)
	{
		Element = element;
	}
}
