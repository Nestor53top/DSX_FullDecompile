using System.Windows;

namespace ModernWpf.Controls;

public sealed class ElementFactoryRecycleArgs
{
	public UIElement Parent { get; set; }

	public UIElement Element { get; set; }
}
