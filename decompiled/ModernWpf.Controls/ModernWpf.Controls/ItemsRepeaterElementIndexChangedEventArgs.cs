using System;
using System.Windows;

namespace ModernWpf.Controls;

public sealed class ItemsRepeaterElementIndexChangedEventArgs : EventArgs
{
	public UIElement Element { get; private set; }

	public int OldIndex { get; private set; }

	public int NewIndex { get; private set; }

	internal ItemsRepeaterElementIndexChangedEventArgs(UIElement element, int oldIndex, int newIndex)
	{
		Update(element, oldIndex, newIndex);
	}

	internal void Update(UIElement element, int oldIndex, int newIndex)
	{
		Element = element;
		OldIndex = oldIndex;
		NewIndex = newIndex;
	}
}
